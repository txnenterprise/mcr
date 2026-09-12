using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities;

public class PropostasTipoSoloEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public int TipoSolo { get; set; }
    public virtual PropostasEntity? Proposta { get; set; }
}
