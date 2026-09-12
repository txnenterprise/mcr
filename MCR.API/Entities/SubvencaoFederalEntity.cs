using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCR.API.Entities
{
    public class SubvencaoFederalEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Cultura { get; set; }
        public decimal Porentagem { get; set; }
        public decimal LimiteReal { get; set; }
        public string CPF { get; set; }
        public string AnoCivil { get; set; }
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
    }
}