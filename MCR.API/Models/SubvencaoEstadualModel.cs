using MCR.API.Entities;

namespace MCR.API.Models
{
    public class SubvencaoEstadualModel
    {
        public SubvencaoEstadualEntity SubvencaoEstadual { get; set; }
        public IList<SubvencaoEstadualEntity> ListaSubvencoesEstaduais { get; set; }
    }
}
