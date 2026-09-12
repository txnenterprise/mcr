namespace MCR.API.Entities;

// Status: Apólice emitida
public class PropostaStatusApoliceEmitidaEntity : PropostasStatusEntity
{
    public string ApoliceEmitida { get; set; } // URL do upload
    public string ApoliceEmitidaNome { get; set; }
    public string NumeroApolice { get; set; }
    public DateTime InicioVigencia { get; set; }
    public DateTime FinalVigencia { get; set; }
    public string? Boleto { get; set; } // URL do boleto
    public string? BoletoNome { get; set; }
    public DateTime? DataVencimento { get; set; }
}