using System.Globalization;
using System.Xml;

namespace MCR.API.Helpers
{
    public static class KmlHelper
    {
        public static double[] ExtractBboxFromKml(string kml)
        {
            if (string.IsNullOrWhiteSpace(kml))
                return null;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(kml);

                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

                var coordinatesNodes = doc.SelectNodes("//kml:coordinates", nsManager);
                if (coordinatesNodes == null || coordinatesNodes.Count == 0)
                    return null;

                double minLon = double.MaxValue, minLat = double.MaxValue;
                double maxLon = double.MinValue, maxLat = double.MinValue;

                foreach (XmlNode coordNode in coordinatesNodes)
                {
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
                            if (lon < minLon) minLon = lon;
                            if (lon > maxLon) maxLon = lon;
                            if (lat < minLat) minLat = lat;
                            if (lat > maxLat) maxLat = lat;
                        }
                    }
                }

                if (minLon == double.MaxValue)
                    return null;

                return new[] { minLon, minLat, maxLon, maxLat };
            }
            catch
            {
                return null;
            }
        }

        public static List<double[]> ExtractPolygonsFromKml(string kml)
        {
            var polygons = new List<double[]>();

            if (string.IsNullOrWhiteSpace(kml))
                return polygons;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(kml);

                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

                var coordinatesNodes = doc.SelectNodes("//kml:coordinates", nsManager);
                if (coordinatesNodes == null)
                    return polygons;

                foreach (XmlNode coordNode in coordinatesNodes)
                {
                    var coords = new List<double[]>();
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
                            coords.Add(new[] { lon, lat });
                        }
                    }

                    if (coords.Count > 0)
                        polygons.Add(coords.SelectMany(c => c).ToArray());
                }
            }
            catch
            {
                // KML parsing failed silently
            }

            return polygons;
        }
    }
}
