using System.Net.Http.Json;
using System.Text.Json;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class CbersStacProvider : ISatelliteProvider
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://data.inpe.br/bdc/stac/v1";

        public CbersStacProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<StacSceneResult>> SearchScenesAsync(
            double[] bbox,
            DateTime startDate,
            DateTime endDate,
            string collection,
            decimal? maxCloudCover = null,
            int limit = 20)
        {
            var searchRequest = new
            {
                collections = new[] { collection },
                bbox = bbox,
                datetime = $"{startDate:yyyy-MM-ddTHH:mm:ssZ}/{endDate:yyyy-MM-ddTHH:mm:ssZ}",
                limit
            };

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/search", searchRequest);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var stacResponse = JsonSerializer.Deserialize<StacSearchResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (stacResponse?.Features == null)
                return new List<StacSceneResult>();

            var scenes = new List<StacSceneResult>();

            foreach (var feature in stacResponse.Features)
            {
                decimal? cloudCover = null;
                if (feature.Properties != null &&
                    feature.Properties.TryGetValue("eo:cloud_cover", out var ccValue))
                {
                    if (ccValue is JsonElement ccElement)
                    {
                        if (ccElement.ValueKind == JsonValueKind.Number)
                            cloudCover = ccElement.GetDecimal();
                        else if (ccElement.ValueKind == JsonValueKind.String &&
                                 decimal.TryParse(ccElement.GetString(), out var parsed))
                            cloudCover = parsed;
                    }
                }

                if (maxCloudCover.HasValue && cloudCover.HasValue && cloudCover > maxCloudCover)
                    continue;

                DateTime? acqDate = null;
                if (feature.Properties != null &&
                    feature.Properties.TryGetValue("datetime", out var dtValue))
                {
                    if (dtValue is JsonElement dtElement && dtElement.ValueKind == JsonValueKind.String)
                    {
                        if (DateTime.TryParse(dtElement.GetString(), out var parsed))
                            acqDate = parsed;
                    }
                }

                var assets = new Dictionary<string, StacAsset>();
                if (feature.Assets != null)
                {
                    foreach (var kvp in feature.Assets)
                    {
                        assets[kvp.Key] = new StacAsset
                        {
                            Href = kvp.Value?.Href,
                            Type = kvp.Value?.Type,
                            Roles = kvp.Value?.Roles,
                            Size = kvp.Value?.Size
                        };
                    }
                }

                scenes.Add(new StacSceneResult
                {
                    Id = feature.Id,
                    AcquisitionDate = acqDate,
                    CloudCover = cloudCover,
                    Bbox = feature.Bbox,
                    Assets = assets
                });
            }

            return scenes.OrderByDescending(s => s.AcquisitionDate).ToList();
        }
    }

    internal class StacSearchResponse
    {
        public string Type { get; set; }
        public List<StacFeature> Features { get; set; } = new();
    }

    internal class StacFeature
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public double[] Bbox { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public Dictionary<string, StacAssetDto> Assets { get; set; }
    }

    internal class StacAssetDto
    {
        public string Href { get; set; }
        public string Type { get; set; }
        public string[] Roles { get; set; }
        public long? Size { get; set; }
    }
}
