using BitMiracle.LibTiff.Classic;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ImageSharpRasterReader : IRasterReader
    {
        private readonly HttpClient _httpClient;

        public ImageSharpRasterReader(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(180);
        }

        public async Task<string> DownloadBandAsync(string url, string tempDir, string filename)
        {
            Directory.CreateDirectory(tempDir);
            var filePath = Path.Combine(tempDir, filename);

            if (File.Exists(filePath))
                return filePath;

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);

            return filePath;
        }

        public Task<RasterData> ReadBandsAsync(string redPath, string nirPath)
        {
            var redData = ReadSingleBand(redPath);
            var nirData = ReadSingleBand(nirPath);

            if (redData.width != nirData.width || redData.height != nirData.height)
                throw new InvalidOperationException(
                    $"Dimensões incompatíveis: Red ({redData.width}x{redData.height}) vs NIR ({nirData.width}x{nirData.height})");

            var raster = new RasterData
            {
                RedBand = redData.pixels,
                NirBand = nirData.pixels,
                Width = redData.width,
                Height = redData.height,
                Scale = 0.0001f,
                NoDataValue = -9999f,
                GeoTransform = redData.geoTransform
            };

            return Task.FromResult(raster);
        }

        private (float[,] pixels, int width, int height, GeoTransform geoTransform) ReadSingleBand(string filePath)
        {
            using var tiff = Tiff.Open(filePath, "r");

            int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            var bitsPerSampleTag = tiff.GetField(TiffTag.BITSPERSAMPLE);
            short bitsPerSample = bitsPerSampleTag != null ? bitsPerSampleTag[0].ToShort() : (short)8;
            var sampleFormatTag = tiff.GetField(TiffTag.SAMPLEFORMAT);
            var sampleFormat = sampleFormatTag != null ? (SampleFormat)sampleFormatTag[0].ToShort() : SampleFormat.UINT;

            var geoTransform = ReadGeoTransform(tiff);

            var pixels = new float[width, height];

            var tileWidthTag = tiff.GetField(TiffTag.TILEWIDTH);
            bool isTiled = tileWidthTag != null && tileWidthTag[0].ToInt() > 0;

            if (isTiled)
            {
                int tileWidth = tiff.GetField(TiffTag.TILEWIDTH)[0].ToInt();
                int tileLength = tiff.GetField(TiffTag.TILELENGTH)[0].ToInt();
                int tileBufferSize = tiff.TileSize();
                var tileBuffer = new byte[tileBufferSize];

                for (int tileY = 0; tileY < height; tileY += tileLength)
                {
                    for (int tileX = 0; tileX < width; tileX += tileWidth)
                    {
                        int bytesRead = tiff.ReadTile(tileBuffer, 0, tileX, tileY, 0, 0);
                        if (bytesRead <= 0) continue;

                        int bytesPerPixel = bitsPerSample / 8;

                        for (int y = 0; y < tileLength && (tileY + y) < height; y++)
                        {
                            for (int x = 0; x < tileWidth && (tileX + x) < width; x++)
                            {
                                int srcIdx = (y * tileWidth + x) * bytesPerPixel;
                                if (srcIdx + bytesPerPixel > tileBuffer.Length) continue;

                                float value = ParsePixelValue(tileBuffer, srcIdx, bitsPerSample, sampleFormat);
                                pixels[tileX + x, tileY + y] = value;
                            }
                        }
                    }
                }
            }
            else
            {
                int scanlineSize = tiff.ScanlineSize();
                var buffer = new byte[scanlineSize];

                for (int y = 0; y < height; y++)
                {
                    tiff.ReadScanline(buffer, y);

                    int bytesPerPixel = bitsPerSample / 8;
                    for (int x = 0; x < width; x++)
                    {
                        int srcIdx = x * bytesPerPixel;
                        if (srcIdx + bytesPerPixel > buffer.Length) continue;

                        pixels[x, y] = ParsePixelValue(buffer, srcIdx, bitsPerSample, sampleFormat);
                    }
                }
            }

            return (pixels, width, height, geoTransform);
        }

        private GeoTransform ReadGeoTransform(Tiff tiff)
        {
            var gt = new GeoTransform();

            var pixelScaleTag = tiff.GetField((TiffTag)33550);
            if (pixelScaleTag != null && pixelScaleTag[0].ToDoubleArray() is double[] scales && scales.Length >= 2)
            {
                gt.PixelWidth = scales[0];
                gt.PixelHeight = scales[1];
            }
            else
            {
                gt.PixelWidth = 1.0 / 111320.0;
                gt.PixelHeight = 1.0 / 111320.0;
            }

            var tiepointTag = tiff.GetField((TiffTag)33922);
            if (tiepointTag != null)
            {
                var raw = tiepointTag[0].ToDoubleArray();
                if (raw != null && raw.Length >= 6)
                {
                    double tieI = raw[0];
                    double tieJ = raw[1];
                    double tieX = raw[3];
                    double tieY = raw[4];

                    gt.OriginX = tieX - tieI * gt.PixelWidth;
                    gt.OriginY = tieY + tieJ * gt.PixelHeight;
                }
            }

            return gt;
        }

        private static float ParsePixelValue(byte[] buffer, int offset, short bitsPerSample, SampleFormat sampleFormat)
        {
            float value;
            switch (bitsPerSample)
            {
                case 16 when sampleFormat == SampleFormat.INT:
                    short signed = (short)(buffer[offset] | (buffer[offset + 1] << 8));
                    value = signed < 0 ? float.NaN : signed * 0.0001f;
                    break;
                case 16 when sampleFormat == SampleFormat.UINT:
                    ushort unsigned = (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
                    value = unsigned == 0 ? float.NaN : unsigned * 0.0001f;
                    break;
                case 32 when sampleFormat == SampleFormat.IEEEFP:
                    int rawInt = buffer[offset] | (buffer[offset + 1] << 8) |
                                 (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24);
                    value = BitConverter.Int32BitsToSingle(rawInt);
                    break;
                case 8:
                    value = buffer[offset];
                    break;
                default:
                    value = float.NaN;
                    break;
            }

            if (float.IsNaN(value) || value > 1.5f)
                return float.NaN;

            return value;
        }
    }
}
