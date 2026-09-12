using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class NdviCalculator : INdviCalculator
    {
        public NdviResult Calculate(RasterData raster, bool[,] polygonMask = null)
        {
            int width = raster.Width;
            int height = raster.Height;
            var ndviGrid = new float[width, height];

            bool hasMask = polygonMask != null &&
                           polygonMask.GetLength(0) == width &&
                           polygonMask.GetLength(1) == height;

            double sum = 0;
            double min = 2;
            double max = -2;
            int validCount = 0;
            int polygonPixelCount = 0;
            int totalPixels = width * height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (hasMask && !polygonMask[x, y])
                    {
                        ndviGrid[x, y] = float.NaN;
                        continue;
                    }

                    if (hasMask) polygonPixelCount++;

                    float red = raster.RedBand[x, y];
                    float nir = raster.NirBand[x, y];

                    if (float.IsNaN(red) || float.IsNaN(nir) ||
                        red < 0 || nir < 0 || (red == 0 && nir == 0))
                    {
                        ndviGrid[x, y] = float.NaN;
                        continue;
                    }

                    float denominator = nir + red;
                    if (Math.Abs(denominator) < 0.0001f)
                    {
                        ndviGrid[x, y] = float.NaN;
                        continue;
                    }

                    float ndvi = (nir - red) / denominator;
                    ndvi = Math.Clamp(ndvi, -1f, 1f);

                    ndviGrid[x, y] = ndvi;
                    double ndviDouble = ndvi;

                    sum += ndviDouble;
                    validCount++;
                    if (ndviDouble < min) min = ndviDouble;
                    if (ndviDouble > max) max = ndviDouble;
                }
            }

            return new NdviResult
            {
                NdviGrid = ndviGrid,
                Width = width,
                Height = height,
                AverageNdvi = validCount > 0 ? Math.Round((decimal)sum / validCount, 4) : 0,
                MinimumNdvi = validCount > 0 ? Math.Round((decimal)min, 4) : 0,
                MaximumNdvi = validCount > 0 ? Math.Round((decimal)max, 4) : 0,
                ValidPixelCount = validCount,
                TotalPixels = totalPixels,
                PolygonPixelCount = hasMask ? polygonPixelCount : totalPixels
            };
        }
    }
}
