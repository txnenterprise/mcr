using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface ICotacaoService
    {
        Task<CotacaoEntity> ObterPorIdAsync(Guid id);
        Task<CotacaoEntity> ObterPorNumeroCotacaoAsync(string numeroCotacao);
        Task<IEnumerable<CotacaoEntity>> ObterTodosAsync(string corretor = null, string seguradora = null, DateTime? dataInicio = null, DateTime? dataFim = null, bool? efetivada = null, int page = 1, int pageSize = 20);
        Task<CotacaoEntity> CadastrarAsync(CotacaoEntity cotacao);
        Task<CotacaoEntity?> AtualizarAsync(CotacaoEntity cotacao);
        Task<bool> EfetivarAsync(Guid id);
        Task<bool> DesfazerEfetivacaoAsync(Guid id);
        Task<IEnumerable<CotacaoRetornoSeguradoraEntity>> ObterRetornosPorCotacaoIdAsync(Guid cotacaoId);
        Task<byte[]?> GerarRelatorioPdfAsync(Guid id);
        Task<int> ObterTotalItensAsync();
    }
}
