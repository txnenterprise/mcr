using MCR.API.Entities;

namespace MCR.API.Models
{
    public class ParametrizacaoSeguradoraModel
    {
        public ParametrizacaoSeguradoraEntity Seguradora { get; set; }
        public IList<ParametrizacaoSeguradoraEntity> ListaSeguradoras { get; set; }
        public ParametrizacaoMultiCalculoParceiroEntity ParametrizacaoMultiCalculoParceiro { get; set; }
    }
}