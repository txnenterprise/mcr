using MCR.API.Entities;
using MCR.API.DTOs;

namespace MCR.API.Services.Interfaces
{
    public interface ICotacoesAgricolaService
    {
        Task<CotacoesAgricolaEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<CotacoesAgricolaEntity>> ObterTodosPaginadoAsync(string codigoCotacao = null, Guid? safraId = null, Guid? culturaId = null, string municipio = null, DateTime? dataCotacao = null, string cpfCliente = null, int page = 1, int pageSize = 20);
        Task<CotacoesAgricolaEntity> CadastrarAsync(CotacoesAgricolaEntity cotacao);
        Task<CotacoesAgricolaEntity> AtualizarAsync(CotacoesAgricolaEntity cotacao);
        Task<bool> ExcluirAsync(Guid id);
        Task<bool> RegistrarInsucessoAsync(Guid id);
        Task<bool> ReabrirAsync(Guid id);
        Task<byte[]> GerarPdfAsync(Guid id);
        Task<byte[]> DownloadPdfAsync(Guid id);
        Task<MCR.API.CotacoesAgricola.Domain.DTO.CotacaoAgricolaPdfDTO?> ObterCotacaoPdf(Guid cotacaoId);
        Task<IEnumerable<object>> ObterTiposSoloPorCulturaSafraAsync(Guid culturaId, Guid safraId);
        Task<IEnumerable<string>> ObterClassificacoesSoloPorCulturaSafraAsync(Guid culturaId, Guid safraId);
        Task<CotacoesAgricola.Domain.DTO.CotacaoAgricolaDadosPropostasDTO> ObterDadosPropostaCotacao(Guid cotacaoId, string order = "segurada");
        Task<List<string>> ObterAcoesPorStatus(string status, string role);
        Task VerificarCotacoesEmAberto();
        Task<List<MCR.API.CotacoesAgricola.Domain.DTO.ProdutoDisponivelDTO>> ObterProdutosDisponiveisAsync(Guid? culturaId, Guid? safraId, Guid? canalId, Guid? pontoAtendimentoId, string? estado = null, string? municipio = null, decimal areaTotal = 0);
    }
}
