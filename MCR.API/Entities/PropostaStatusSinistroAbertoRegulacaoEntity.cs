namespace MCR.API.Entities;

// Status: Sinistro Aberto/Em Regulação
public class PropostaStatusSinistroAbertoRegulacaoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string UploadArquivo { get; set; }
    public string UploadArquivoNome { get; set; } 
    public string? TipoDocumento { get; set; }
    public string? ProtocoloAvisoSinistro { get; set; }
    public DateTime? DataAvisoSinistro { get; set; }
    public string? EmpresaPerito { get; set; }
    public string? Telefone { get; set; }
    public string? Observacoes { get; set; }
}

