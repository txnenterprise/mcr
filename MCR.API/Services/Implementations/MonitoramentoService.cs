using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Helpers;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class MonitoramentoService : IMonitoramentoService
    {
        private readonly DbContextMCR _context;
        private readonly ISatelliteProvider _satelliteProvider;

        private static readonly Random _random = new();
        private static readonly string[] _satelites = { "CBERS-4A", "CBERS-4", "SENTINEL-2A", "LANDSAT-8" };
        private static readonly string[] _statusExecucao = { "Concluído", "Concluído", "Concluído", "Processando", "Falha" };

        public MonitoramentoService(DbContextMCR context, ISatelliteProvider satelliteProvider)
        {
            _context = context;
            _satelliteProvider = satelliteProvider;
        }

        public async Task<MonitoringConfigurationEntity> ObterConfiguracaoAsync(Guid propertyId)
        {
            var config = await _context.MonitoringConfigurations
                .FirstOrDefaultAsync(c => c.PropertyId == propertyId);

            if (config == null)
            {
                config = new MonitoringConfigurationEntity
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    Enabled = false,
                    UpdateFrequency = "Semanal",
                    CloudLimit = 20.0m,
                    CreatedAt = DateTime.UtcNow,
                    Sucesso = true,
                    Mensagem = "Configuração padrão criada."
                };
            }
            else
            {
                config.Sucesso = true;
                config.Mensagem = "Configuração obtida com sucesso.";
            }

            return config;
        }

        public async Task<MonitoringConfigurationEntity> AtivarMonitoramentoAsync(Guid propertyId)
        {
            var config = await _context.MonitoringConfigurations
                .FirstOrDefaultAsync(c => c.PropertyId == propertyId);

            if (config == null)
            {
                config = new MonitoringConfigurationEntity
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    Enabled = true,
                    UpdateFrequency = "Semanal",
                    CloudLimit = 20.0m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.MonitoringConfigurations.Add(config);
            }
            else
            {
                config.Enabled = true;
                config.UpdatedAt = DateTime.UtcNow;
                _context.MonitoringConfigurations.Update(config);
            }

            await _context.SaveChangesAsync();

            config.Sucesso = true;
            config.Mensagem = "Monitoramento ativado com sucesso.";
            return config;
        }

        public async Task<MonitoringConfigurationEntity> DesativarMonitoramentoAsync(Guid propertyId)
        {
            var config = await _context.MonitoringConfigurations
                .FirstOrDefaultAsync(c => c.PropertyId == propertyId);

            if (config == null)
            {
                return new MonitoringConfigurationEntity
                {
                    Sucesso = false,
                    Mensagem = "Configuração de monitoramento não encontrada."
                };
            }

            config.Enabled = false;
            config.UpdatedAt = DateTime.UtcNow;
            _context.MonitoringConfigurations.Update(config);
            await _context.SaveChangesAsync();

            config.Sucesso = true;
            config.Mensagem = "Monitoramento desativado com sucesso.";
            return config;
        }

        public async Task<IEnumerable<MonitoringExecutionEntity>> ObterExecucoesAsync(Guid propertyId, int page = 1, int pageSize = 10)
        {
            var query = _context.MonitoringExecutions
                .Where(e => e.PropertyId == propertyId)
                .OrderByDescending(e => e.ExecutionDate);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var exec in results)
            {
                exec.Sucesso = true;
                exec.Mensagem = $"Total: {totalItems}, Página: {page}/{totalPages}";
            }

            return results;
        }

        public async Task<MonitoringExecutionEntity> ExecutarAnaliseAsync(Guid propertyId)
        {
            var execution = new MonitoringExecutionEntity
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                ExecutionDate = DateTime.UtcNow,
                Status = _statusExecucao[_random.Next(_statusExecucao.Length)],
                Satellite = _satelites[_random.Next(_satelites.Length)],
                CloudCover = Math.Round((decimal)(_random.NextDouble() * 30), 2),
                AverageVegetationIndex = Math.Round((decimal)(_random.NextDouble() * 0.8 + 0.1), 4),
                AffectedArea = Math.Round((decimal)(_random.NextDouble() * 50), 2),
                Message = "Análise simulada executada com sucesso (MVP).",
                JsonStatistics = System.Text.Json.JsonSerializer.Serialize(new
                {
                    ndvi = Math.Round((decimal)(_random.NextDouble() * 0.8 + 0.1), 4),
                    ndwi = Math.Round((decimal)(_random.NextDouble() * 0.5), 4),
                    evi = Math.Round((decimal)(_random.NextDouble() * 0.6 + 0.1), 4),
                    areaTotal = 100.0,
                    areaAffectada = Math.Round((decimal)(_random.NextDouble() * 50), 2),
                    ph = Math.Round((decimal)(_random.NextDouble() * 3 + 5), 2)
                }),
                CreatedAt = DateTime.UtcNow,
                Sucesso = true,
                Mensagem = "Análise simulada criada."
            };

            _context.MonitoringExecutions.Add(execution);
            await _context.SaveChangesAsync();

            if (execution.AverageVegetationIndex < 0.3m)
            {
                var alert = new MonitoringAlertEntity
                {
                    Id = Guid.NewGuid(),
                    ExecutionId = execution.Id,
                    Severity = "Alta",
                    Message = $"Índice de vegetação baixo ({execution.AverageVegetationIndex:F4}) detectado na propriedade.",
                    Viewed = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.MonitoringAlerts.Add(alert);
                await _context.SaveChangesAsync();
            }

            if (execution.CloudCover > 25m)
            {
                var alert = new MonitoringAlertEntity
                {
                    Id = Guid.NewGuid(),
                    ExecutionId = execution.Id,
                    Severity = "Média",
                    Message = $"Cobertura de nuvens elevada ({execution.CloudCover}%) pode afetar a qualidade da análise.",
                    Viewed = false,
                    CreatedAt = DateTime.UtcNow
                };
                _context.MonitoringAlerts.Add(alert);
                await _context.SaveChangesAsync();
            }

            return execution;
        }

        public async Task<IEnumerable<MonitoringAlertEntity>> ObterAlertasAsync(Guid propertyId, bool? apenasNaoVisualizados = null)
        {
            var query = from alert in _context.MonitoringAlerts
                        join execution in _context.MonitoringExecutions on alert.ExecutionId equals execution.Id
                        where execution.PropertyId == propertyId
                        orderby alert.CreatedAt descending
                        select alert;

            if (apenasNaoVisualizados.HasValue)
                query = query.Where(a => a.Viewed == apenasNaoVisualizados.Value);

            var results = await query.Take(50).ToListAsync();

            foreach (var alert in results)
            {
                alert.Sucesso = true;
                alert.Mensagem = "Alerta obtido.";
            }

            return results;
        }

        public async Task<MonitoringAlertEntity> MarcarAlertaVisualizadoAsync(Guid alertId)
        {
            var alert = await _context.MonitoringAlerts.FindAsync(alertId);
            if (alert == null)
            {
                return new MonitoringAlertEntity
                {
                    Sucesso = false,
                    Mensagem = "Alerta não encontrado."
                };
            }

            alert.Viewed = true;
            _context.MonitoringAlerts.Update(alert);
            await _context.SaveChangesAsync();

            alert.Sucesso = true;
            alert.Mensagem = "Alerta marcado como visualizado.";
            return alert;
        }

        public async Task<List<SatelliteSceneEntity>> BuscarCenasSateliteAsync(
            Guid propertyId, Guid talhaoId, DateTime dataInicio, DateTime dataFim, decimal? nuvemMaxima = null)
        {
            var talhao = await _context.Talhoes.FindAsync(talhaoId);
            if (talhao == null || talhao.PropriedadeId != propertyId)
            {
                return new List<SatelliteSceneEntity>();
            }

            var bbox = KmlHelper.ExtractBboxFromKml(talhao.KmlTalhao);
            if (bbox == null)
            {
                return new List<SatelliteSceneEntity>();
            }

            var config = await ObterConfiguracaoAsync(propertyId);
            var cloudLimit = nuvemMaxima ?? config.CloudLimit ?? 20.0m;

            var collection = "CB4A-MUX-L4-SR-1";

            var stacScenes = await _satelliteProvider.SearchScenesAsync(
                bbox, dataInicio, dataFim, collection, cloudLimit, limit: 20);

            var savedScenes = new List<SatelliteSceneEntity>();

            foreach (var scene in stacScenes)
            {
                var exists = await _context.SatelliteScenes
                    .AnyAsync(s => s.SceneId == scene.Id && s.TalhaoId == talhaoId);

                if (exists)
                    continue;

                var primaryAsset = scene.Assets.Values
                    .FirstOrDefault(a => a.Type?.Contains("geotiff") == true && a.Roles?.Contains("data") == true);

                var entity = new SatelliteSceneEntity
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    TalhaoId = talhaoId,
                    Collection = collection,
                    SceneId = scene.Id,
                    AcquisitionDate = scene.AcquisitionDate ?? DateTime.MinValue,
                    CloudCover = scene.CloudCover,
                    AssetUrl = primaryAsset?.Href ?? "",
                    Bbox = scene.Bbox != null ? System.Text.Json.JsonSerializer.Serialize(scene.Bbox) : "",
                    AssetsJson = System.Text.Json.JsonSerializer.Serialize(scene.Assets),
                    CreatedAt = DateTime.UtcNow,
                    Sucesso = true,
                    Mensagem = "Cena salva com sucesso."
                };

                _context.SatelliteScenes.Add(entity);
                savedScenes.Add(entity);
            }

            if (savedScenes.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            foreach (var scene in savedScenes)
            {
                scene.Mensagem = $"Cena {scene.SceneId} salva.";
            }

            return savedScenes;
        }

        public async Task<List<SatelliteSceneEntity>> ObterCenasSalvasAsync(Guid propertyId, Guid? talhaoId = null)
        {
            IQueryable<SatelliteSceneEntity> query = _context.SatelliteScenes
                .Where(s => s.PropertyId == propertyId);

            if (talhaoId.HasValue)
                query = query.Where(s => s.TalhaoId == talhaoId.Value);

            var scenes = await query.OrderByDescending(s => s.AcquisitionDate).ToListAsync();

            foreach (var scene in scenes)
            {
                scene.Sucesso = true;
                scene.Mensagem = "Cena obtida.";
            }

            return scenes;
        }
    }
}
