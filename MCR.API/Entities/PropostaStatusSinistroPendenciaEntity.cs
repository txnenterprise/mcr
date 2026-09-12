namespace MCR.API.Entities;

// Status: Sinistro Com Pendência
public class PropostaStatusSinistroPendenciaEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string UploadArquivo { get; set; }
    public string UploadArquivoNome { get; set; }
    public string RetornoPendencia { get; set; }
    public string TipoDocumento { get; set; }
}

