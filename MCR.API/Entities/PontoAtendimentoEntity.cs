using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;





namespace MCR.API.Entities
{
    public class PontoAtendimentoEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CanalId { get; set; }
        public CanalEntity Canal { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public bool Ativo { get; set; }
        public bool Excluido { get; set; }


        public string? EmailSeguro { get; set; }
        public string? EmailCopiaSeguro { get; set; }
        public string? EmailSinistro { get; set; }
        public string? EmailCopiaSinistro { get; set; }
        public string ImagemLogo { get; set; }
        [NotMapped]
        public string PontoAtendimentoNome { get; set; }
        [NotMapped]
        public int QuantidadeConsultoresAssociados { get; set; }
        [NotMapped]
        public int TotalItems { get; set; }
        [NotMapped]
        public int TotalPages { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
        public virtual ICollection<CotacoesAgricolaEntity> CotacoesAgricola { get; set; } = new List<CotacoesAgricolaEntity>();

        public virtual ICollection<ProdutosCanalPontoAtendimentoEntity> ProdutosCanalPontoAtendimento { get; set; } = new List<ProdutosCanalPontoAtendimentoEntity>();
        public virtual ICollection<PropostasEntity> Propostas { get; set; } = new List<PropostasEntity>();

        public virtual ICollection<UsuarioPontoAtendimentoEntity> UsuarioPontoAtendimento { get; set; } = new List<UsuarioPontoAtendimentoEntity>();

        public virtual ICollection<UsuarioEstruturaNegocioEntity> UsuarioEstruturaNegocio { get; set; } = new List<UsuarioEstruturaNegocioEntity>();
    }
}