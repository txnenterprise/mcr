using MCR.API.Entities;

namespace MCR.API.Models
{
    public class UsuarioPontoAtendimentoModel
    {
        public UsuarioPontoAtendimentoEntity UsuarioPontoAtendimento { get; set; }
        public IList<UsuarioPontoAtendimentoEntity> ListaUsuarioPontoAtendimento { get; set; }
        public UsuarioPontoAtendimentoDTO UsuarioPontoAtendimentoDTO { get; set; }
        public IList<UsuarioPontoAtendimentoDTO> ListaUsuarioPontoAtendimentoDTO { get; set; }
        public IList<UsuarioEntity> ListaUsuariosCadastrados { get; set; }
        public string jsonUsuarios { get; set; }
        public IEnumerable<PontoAtendimentoEntity> ListaPontosAtendimento { get; set; }
        public IEnumerable<CanalEntity> ListaCanais { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string PesquisaNomeUsuario { get; set; }
        public string PesquisaNomeLiderado { get; set; }
        public string PesquisaNomePontoAtendimento { get; set; }
    }

    public class VinculosPontoAtendimento
    {
        public string nome { get; set; }
        public string email { get; set; }
        public string canalNome { get; set; }
        public string canalId { get; set; }
        public string pontoAtendimentoNome { get; set; }
        public string pontoAtendimentoId { get; set; }
    }
}