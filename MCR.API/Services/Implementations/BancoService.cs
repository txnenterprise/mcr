using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class BancoService : IBancoService
    {
        private readonly DbContextMCR _context;

        public BancoService(DbContextMCR context)
        {
            _context = context;
        }

        public IList<BancoEntity> ObterTodos()
        {
            return _context.BancoEntity.ToList();
        }
    }
}
