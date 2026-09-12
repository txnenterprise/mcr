namespace MCR.API.Entities;

// Status: Endosso com pendência
public class PropostaStatusEndossoPendenciaEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string UploadArquivo { get; set; } 
    public string UploadArquivoNome { get; set; } 
    public string DescricaoPendencia { get; set; }
    public string TipoDocumento { get; set; }
}

