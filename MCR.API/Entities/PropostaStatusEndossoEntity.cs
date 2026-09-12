namespace MCR.API.Entities;

// Status: Solicitar Endosso / Endosso com pendência
public class PropostaStatusEndossoEntity : PropostasStatusEntity
{
    public string Acao { get; set; } // Solicitar Endosso / Endosso com pendência
    public string UploadArquivo { get; set; } // URL do upload
    public string UploadArquivoNome { get; set; } // Nome do arquivo
    public string? RetornoPendencia { get; set; } // Retorno da pendência (se Endosso com pendência)
    public string? TipoDocumento { get; set; } // Tipo de documento (se Endosso com pendência)
}

