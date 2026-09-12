using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class MonitoringConfigurationEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public bool Enabled { get; set; }
        public string UpdateFrequency { get; set; }
        public decimal? CloudLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}
