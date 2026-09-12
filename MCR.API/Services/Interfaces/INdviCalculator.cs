namespace MCR.API.Services.Interfaces
{
    public class NdviResult
    {
        public float[,] NdviGrid { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal AverageNdvi { get; set; }
        public decimal MinimumNdvi { get; set; }
        public decimal MaximumNdvi { get; set; }
        public int ValidPixelCount { get; set; }
        public int TotalPixels { get; set; }
        public int PolygonPixelCount { get; set; }
    }

    public interface INdviCalculator
    {
        NdviResult Calculate(RasterData raster, bool[,] polygonMask = null);
    }
}
