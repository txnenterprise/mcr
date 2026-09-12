using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities;

public class CotacoesAgricolaTipoSoloEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid CotacaoAgricolaId { get; set; }
    public int TipoSolo { get; set; }

    public virtual CotacoesAgricolaEntity CotacoesAgricola { get; set; }
}
