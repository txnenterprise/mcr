using MCR.API.Entities;

namespace MCR.API.Models
{
    public class LoginSeguradoraModel
    {
        public IList<LoginSeguradoraEntity> ListaLoginSeguradoras { get; set; }
        public IList<SeguradoraEntity> ListaSeguradoras { get; set; }
        public string CorretoraVinculada { get; set; }
        public Guid CorretoraId { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}