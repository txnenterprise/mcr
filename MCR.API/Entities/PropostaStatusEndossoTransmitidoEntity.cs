namespace MCR.API.Entities;

// Status: Endosso Transmitido
public class PropostaStatusEndossoTransmitidoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string? NumeroEndosso { get; set; }
    public string UploadArquivo { get; set; } 
    public string UploadArquivoNome { get; set; } 
    public string? TipoDocumento { get; set; }
    public string? Observacoes { get; set; }
}

