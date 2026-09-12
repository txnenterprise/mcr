using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class TalhaoEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PropriedadeId { get; set; }
        [NotMapped]
        public PropriedadeEntity Propriedade { get; set; }
        public string Nome { get; set; }
        public decimal Area { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string RoteiroAcesso { get; set; }
        public bool PossuiAnaliseFisicaSolo { get; set; }
        public decimal PercentualAreia { get; set; }
        public decimal PercentualSilte { get; set; }
        public decimal PercentualArgila { get; set; }
        public string TipoSolo { get; set; }
        public string ClassificacaoSolo { get; set; }
        public string ImagemTalhao { get; set; }
        public string KmlTalhao { get; set; }
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

        public virtual ICollection<PropostasClientePropriedadesTalhoesEntity> PropostasClientePropriedadesTalhoes { get; set; } = new List<PropostasClientePropriedadesTalhoesEntity>();
    }
}