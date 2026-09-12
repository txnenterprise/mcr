namespace MCR.API.Entities;

// Status: Sinistro indeferido
public class PropostaStatusSinistroIndeferidoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string UploadArquivo { get; set; } 
    public string UploadArquivoNome { get; set; }
    public string? TipoDocumento { get; set; }
    public DateTime? DataIndeferimento { get; set; }
    public string? Observacoes { get; set; }
}

