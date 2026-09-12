using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    [Table("CulturasMaturacao")]
    public class CulturaMaturacaoEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string GrupoMaturacao { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        public Guid CulturaId { get; set; }
        [ForeignKey("CulturaId")]
        public virtual CulturaEntity Cultura { get; set; }

        public virtual ICollection<CulturaMaturacaoVariedadeEntity> Variedades { get; set; } = new List<CulturaMaturacaoVariedadeEntity>();
        public virtual ICollection<PropostasClientePropriedadesTalhoesEntity> PropostasClientePropriedadesTalhoes { get; set; } = new List<PropostasClientePropriedadesTalhoesEntity>();
    }
}
