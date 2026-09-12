using System.ComponentModel.DataAnnotations;



namespace MCR.API.Entities;

public class PropostasBeneficiariosEntity
{

    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public Guid? ClienteId { get; set; }
    public Guid? BeneficiarioId { get; set; }
    public string? Nome { get; set; }
    public string? Documento { get; set; }
    public string? ImagemDocumento { get; set; }
    public string? Banco { get; set; }
    public string? Agencia { get; set; }
    public string? Conta { get; set; }
    public string? ChavePIX { get; set; }
    public decimal? Percentual { get; set; }
    public virtual PropostasEntity? Proposta { get; set; }

    public virtual ClienteEntity? Cliente { get; set; }
    public virtual BeneficiarioEntity? Beneficiario { get; set; }

    public virtual ICollection<PropostasQuestionarioBancosEntity>? Bancos { get; set; }
}
