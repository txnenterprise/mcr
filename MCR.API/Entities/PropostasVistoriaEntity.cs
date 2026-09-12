using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities;

public class PropostasVistoriaEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public Guid? PropostasSeguradoId { get; set; }
    public string? Nome { get; set; }
    public string? CPF { get; set; }
    public string? Telefone { get; set; }
    public virtual PropostasEntity? Proposta { get; set; }
    public virtual PropostasSeguradosEntity? PropostasSegurados { get; set; }
}
