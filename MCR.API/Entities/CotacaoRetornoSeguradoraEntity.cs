using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class CotacaoRetornoSeguradoraEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        public string? NumeroCotacao { get; set; }
        [Required]
        public string Seguradora { get; set; }
        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime DataHoraRetorno { get; set; }
        public string FormaPagamento { get; set; }
        public string DiaPagamento { get; set; }
        public string NumeroParcelas { get; set; }
        [Required]
        public bool Efetivada { get; set; }
        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime DataHoraEfetivacao { get; set; }
        public string CorretorEfetivou { get; set; }
        public string Premio { get; set; }
        public string JsonRetornoPremioCobertura { get; set; }
        [NotMapped]
        public decimal PremioDecimal { get; set; }
        public string Parcelamento { get; set; }
        public string MensagemComplementar { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public IList<RelatorioDemonstrativoCotacaoDTO> JsonRetornoCoberturas { get; set; }
    }
}