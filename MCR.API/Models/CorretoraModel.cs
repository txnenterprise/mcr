using MCR.API.Entities;

namespace MCR.API.Models
{
    public class CorretoraModel
    {
        public CorretoraEntity Corretora { get; set; }
        public IList<CorretoraEntity> ListaCorretoras { get; set; }
    }
}