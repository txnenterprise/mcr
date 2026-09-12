using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface IBancoService
    {
        IList<BancoEntity> ObterTodos();
    }
}
