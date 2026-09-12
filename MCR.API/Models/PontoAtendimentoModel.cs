using MCR.API.Entities;

namespace MCR.API.Models
{
    public class PontoAtendimentoModel
    {
        public PontoAtendimentoEntity PontoAtendimento { get; set; }
        public IList<PontoAtendimentoEntity> ListaPontosAtendimento { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string ObjectImagem { get; set; }
        public string? PesquisaRazaoSocial { get; set; }
        public string? PesquisaAtivoInativo { get; set; }
        public string? PesquisaCanalRazaoSocial { get; set; }
    }
}