namespace MCR.API.Entities;

// Status: Proposta com pendência
public class PropostaStatusPendenciaEntity : PropostasStatusEntity
{
    public string UploadArquivo { get; set; } // URL do upload
    public string UploadArquivoNome { get; set; } // Nome
    public string RetornoPendencia { get; set; }
    public string TipoDocumento { get; set; }
}
