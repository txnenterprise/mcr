using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class CotacaoCondicaoComercialSeguradoraEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public string Seguradora { get; set; }
        [Required]
        public decimal Comissao { get; set; }
        [Required]
        public decimal DescontoAgravo { get; set; }
        [Required]
        public decimal MultiplicadorFranquia { get; set; }
        [NotMapped]
        public bool Suscesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
    
}