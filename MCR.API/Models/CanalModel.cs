using MCR.API.Entities;

namespace MCR.API.Models
{
    public class CanalModel
    {
        public CanalEntity Canal { get; set; } = new CanalEntity();
        public IList<CanalEntity> ListaCanais { get; set; } = new List<CanalEntity>();
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string ObjectImagem { get; set; }
        public string PesquisaRazaoSocial { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public string PesquisaRazaoSocialPA { get; set; }
        public string PesquisaAtivoInativoPA { get; set; }
    }
}