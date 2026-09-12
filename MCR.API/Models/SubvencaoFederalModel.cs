using MCR.API.Entities;

namespace MCR.API.Models
{
    public class SubvencaoFederalModel
    {
        public SubvencaoFederalEntity SubvencaoFederal { get; set; }
        public IList<SubvencaoFederalEntity> ListaSubvencoesFederais { get; set; }
    }
}
