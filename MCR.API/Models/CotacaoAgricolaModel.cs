using MCR.API.Entities;

namespace MCR.API.Models
{
    public class CotacaoAgricolaModel
    {
        public CotacoesAgricolaEntity? Cotacao { get; set; }
        public List<CotacoesAgricolaPropostaEntity>? Proposta { get; set; }
        public List<CotacoesAgricolaStatusEntity>? Historico { get; set; }

        // Search and Filter Properties
        public string? PesquisaCodigoCotacao { get; set; }
        public string? PesquisaSafra { get; set; }
        public string? PesquisaCultura { get; set; }
        public string? PesquisaMunicipio { get; set; }
        public DateTime? PesquisaDataCotacao { get; set; }
        public string? PesquisaCpf { get; set; }
        public string? PesquisaStatus { get; set; }
        public int? PaginaAtual { get; set; }
        public int? TotalPaginas { get; set; }
        public IEnumerable<CotacoesAgricolaEntity>? ListaCotacoes { get; set; }

        // Navigation Properties
        public byte[]? ObjectImagem { get; set; }

        public CotacaoAgricolaModel()
        {
            Cotacao = new CotacoesAgricolaEntity();
            //Coberturas = new List<CotacoesAgricolaCoberturaEntity>();
            //Premios = new List<CotacoesAgricolaPremioEntity>();
            //Historico = new List<CotacoesAgricolaStatusEntity>();
            //ListaCotacoes = new List<CotacoesAgricolaEntity>();
        }
    }
}
