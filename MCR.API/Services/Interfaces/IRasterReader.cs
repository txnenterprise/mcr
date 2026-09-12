namespace MCR.API.Services.Interfaces
{
    public class RasterData
    {
        public float[,] RedBand { get; set; }
        public float[,] NirBand { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Scale { get; set; } = 0.0001f;
        public float NoDataValue { get; set; } = -9999f;
        public GeoTransform GeoTransform { get; set; }
    }

    public class GeoTransform
    {
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double PixelWidth { get; set; }
        public double PixelHeight { get; set; }

        public (double lon, double lat) PixelToGeo(int col, int row)
        {
            double lon = OriginX + col * PixelWidth;
            double lat = OriginY - row * PixelHeight;
            return (lon, lat);
        }

        public (int col, int row) GeoToPixel(double lon, double lat)
        {
            int col = (int)Math.Round((lon - OriginX) / PixelWidth);
            int row = (int)Math.Round((OriginY - lat) / PixelHeight);
            return (col, row);
        }
    }

    public interface IRasterReader
    {
        Task<RasterData> ReadBandsAsync(string redPath, string nirPath);
        Task<string> DownloadBandAsync(string url, string tempDir, string filename);
    }
}
