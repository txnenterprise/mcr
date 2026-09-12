using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class CotacaoInformacaoSeguradoEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid CotacaoId { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string TipoPessoa { get; set; }
        [Required]
        public string CPFCNPJ { get; set; }
        [Required]
        public string CEP { get; set; }
        [Required]
        public string Endereco { get; set; }
        [Required]
        public string Bairro { get; set; }
        [Required]
        public string Numero { get; set; }
        [Required]
        public string Estado { get; set; }
        [Required]
        public string Cidade { get; set; }
    }
}