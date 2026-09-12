namespace MCR.API.Entities;

// Status: Cotação encerrada sem sucesso, Proposta cancelada, Proposta recusada, Apólice cancelada
public class PropostaStatusJustificativaEntity : PropostasStatusEntity
{
    public string Justificativa { get; set; }
}
