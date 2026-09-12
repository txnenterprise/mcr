using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class CotacaoItemEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public Guid SeguradoraId { get; set; }
        [NotMapped]
        public string NomeSeguradora { get; set; }
        [NotMapped]
        public SeguradoraEntity Seguradora { get; set; }
        [NotMapped]
        public CotacaoEntity Cotacao { get; set; }
        [Required]
        public string CodigoCotacaoSeguradora { get; set; }
        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime DataHoraCotacao { get; set; }
        [Required]
        public bool Cancelado { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}