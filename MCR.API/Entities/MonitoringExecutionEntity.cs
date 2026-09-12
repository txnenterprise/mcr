using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class MonitoringExecutionEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string Status { get; set; }
        public string Satellite { get; set; }
        public decimal? CloudCover { get; set; }
        public decimal? AverageVegetationIndex { get; set; }
        public decimal? AffectedArea { get; set; }
        public string Message { get; set; }
        public string JsonStatistics { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}
