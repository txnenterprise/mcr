using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ColorMapGenerator : IColorMapGenerator
    {
        public byte[] GeneratePng(float[,] ndviGrid, int width, int height, bool[,] polygonMask = null)
        {
            using var image = new Image<Rgba32>(width, height);

            bool hasMask = polygonMask != null &&
                           polygonMask.GetLength(0) == width &&
                           polygonMask.GetLength(1) == height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (hasMask && !polygonMask[x, y])
                    {
                        image[x, y] = new Rgba32(0, 0, 0, 0);
                        continue;
                    }

                    float ndvi = ndviGrid[x, y];

                    if (float.IsNaN(ndvi))
                    {
                        image[x, y] = new Rgba32(200, 200, 200, 255);
                        continue;
                    }

                    image[x, y] = GetNdviColor(ndvi);
                }
            }

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }

        private static Rgba32 GetNdviColor(float ndvi)
        {
            ndvi = Math.Clamp(ndvi, 0f, 1f);

            if (ndvi < 0.2f)
                return LerpColor(
                    new Rgba32(220, 50, 50),
                    new Rgba32(230, 170, 50),
                    ndvi / 0.2f);

            if (ndvi < 0.4f)
                return LerpColor(
                    new Rgba32(230, 170, 50),
                    new Rgba32(200, 220, 50),
                    (ndvi - 0.2f) / 0.2f);

            if (ndvi < 0.6f)
                return LerpColor(
                    new Rgba32(200, 220, 50),
                    new Rgba32(100, 200, 50),
                    (ndvi - 0.4f) / 0.2f);

            return LerpColor(
                new Rgba32(100, 200, 50),
                new Rgba32(0, 150, 30),
                (ndvi - 0.6f) / 0.4f);
        }

        private static Rgba32 LerpColor(Rgba32 a, Rgba32 b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return new Rgba32(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                255);
        }
    }
}
