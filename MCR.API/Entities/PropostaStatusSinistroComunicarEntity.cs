namespace MCR.API.Entities;

// Status: Comunicar Sinistro
public class PropostaStatusSinistroComunicarEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string Cobertura { get; set; } // Replantio ou Produção
    public string Evento { get; set; } // Evento selecionado
    public string? SeveridadeDano { get; set; }
    public decimal? DanoEstimado { get; set; } // Percentual
    public decimal? AreaTotalAfetada { get; set; } // Hectares
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFinal { get; set; }
    public string? TipoRespVistoria { get; set; }
    public string? NomeRespVistoria { get; set; }
    public string? CPFRespVistoria { get; set; }
    public string? Observacao { get; set; }
}

