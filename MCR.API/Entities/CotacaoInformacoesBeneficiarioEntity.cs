using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class CotacaoInformacoesBeneficiarioEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public string TipoPessoa { get; set; }
        [Required]
        public string NomeBeneficiario { get; set; }
        [Required]
        public string CPFCNPJBeneficiario { get; set; }
        public string BancoBeneficiario { get; set; }
        public string AgenciaBeneficiario { get; set; }
        public string ContaBeneficiario { get; set; }
        public string DigitoContaBeneficiario { get; set; }
    }
}