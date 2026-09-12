using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    [Table("CulturasMaturacaoVariedades")]
    public class CulturaMaturacaoVariedadeEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        public Guid CulturaMaturacaoId { get; set; }
        [ForeignKey("CulturaMaturacaoId")]
        public virtual CulturaMaturacaoEntity CulturaMaturacao { get; set; }

        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }
        public virtual ICollection<PropostasClientePropriedadesTalhoesEntity> PropostasClientePropriedadesTalhoes { get; set; } = new List<PropostasClientePropriedadesTalhoesEntity>();
    }
}
