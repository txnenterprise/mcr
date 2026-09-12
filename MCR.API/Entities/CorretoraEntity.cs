using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class CorretoraEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string RazaoSocial { get; set; } = string.Empty;
        [Required]
        public string NomeFantasia { get; set; } = string.Empty;
        [Required]
        public string CNPJ { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public bool Ativa { get; set; }
        public bool EmAtraso { get; set; }
        public string PlanoContratado { get; set; } = string.Empty;
        public string ImagemLogo { get; set; } = string.Empty;
        public int DiaVencimento { get; set; }
        public string EmailSeguro { get; set; } = string.Empty;
        public string EmailCopiaSeguro { get; set; } = string.Empty;
        public string EmailSinistro { get; set; } = string.Empty;
        public string EmailCopiaSinistro { get; set; } = string.Empty;
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; } = string.Empty;
    }
}
