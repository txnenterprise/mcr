namespace MCR.API.Entities;

// Status: Sinistro Aguardando Pagamento
public class PropostaStatusSinistroAguardPagamentoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string UploadArquivo { get; set; }
    public string UploadArquivoNome { get; set; }
    public string? TipoDocumento { get; set; }
    public DateTime? DataDeferimento { get; set; }
    public decimal? ValorIndenizacao { get; set; }
    public string? Observacoes { get; set; }
}

