using MCR.API.Entities;

namespace MCR.API.Models
{
    public class SeguradoraModel
    {
        public SeguradoraEntity Seguradora { get; set; }
        public IList<SeguradoraEntity> ListaSeguradoras { get; set; }
        public string ObjectImagem { get; set; }
    }
}