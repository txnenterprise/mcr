using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace MCR.API.Entities
{
    public class SafraEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string AnoReferencia { get; set; }
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

        public virtual ICollection<CotacoesAgricolaEntity> CotacoesAgricola { get; set; } = new List<CotacoesAgricolaEntity>();
        public virtual ICollection<PropostasEntity> Propostas { get; set; } = new List<PropostasEntity>();
    }
}