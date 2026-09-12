using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface IProdutosService
    {
        Task<ProdutosEntity> ObterPorIdAsync(Guid id);
        Task<IEnumerable<ProdutosEntity>> ObterTodosPaginadoAsync(string descricao = null, Guid? seguradoraId = null, Guid? safraId = null, Guid? culturaId = null, bool? ativo = null, int page = 1, int pageSize = 20);
        Task<ProdutosEntity> CadastrarAsync(ProdutosEntity produto);
        Task<ProdutosEntity> AtualizarAsync(ProdutosEntity produto);
        Task<bool> ExcluirAsync(Guid id);
        Task<ProdutosEntity> DuplicarAsync(Guid id);
        Task<bool> ToggleAtivoAsync(Guid id);
    }
}
