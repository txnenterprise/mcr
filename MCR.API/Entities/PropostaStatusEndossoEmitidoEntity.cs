namespace MCR.API.Entities;

// Status: Endosso Emitido
public class PropostaStatusEndossoEmitidoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string NumeroEndosso { get; set; }
    public string UploadArquivo { get; set; } 
    public string UploadArquivoNome { get; set; }
    public string? TipoDocumento { get; set; }
    public string? LMITotal { get; set; }
    public string? PremioTotal { get; set; }
    public string? SubFederal { get; set; }
    public string? SubEstadual { get; set; }
    public decimal? ParcSegurado { get; set; }
    public decimal? AreaTotal { get; set; }
    public DateTime? InicioVigencia { get; set; }
    public DateTime? FimVigencia { get; set; }
    public string? Observacoes { get; set; }
}

