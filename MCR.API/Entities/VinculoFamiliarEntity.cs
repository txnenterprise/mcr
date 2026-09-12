using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class VinculoFamiliarEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string RelacaoParental { get; set; }

        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        // Relação com Cliente
        public Guid ClienteId { get; set; }
        public ClienteEntity Cliente { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }

        public virtual ICollection<PropostasSeguradosEntity> PropostasSegurados { get; set; } = new List<PropostasSeguradosEntity>();
        public virtual ICollection<PropostasQuestionarioFamiliarEntity>? PropostasQuestionarioFamiliar { get; set; } = new List<PropostasQuestionarioFamiliarEntity>();
    }
}