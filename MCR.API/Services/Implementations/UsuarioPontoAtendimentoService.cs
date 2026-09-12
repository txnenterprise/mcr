using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;

namespace MCR.API.Services.Implementations
{
    public interface IUsuarioPontoAtendimentoService
    {
        Task<(List<UsuarioPontoAtendimentoEntity> Lista, int TotalItems, int TotalPages)> ObterUsuarioPontoAtendimento(
            string pesquisaNomeUsuario, string pesquisaNomeLiderado, string pesquisaNomePontoAtendimento,
            string pesquisaNomeCanal, int page, int pageSize);
        Task<UsuarioPontoAtendimentoEntity> ObterPorIdAsync(Guid id);
        Task<List<UsuarioPontoAtendimentoEntity>> AdicionarUsuarioPontoAtendimentoAsync(List<UsuarioPontoAtendimentoEntity> vinculos);
        Task<UsuarioPontoAtendimentoEntity> RemoverUsuarioPontoAtendimentoAsync(Guid id);
    }

    public class UsuarioPontoAtendimentoService : IUsuarioPontoAtendimentoService
    {
        private readonly DbContextMCR _context;

        public UsuarioPontoAtendimentoService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<(List<UsuarioPontoAtendimentoEntity> Lista, int TotalItems, int TotalPages)> ObterUsuarioPontoAtendimento(
            string pesquisaNomeUsuario, string pesquisaNomeLiderado, string pesquisaNomePontoAtendimento,
            string pesquisaNomeCanal, int page, int pageSize)
        {
            var query = _context.UsuariosPontoAtendimento
                .Include(u => u.Usuario)
                .Include(u => u.Liderado)
                .Include(u => u.PontoAtendimento)
                .Include(u => u.Canal)
                .Where(u => !u.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisaNomeUsuario))
                query = query.Where(u => u.Usuario.Name.Contains(pesquisaNomeUsuario));
            if (!string.IsNullOrEmpty(pesquisaNomeLiderado))
                query = query.Where(u => u.Liderado.Name.Contains(pesquisaNomeLiderado));
            if (!string.IsNullOrEmpty(pesquisaNomePontoAtendimento))
                query = query.Where(u => u.PontoAtendimento.NomeFantasia.Contains(pesquisaNomePontoAtendimento));
            if (!string.IsNullOrEmpty(pesquisaNomeCanal))
                query = query.Where(u => u.Canal.NomeFantasia.Contains(pesquisaNomeCanal));

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var lista = await query.OrderBy(u => u.Usuario.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (lista, totalItems, totalPages);
        }

        public async Task<UsuarioPontoAtendimentoEntity> ObterPorIdAsync(Guid id)
        {
            return await _context.UsuariosPontoAtendimento
                .Include(u => u.Usuario)
                .Include(u => u.Liderado)
                .Include(u => u.PontoAtendimento)
                .Include(u => u.Canal)
                .FirstOrDefaultAsync(u => u.Id == id && !u.Excluido);
        }

        public async Task<List<UsuarioPontoAtendimentoEntity>> AdicionarUsuarioPontoAtendimentoAsync(List<UsuarioPontoAtendimentoEntity> vinculos)
        {
            foreach (var v in vinculos)
            {
                v.Id = Guid.NewGuid();
                v.Ativo = true;
                _context.UsuariosPontoAtendimento.Add(v);
            }
            await _context.SaveChangesAsync();
            return vinculos;
        }

        public async Task<UsuarioPontoAtendimentoEntity> RemoverUsuarioPontoAtendimentoAsync(Guid id)
        {
            var entity = await _context.UsuariosPontoAtendimento.FindAsync(id);
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
