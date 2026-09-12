namespace MCR.API.Entities;

// Status: Sinistro deferido Pago
public class PropostaStatusSinistroDeferidoPagoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string? Observacoes { get; set; }
}

