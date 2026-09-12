using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;







namespace MCR.API.Entities;

public class PropostasEntity
{

    [Key]
    public Guid Id { get; set; }

    public Guid CotacaoAgricolaId { get; set; }

    public Guid ClienteId { get; set; }

    public Guid CulturaId { get; set; }
    public Guid SafraId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public decimal AreaTotal { get; set; }

    public bool IsModalidadeProdutividade { get; set; }
    public decimal? PrecoSaca { get; set; }
    public decimal? ValorCusteio { get; set; }

    public bool PlantioConsorciado { get; set; }
    public bool LavouraIrrigada { get; set; }
    public bool PlantioDireto { get; set; }
    public bool PosCana { get; set; }
    public decimal CustoProducao { get; set; }

    public bool SubvencaoFederal { get; set; }
    public bool SubvencaoEstadual { get; set; }

    public Guid? CorretoraId { get; set; }
    public Guid? CanalId { get; set; }
    public Guid? PontoAtendimentoId { get; set; }

    public string? CodigoCotacao { get; set; }
    public DateTime? DataCotacao { get; set; }
    public string? Status { get; set; }


    public bool Ativo { get; set; }
    public bool Excluido { get; set; }

    public Guid UsuarioId { get; set; }

    public DateTime? DataInsucesso { get; set; }

    public int CodigoProposta { get; set; }

    public virtual UsuarioEntity? Usuario { get; set; }
    public virtual ClienteEntity? Cliente { get; set; }
    public virtual CulturaEntity? Cultura { get; set; }

    public virtual CanalEntity? Canal { get; set; }
    public virtual PontoAtendimentoEntity? PontoAtendimento { get; set; }
    public virtual CorretoraEntity? Corretora { get; set; }
    public virtual SafraEntity? Safra { get; set; }

    public virtual CotacoesAgricolaEntity? CotacoesAgricola { get; set; }
    public virtual ICollection<PropostasProdutosEntity> PropostasProdutos { get; set; } = new List<PropostasProdutosEntity>();
    public virtual ICollection<PropostasStatusEntity> PropostasStatus { get; set; } = new List<PropostasStatusEntity>();

    public virtual ICollection<PropostasTipoSoloEntity> PropostasTipoSolo { get; set; } = new List<PropostasTipoSoloEntity>();
    public virtual ICollection<PropostasClassificacaoSoloEntity> PropostasClassificacaoSolo { get; set; } = new List<PropostasClassificacaoSoloEntity>();

    public virtual ICollection<PropostasSeguradosEntity> PropostasSegurados { get; set; } = new List<PropostasSeguradosEntity>();
    public virtual ICollection<PropostasClientePropriedadesEntity> PropostasClientePropriedades { get; set; } = new List<PropostasClientePropriedadesEntity>();

    public virtual ICollection<PropostasVistoriaEntity> PropostasVistoria { get; set; } = new List<PropostasVistoriaEntity>();
    public virtual ICollection<PropostasBeneficiariosEntity> PropostasBeneficiarios { get; set; } = new List<PropostasBeneficiariosEntity>();

    public virtual ICollection<PropostasQuestionarioEntity>? PropostasQuestionario { get; set; } = new List<PropostasQuestionarioEntity>();
    public virtual ICollection<PropostasObservacoesEntity> PropostasObservacoes { get; set; } = new List<PropostasObservacoesEntity>();

    public virtual ICollection<PropostasDocumentosEntity> PropostasDocumentos { get; set; } = new List<PropostasDocumentosEntity>();
    public virtual ICollection<PropostasFormaPagamentosEntity> PropostasFormaPagamentos { get; set; } = new List<PropostasFormaPagamentosEntity>();
}
