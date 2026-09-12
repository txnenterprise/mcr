using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class MonitoringAlertEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ExecutionId { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public bool Viewed { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}
