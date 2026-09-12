namespace MCR.API.Services.Interfaces
{
    public interface IColorMapGenerator
    {
        byte[] GeneratePng(float[,] ndviGrid, int width, int height, bool[,] polygonMask = null);
    }
}
