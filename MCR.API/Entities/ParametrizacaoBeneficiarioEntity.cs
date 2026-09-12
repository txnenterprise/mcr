using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class ParametrizacaoBeneficiarioEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Banco { get; set; }
        [Required]
        public string Agencia { get; set; }
        [Required]
        public string Conta { get; set; }
        [Required]
        public string DigitoConta { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
}