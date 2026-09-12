using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class CotacaoInformacaoSeguroEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [NotMapped]
        public CotacaoEntity Cotacao { get; set; }
        [Required]
        public string TempoVigenciaSeguro { get; set; }
        [Required]
        public string TipoSeguro { get; set; }
        public string ApoliceRenovacao { get; set; }
        public string SeguradoraAnterior { get; set; }
        public bool BemFinanciado { get; set; }
        public string BancoBeneficiarioInformacaoSeguro { get; set; }
        public string FormaPagamentoSeguro { get; set; }//Débito em conta / Boleto / Cartão crédito [Parametrização: para em 4x o parcelamento]
        [Required]
        public int QuantidadeParcelas { get; set; }
        public string PrazoSeguro { get; set; }//Anual / Pró-rata / Plurianual
    }
}