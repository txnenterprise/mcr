using System.Globalization;
using System.Xml;

namespace MCR.API.Helpers
{
    public static class PolygonMaskHelper
    {
        public static bool[,] CreateMaskFromKml(string kml, int imageWidth, int imageHeight,
            double originX, double originY, double pixelWidth, double pixelHeight)
        {
            var mask = new bool[imageWidth, imageHeight];

            var polygons = ExtractPolygonCoordinates(kml);
            if (polygons == null || polygons.Count == 0)
            {
                for (int y = 0; y < imageHeight; y++)
                    for (int x = 0; x < imageWidth; x++)
                        mask[x, y] = true;
                return mask;
            }

            var pixelPolygons = new List<List<(int x, int y)>>();
            foreach (var polygon in polygons)
            {
                var pixelPoly = new List<(int x, int y)>();
                foreach (var (lon, lat) in polygon)
                {
                    int col = (int)Math.Round((lon - originX) / pixelWidth);
                    int row = (int)Math.Round((originY - lat) / pixelHeight);
                    pixelPoly.Add((col, row));
                }
                pixelPolygons.Add(pixelPoly);
            }

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    foreach (var pixelPoly in pixelPolygons)
                    {
                        if (IsPointInPolygon(x, y, pixelPoly))
                        {
                            mask[x, y] = true;
                            break;
                        }
                    }
                }
            }

            return mask;
        }

        public static bool[,] CreateMaskFromKmlWithBbox(string kml, int imageWidth, int imageHeight,
            double[] bbox)
        {
            if (bbox == null || bbox.Length < 4)
                return null;

            double minLon = bbox[0];
            double minLat = bbox[1];
            double maxLon = bbox[2];
            double maxLat = bbox[3];

            double originX = minLon;
            double originY = maxLat;
            double pixelWidth = (maxLon - minLon) / imageWidth;
            double pixelHeight = (maxLat - minLat) / imageHeight;

            return CreateMaskFromKml(kml, imageWidth, imageHeight,
                originX, originY, pixelWidth, pixelHeight);
        }

        private static List<List<(double lon, double lat)>> ExtractPolygonCoordinates(string kml)
        {
            var result = new List<List<(double lon, double lat)>>();

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(kml);

                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

                var coordinatesNodes = doc.SelectNodes("//kml:coordinates", nsManager);
                if (coordinatesNodes == null)
                    return result;

                foreach (XmlNode coordNode in coordinatesNodes)
                {
                    var coords = new List<(double lon, double lat)>();
                    var coordText = coordNode.InnerText?.Trim();
                    if (string.IsNullOrEmpty(coordText))
                        continue;

                    var pairs = coordText.Split(new[] { ' ', '\n', '\r', '\t' },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (var pair in pairs)
                    {
                        var parts = pair.Split(',');
                        if (parts.Length < 2)
                            continue;

                        if (double.TryParse(parts[0], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double lon) &&
                            double.TryParse(parts[1], NumberStyles.Float,
                            CultureInfo.InvariantCulture, out double lat))
                        {
                            coords.Add((lon, lat));
                        }
                    }

                    if (coords.Count > 2)
                        result.Add(coords);
                }
            }
            catch
            {
                // KML parsing failed
            }

            return result;
        }

        private static bool IsPointInPolygon(int px, int py, List<(int x, int y)> polygon)
        {
            bool inside = false;
            int n = polygon.Count;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                var (xi, yi) = polygon[i];
                var (xj, yj) = polygon[j];

                if (((yi > py) != (yj > py)) &&
                    (px < (xj - xi) * (double)(py - yi) / (yj - yi) + xi))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static (double minLon, double minLat, double maxLon, double maxLat) GetBboxFromKml(string kml)
        {
            var polygons = ExtractPolygonCoordinates(kml);
            if (polygons == null || polygons.Count == 0)
                return (0, 0, 0, 0);

            double minLon = double.MaxValue, minLat = double.MaxValue;
            double maxLon = double.MinValue, maxLat = double.MinValue;

            foreach (var polygon in polygons)
            {
                foreach (var (lon, lat) in polygon)
                {
                    if (lon < minLon) minLon = lon;
                    if (lon > maxLon) maxLon = lon;
                    if (lat < minLat) minLat = lat;
                    if (lat > maxLat) maxLat = lat;
                }
            }

            return (minLon, minLat, maxLon, maxLat);
        }
    }
}
