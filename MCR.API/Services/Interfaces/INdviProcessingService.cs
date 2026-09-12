using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public class NdviAnalysisResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public MonitoringAnalysisEntity Analysis { get; set; }
        public string PreviewImageBase64 { get; set; }
    }

    public interface INdviProcessingService
    {
        Task<NdviAnalysisResult> ProcessSceneAsync(Guid satelliteSceneId, Guid talhaoId, bool forceReprocess = false);
        Task<List<MonitoringAnalysisEntity>> ObterAnalisesAsync(Guid propertyId, Guid? talhaoId = null);
    }
}
