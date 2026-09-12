namespace MCR.API.Entities;

// Status: Sinistro Cancelado
public class PropostaStatusSinistroCanceladoEntity : PropostasStatusEntity
{
    public string Acao { get; set; }
    public string MotivoCancelamento { get; set; }
}

