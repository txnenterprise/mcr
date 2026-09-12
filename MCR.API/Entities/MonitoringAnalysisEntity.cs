using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class MonitoringAnalysisEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid TalhaoId { get; set; }
        public Guid SatelliteSceneId { get; set; }
        public string SceneId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal AverageNdvi { get; set; }
        public decimal MinimumNdvi { get; set; }
        public decimal MaximumNdvi { get; set; }
        public int ValidPixelCount { get; set; }
        public decimal HealthyAreaPercent { get; set; }
        public decimal CriticalAreaPercent { get; set; }
        public string Classification { get; set; }
        public string PreviewImageBase64 { get; set; }
        public string JsonStatistics { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}
