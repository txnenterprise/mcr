using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class CotacaoInformacaoBemEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public string TipoEquipamento { get; set; }
        [Required]
        public int AnoFabricacao { get; set; }
        [Required]
        public decimal ValorEquipamento { get; set; }
        [Required]
        public string MarcaEquipamento { get; set; }
        [Required]
        public string ModeloEquipamento { get; set; }
        [Required]
        public string NumeroSerieEquipamento { get; set; }
        [Required]
        public string NumeroChassiEquipamento { get; set; }
        public bool InformarNotaFiscal { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTime DataNotaFiscal { get; set; }
        public string NumeroNotaFiscal { get; set; }
        [NotMapped]
        public CotacaoFormularioRiscoEntity CotacaoFormularioRisco { get; set; }
        [NotMapped]
        public IList<CotacaoFormularioRiscoEntity> ListaCotacaoFormularioRisco { get; set; }
        [NotMapped]
        public CotacaoCoberturaEntity CotacaoCobertura { get; set; }
        [NotMapped]
        public IList<CotacaoCoberturaEntity> ListaCotacaoCobertura { get; set; }
    }
}