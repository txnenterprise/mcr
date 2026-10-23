using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface IPlanoService
    {
        Task<IEnumerable<PlanoEntity>> ObterTodosAsync();
        Task<PlanoEntity?> ObterPorIdAsync(Guid id);
        Task<PlanoEntity> CadastrarAsync(PlanoEntity entity);
    }
}
