namespace MCR.API.Entities;

// Status: Proposta aceita
public class PropostaStatusAceitaEntity : PropostasStatusEntity
{
    public string PropostaSeguradora { get; set; } // URL do upload
    public string PropostaSeguradoraNome { get; set; }
    public string NumeroProposta { get; set; }
    public string LmiTotal { get; set; }
    public string PremioTotal { get; set; }
    public string SubFederal { get; set; }
    public string SubEstadual { get; set; }
    public decimal ParcelaSegurado { get; set; }
    public string? Boleto { get; set; } // URL do boleto
    public string? BoletoNome { get; set; }
    public DateTime? DataVencimento { get; set; }
}
