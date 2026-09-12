using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCR.API.CotacoesAgricola.Domain.DTO;

namespace MCR.API.Entities;

public class CotacoesAgricolaEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid? ClienteId { get; set; }

    public string? ClienteCPF { get; set; }
    public string? ClienteNome { get; set; }
    public Guid CulturaId { get; set; }
    public Guid SafraId { get; set; }
    public string Estado { get; set; }
    public string Municipio { get; set; }
    public decimal AreaTotal { get; set; }

    public bool IsModalidadeProdutividade { get; set; }
    public decimal? PrecoSaca { get; set; }
    public decimal? ValorCusteio { get; set; }


    //public int TipoSolo { get; set; }
    //public string ClassificacaoSolo { get; set; }
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

    [NotMapped]
    public bool Sucesso { get; set; }
    [NotMapped]
    public string? Mensagem { get; set; }
    [NotMapped]
    public int TotalItems { get; set; }
    [NotMapped]
    public int TotalPages { get; set; }
    [NotMapped]
    public MotorCotacaoDiagnosticoDTO? MotorDiagnostico { get; set; }

    public Guid UsuarioId { get; set; }

    public DateTime? DataInsucesso { get; set; }


    public virtual UsuarioEntity Usuario { get; set; }
    public virtual ClienteEntity? Cliente { get; set; }
    public virtual CulturaEntity Cultura { get; set; }

    public virtual CanalEntity? Canal { get; set; }
    public virtual PontoAtendimentoEntity? PontoAtendimento { get; set; }
    public virtual CorretoraEntity? Corretora { get; set; }
    public virtual SafraEntity? Safra { get; set; }


    public virtual ICollection<CotacoesAgricolaPropostaEntity> CotacoesAgricolaProposta { get; set; } = new List<CotacoesAgricolaPropostaEntity>();
    public virtual ICollection<CotacoesAgricolaStatusEntity> CotacoesAgricolaStatus { get; set; } = new List<CotacoesAgricolaStatusEntity>();

    public virtual ICollection<CotacoesAgricolaTipoSoloEntity> CotacoesAgricolaTipoSolo { get; set; } = new List<CotacoesAgricolaTipoSoloEntity>();
    public virtual ICollection<CotacoesAgricolaClassificacaoSoloEntity> CotacoesAgricolaClassificacaoSolo { get; set; } = new List<CotacoesAgricolaClassificacaoSoloEntity>();


    public virtual ICollection<PropostasEntity> Propostas { get; set; } = new List<PropostasEntity>();

}
