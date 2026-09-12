using MCR.API.Entities;

namespace MCR.API.Models
{
    public class CulturaModel
    {
        public CulturaEntity Cultura { get; set; }
        public IList<CulturaEntity> ListaCulturas { get; set; }
    }
}
