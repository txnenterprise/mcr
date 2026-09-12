using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class BeneficiarioEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string ImagemCNPJ { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string ChavePIX { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }

        public virtual ICollection<PropostasBeneficiariosEntity> PropostasBeneficiarios { get; set; } = new List<PropostasBeneficiariosEntity>();
    }
}