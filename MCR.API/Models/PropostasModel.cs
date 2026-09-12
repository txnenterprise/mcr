namespace MCR.API.Models
{
    public class PropostasModel
    {
        public PropostasModel()
        {

        }

        public PropostasModel(Guid propostaId)
        {
            PropostaId = propostaId;
        }

        public Guid PropostaId { get; set; }
    }
}
