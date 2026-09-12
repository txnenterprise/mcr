namespace MCR.API.Entities;

// Status: Solicitar Endosso
public class PropostaStatusEndossoSolicitacaoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string UploadArquivo { get; set; }
    public string UploadArquivoNome { get; set; } 
    public string? DescricaoAlteracao { get; set; }
    public string? RetornoPendencia { get; set; }
}

