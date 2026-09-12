using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities;

public class PropostasQuestionarioBancosEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid QuestionarioId { get; set; }
    public Guid BeneficiarioId { get; set; }
    public string? NumeroCreditoCedula { get; set; }
    public DateTime? DataVencimento { get; set; }

    public virtual PropostasQuestionarioEntity? Questionario { get; set; }
    public virtual PropostasBeneficiariosEntity? Beneficiario { get; set; }
}
