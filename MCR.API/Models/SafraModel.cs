using MCR.API.Entities;

namespace MCR.API.Models
{
    public class SafraModel
    {
        public SafraEntity Safra { get; set; }
        public IList<SafraEntity> ListaSafras { get; set; }
    }
}
