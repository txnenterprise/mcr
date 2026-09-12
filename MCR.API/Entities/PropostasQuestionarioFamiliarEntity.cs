using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasQuestionarioFamiliarEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid QuestionarioId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid? VinculoFamiliarId { get; set; }
    public string? Nome { get; set; }
    public string? CPF { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Relacao { get; set; }

    public virtual ClienteEntity? Cliente { get; set; }
    public virtual VinculoFamiliarEntity? VinculoFamiliar { get; set; }
    public virtual PropostasQuestionarioEntity? Questionario { get; set; }
}
