using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;

namespace MCR.API.Services.Implementations
{
    public interface IUsuarioCanalService
    {
        Task<(List<UsuarioCanalEntity> Lista, int TotalItems, int TotalPages)> ObterUsuariosCanal(
            string pesquisaNomeUsuario, string pesquisaNomeLiderado, string pesquisaNomeCanal, int page, int pageSize);
        Task<UsuarioCanalEntity> ObterPorIdAsync(Guid id);
        Task<List<UsuarioCanalEntity>> AdicionarUsuarioCanalAsync(List<UsuarioCanalEntity> vinculos);
        Task<UsuarioCanalEntity> RemoverUsuarioCanalAsync(Guid id);
    }

    public class UsuarioCanalService : IUsuarioCanalService
    {
        private readonly DbContextMCR _context;

        public UsuarioCanalService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<(List<UsuarioCanalEntity> Lista, int TotalItems, int TotalPages)> ObterUsuariosCanal(
            string pesquisaNomeUsuario, string pesquisaNomeLiderado, string pesquisaNomeCanal, int page, int pageSize)
        {
            var query = _context.UsuariosCanal
                .Include(u => u.Usuario)
                .Include(u => u.UsuarioLiderado)
                .Include(u => u.Canal)
                .Where(u => !u.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisaNomeUsuario))
                query = query.Where(u => u.Usuario.Name.Contains(pesquisaNomeUsuario));
            if (!string.IsNullOrEmpty(pesquisaNomeLiderado))
                query = query.Where(u => u.UsuarioLiderado.Name.Contains(pesquisaNomeLiderado));
            if (!string.IsNullOrEmpty(pesquisaNomeCanal))
                query = query.Where(u => u.Canal.NomeFantasia.Contains(pesquisaNomeCanal));

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var lista = await query.OrderBy(u => u.Usuario.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (lista, totalItems, totalPages);
        }

        public async Task<UsuarioCanalEntity> ObterPorIdAsync(Guid id)
        {
            return await _context.UsuariosCanal
                .Include(u => u.Usuario)
                .Include(u => u.UsuarioLiderado)
                .Include(u => u.Canal)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Excluido);
        }

        public async Task<List<UsuarioCanalEntity>> AdicionarUsuarioCanalAsync(List<UsuarioCanalEntity> vinculos)
        {
            foreach (var v in vinculos)
            {
                v.Id = Guid.NewGuid();
                v.Ativo = true;
                _context.UsuariosCanal.Add(v);
            }
            await _context.SaveChangesAsync();
            return vinculos;
        }

        public async Task<UsuarioCanalEntity> RemoverUsuarioCanalAsync(Guid id)
        {
            var entity = await _context.UsuariosCanal.FindAsync(id);
            if (entity != null)
            {
                entity.Excluido = true;
                entity.Sucesso = true;
                await _context.SaveChangesAsync();
            }
            return entity;
        }
    }
}
