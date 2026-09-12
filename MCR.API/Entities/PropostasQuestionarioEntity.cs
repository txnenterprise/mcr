using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasQuestionarioEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }

    public string? SistemaPlantio { get; set; }  // Direto ou Convencional
    public bool LavouraPlantada { get; set; }
    public bool? PossuiDanosPreExistentes { get; set; }
    public bool ConheceZARC { get; set; }
    public bool PossuiOutroSeguro { get; set; }
    public bool LavouraImplantadaAposOutraArea { get; set; }
    public bool NotasFiscaisProprioSegurado { get; set; }

    public Guid? CulturaAnteriorId { get; set; }
    public bool LavouraIrrigada { get; set; }
    public bool PossuiCreditoBancario { get; set; }

    // Relacionamentos
    public virtual PropostasEntity? Proposta { get; set; }

    public virtual CulturaEntity? CulturaAnterior { get; set; }
    public virtual ICollection<PropostasQuestionarioFamiliarEntity>? Familiares { get; set; }
    public virtual ICollection<PropostasQuestionarioBancosEntity>? Bancos { get; set; }
}
