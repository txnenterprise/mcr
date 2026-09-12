namespace MCR.API.Services.Interfaces
{
    public class StacSceneResult
    {
        public string Id { get; set; }
        public DateTime? AcquisitionDate { get; set; }
        public decimal? CloudCover { get; set; }
        public double[] Bbox { get; set; }
        public Dictionary<string, StacAsset> Assets { get; set; } = new();
    }

    public class StacAsset
    {
        public string Href { get; set; }
        public string Type { get; set; }
        public string[] Roles { get; set; }
        public long? Size { get; set; }
    }

    public interface ISatelliteProvider
    {
        Task<List<StacSceneResult>> SearchScenesAsync(
            double[] bbox,
            DateTime startDate,
            DateTime endDate,
            string collection,
            decimal? maxCloudCover = null,
            int limit = 20);
    }
}
