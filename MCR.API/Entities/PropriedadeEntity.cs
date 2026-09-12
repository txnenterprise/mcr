using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class PropriedadeEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public decimal SomaAreaTotalTalhao { get; set; }
        public string ImagemGeralTodosTalhoes { get; set; }
        public string MatriculaLote { get; set; }
        public string CadastroAmbientalRural { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }
        [NotMapped]
        public Guid ClienteId { get; set; }
        [NotMapped]
        public ClienteEntity Cliente { get; set; }
        [NotMapped]
        public IList<TalhaoEntity> Talhoes { get; set; } = new List<TalhaoEntity>();
        [NotMapped]
        public int QuantidadeTalhoesAssociados { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }


        public virtual ICollection<VinculoPropriedadeClienteEntity> VinculoPropriedadeCliente { get; set; } = new List<VinculoPropriedadeClienteEntity>();
        public virtual ICollection<PropostasClientePropriedadesEntity> PropostasClientePropriedades { get; set; } = new List<PropostasClientePropriedadesEntity>();
    }
}