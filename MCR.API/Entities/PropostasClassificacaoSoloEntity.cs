using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities;

public class PropostasClassificacaoSoloEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public string ClassificacaoSolo { get; set; } = string.Empty;

    public virtual PropostasEntity? Proposta { get; set; }
}