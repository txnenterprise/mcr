using MCR.API.Entities;

namespace MCR.API.Models
{
    public class TalhaoModel
    {
        public TalhaoEntity Talhao { get; set; } = new TalhaoEntity();
        public IList<TalhaoEntity> ListaTalhoes { get; set; }
        public IList<TalhaoArquivoModel> ListaKmlFiles { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string ImageJson { get; set; }
        public string KmlJson { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public string PesquisaPropriedadeId { get; set; }

        // Propriedades para o Razor
        public IList<TalhaoArquivoModel> ImagensJson { get; set; } = new List<TalhaoArquivoModel>();
        public IList<TalhaoArquivoModel> KmlsJson { get; set; } = new List<TalhaoArquivoModel>();
    }

    public class TalhaoArquivoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Arquivo { get; set; } // Base64 do conteúdo do arquivo
    }
}