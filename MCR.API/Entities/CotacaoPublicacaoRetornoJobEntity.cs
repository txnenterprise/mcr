using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class CotacaoPublicacaoRetornoJobEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public string JsonPublicacao { get; set; }
        [Required]
        public bool Publicado { get; set; }
        [Required]
        public string Fila { get; set; }
        [Required]
        public string Seguradora { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
    }
}