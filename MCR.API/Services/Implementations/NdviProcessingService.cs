using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Helpers;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class NdviProcessingService : INdviProcessingService
    {
        private readonly DbContextMCR _context;
        private readonly IRasterReader _rasterReader;
        private readonly INdviCalculator _ndviCalculator;
        private readonly IColorMapGenerator _colorMapGenerator;

        public NdviProcessingService(
            DbContextMCR context,
            IRasterReader rasterReader,
            INdviCalculator ndviCalculator,
            IColorMapGenerator colorMapGenerator)
        {
            _context = context;
            _rasterReader = rasterReader;
            _ndviCalculator = ndviCalculator;
            _colorMapGenerator = colorMapGenerator;
        }

        public async Task<NdviAnalysisResult> ProcessSceneAsync(Guid satelliteSceneId, Guid talhaoId, bool forceReprocess = false)
        {
            var scene = await _context.SatelliteScenes.FindAsync(satelliteSceneId);
            if (scene == null)
                return new NdviAnalysisResult { Success = false, ErrorMessage = "Cena não encontrada." };

            var talhao = await _context.Talhoes.FindAsync(talhaoId);
            if (talhao == null || talhao.PropriedadeId != scene.PropertyId)
                return new NdviAnalysisResult { Success = false, ErrorMessage = "Talhão não pertence à propriedade da cena." };

            var existingAnalysis = await _context.MonitoringAnalyses
                .FirstOrDefaultAsync(a => a.SatelliteSceneId == satelliteSceneId && a.TalhaoId == talhaoId);
            if (existingAnalysis != null && !forceReprocess)
                return new NdviAnalysisResult
                {
                    Success = true,
                    Analysis = existingAnalysis,
                    PreviewImageBase64 = existingAnalysis.PreviewImageBase64
                };

            if (existingAnalysis != null && forceReprocess)
            {
                _context.MonitoringAnalyses.Remove(existingAnalysis);
                await _context.SaveChangesAsync();
            }

            var assets = JsonSerializer.Deserialize<Dictionary<string, StacAssetDto>>(scene.AssetsJson);
            if (assets == null)
                return new NdviAnalysisResult { Success = false, ErrorMessage = "Assets da cena não disponíveis." };

            var redAssetEntry = assets.FirstOrDefault(a =>
                a.Key.Equals("B7", StringComparison.OrdinalIgnoreCase) ||
                a.Key.Equals("BAND7", StringComparison.OrdinalIgnoreCase) ||
                a.Key.Equals("B07", StringComparison.OrdinalIgnoreCase));

            if (redAssetEntry.Value == null)
                redAssetEntry = assets.FirstOrDefault(a =>
                    a.Key.Contains("7", StringComparison.OrdinalIgnoreCase) &&
                    (a.Value.Type?.Contains("geotiff") == true || a.Value.Type?.Contains("tiff") == true));

            var nirAssetEntry = assets.FirstOrDefault(a =>
                a.Key.Equals("B8", StringComparison.OrdinalIgnoreCase) ||
                a.Key.Equals("BAND8", StringComparison.OrdinalIgnoreCase) ||
                a.Key.Equals("B08", StringComparison.OrdinalIgnoreCase));

            if (nirAssetEntry.Value == null)
                nirAssetEntry = assets.FirstOrDefault(a =>
                    a.Key.Contains("8", StringComparison.OrdinalIgnoreCase) &&
                    (a.Value.Type?.Contains("geotiff") == true || a.Value.Type?.Contains("tiff") == true));

            var redAsset = redAssetEntry.Value;
            var nirAsset = nirAssetEntry.Value;

            if (redAsset == null || nirAsset == null)
                return new NdviAnalysisResult
                {
                    Success = false,
                    ErrorMessage = "Bandas B7 (Red) e/ou B8 (NIR) não encontradas nos assets da cena. Chaves disponíveis: " +
                        string.Join(", ", assets.Keys)
                };

            var tempDir = Path.Combine(Path.GetTempPath(), "mcr-raster", Guid.NewGuid().ToString("N"));

            try
            {
                var redPath = await _rasterReader.DownloadBandAsync(
                    redAsset.Href, tempDir, "B7_RED.tif");

                var nirPath = await _rasterReader.DownloadBandAsync(
                    nirAsset.Href, tempDir, "B8_NIR.tif");

                var raster = await _rasterReader.ReadBandsAsync(redPath, nirPath);

                bool[,] polygonMask = null;
                double[] sceneBbox = null;
                if (!string.IsNullOrEmpty(scene.Bbox))
                {
                    try { sceneBbox = System.Text.Json.JsonSerializer.Deserialize<double[]>(scene.Bbox); } catch { }
                }

                bool hasValidGeoTransform = raster.GeoTransform != null &&
                    raster.GeoTransform.OriginX != 0 && raster.GeoTransform.OriginY != 0;

                if (!string.IsNullOrEmpty(talhao.KmlTalhao))
                {
                    if (hasValidGeoTransform)
                    {
                        polygonMask = PolygonMaskHelper.CreateMaskFromKml(
                            talhao.KmlTalhao,
                            raster.Width, raster.Height,
                            raster.GeoTransform.OriginX, raster.GeoTransform.OriginY,
                            raster.GeoTransform.PixelWidth, raster.GeoTransform.PixelHeight);
                    }
                    else if (sceneBbox != null)
                    {
                        polygonMask = PolygonMaskHelper.CreateMaskFromKmlWithBbox(
                            talhao.KmlTalhao,
                            raster.Width, raster.Height,
                            sceneBbox);
                    }
                }

                var ndviResult = _ndviCalculator.Calculate(raster, polygonMask);

                var pngBytes = _colorMapGenerator.GeneratePng(
                    ndviResult.NdviGrid, ndviResult.Width, ndviResult.Height, polygonMask);
                var previewBase64 = Convert.ToBase64String(pngBytes);

                decimal healthyPercent = 0;
                decimal criticalPercent = 0;

                if (ndviResult.ValidPixelCount > 0)
                {
                    int healthyCount = 0;
                    int criticalCount = 0;

                    for (int y = 0; y < ndviResult.Height; y++)
                    {
                        for (int x = 0; x < ndviResult.Width; x++)
                        {
                            float ndvi = ndviResult.NdviGrid[x, y];
                            if (float.IsNaN(ndvi)) continue;

                            if (ndvi >= (float)NdviClassification.BomMin) healthyCount++;
                            else if (ndvi < (float)NdviClassification.AtencaoMin) criticalCount++;
                        }
                    }

                    healthyPercent = Math.Round((decimal)healthyCount / ndviResult.ValidPixelCount * 100, 1);
                    criticalPercent = Math.Round((decimal)criticalCount / ndviResult.ValidPixelCount * 100, 1);
                }

                var classification = NdviClassification.Classificar(ndviResult.AverageNdvi);

                var analysis = new MonitoringAnalysisEntity
                {
                    Id = Guid.NewGuid(),
                    PropertyId = scene.PropertyId,
                    TalhaoId = talhaoId,
                    SatelliteSceneId = satelliteSceneId,
                    SceneId = scene.SceneId,
                    AcquisitionDate = scene.AcquisitionDate,
                    AverageNdvi = ndviResult.AverageNdvi,
                    MinimumNdvi = ndviResult.MinimumNdvi,
                    MaximumNdvi = ndviResult.MaximumNdvi,
                    ValidPixelCount = ndviResult.ValidPixelCount,
                    HealthyAreaPercent = healthyPercent,
                    CriticalAreaPercent = criticalPercent,
                    Classification = classification,
                    PreviewImageBase64 = previewBase64,
                    JsonStatistics = JsonSerializer.Serialize(new
                    {
                        averageNdvi = ndviResult.AverageNdvi,
                        minimumNdvi = ndviResult.MinimumNdvi,
                        maximumNdvi = ndviResult.MaximumNdvi,
                        validPixels = ndviResult.ValidPixelCount,
                        totalPixels = ndviResult.TotalPixels,
                        healthyPercent,
                        criticalPercent,
                        classification,
                        sceneId = scene.SceneId,
                        acquisitionDate = scene.AcquisitionDate.ToString("dd/MM/yyyy")
                    }),
                    CreatedAt = DateTime.UtcNow,
                    Sucesso = true,
                    Mensagem = "Análise NDVI processada com sucesso."
                };

                _context.MonitoringAnalyses.Add(analysis);
                await _context.SaveChangesAsync();

                return new NdviAnalysisResult
                {
                    Success = true,
                    Analysis = analysis,
                    PreviewImageBase64 = previewBase64
                };
            }
            catch (Exception ex)
            {
                return new NdviAnalysisResult
                {
                    Success = false,
                    ErrorMessage = $"Erro no processamento: {ex.Message}"
                };
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch { }
                }
            }
        }

        public async Task<List<MonitoringAnalysisEntity>> ObterAnalisesAsync(Guid propertyId, Guid? talhaoId = null)
        {
            IQueryable<MonitoringAnalysisEntity> query = _context.MonitoringAnalyses
                .Where(a => a.PropertyId == propertyId);

            if (talhaoId.HasValue)
                query = query.Where(a => a.TalhaoId == talhaoId.Value);

            var analyses = await query
                .OrderByDescending(a => a.AcquisitionDate)
                .ToListAsync();

            foreach (var a in analyses)
            {
                a.Sucesso = true;
                a.Mensagem = "Análise obtida.";
            }

            return analyses;
        }
    }
}
