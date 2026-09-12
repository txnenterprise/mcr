using MCR.API.Entities;

namespace MCR.API.Models
{
    public class PropriedadeModel
    {
        public PropriedadeEntity Propriedade { get; set; }
        public IList<PropriedadeEntity> ListaPropriedades { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string SelectedItemsJson { get; set; }
        public string ObjectImagem { get; set; }
        public string PesquisaNome { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public string PesquisaCidade { get; set; }
        public string PesquisaEstado { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}