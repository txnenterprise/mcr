using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities;

public class CotacoesAgricolaClassificacaoSoloEntity
{
    [Key]
    public Guid Id { get; set; }
    public Guid CotacaoAgricolaId { get; set; }
    public string ClassificacaoSolo { get; set; }

    public virtual CotacoesAgricolaEntity CotacoesAgricola { get; set; }
}