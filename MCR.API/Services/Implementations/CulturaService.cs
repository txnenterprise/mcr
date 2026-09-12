using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class CulturaService : ICulturaService
    {
        private readonly DbContextMCR _context;

        public CulturaService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<CulturaEntity> ObterPorIdAsync(Guid id)
        {
            var entity = await _context.Culturas
                .FirstOrDefaultAsync(c => c.Id == id && !c.Excluido);

            if (entity == null)
            {
                return new CulturaEntity
                {
                    Sucesso = false,
                    Mensagem = "Cadastro não encontrado ou excluído."
                };
            }

            entity.Sucesso = true;
            return entity;
        }

        public async Task<IEnumerable<CulturaEntity>> ObterTodosPaginadoAsync(string cultura = null, string grupoMaturacao = null, string variedade = null, bool? ativo = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Culturas
                .Where(c => !c.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cultura))
                query = query.Where(c => c.Nome.Contains(cultura));

            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var entity in results)
            {
                entity.TotalItems = totalItems;
                entity.TotalPages = totalPages;
                entity.Sucesso = true;
                entity.Mensagem = "Dados carregados com sucesso!";
            }

            return results;
        }

        public async Task<CulturaEntity> CadastrarAsync(CulturaEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;

            _context.Culturas.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0
                ? "Cadastrado com sucesso."
                : "Não foi possível cadastrar. Feche a tela e tente novamente.";

            return entity;
        }

        public async Task<CulturaEntity> AtualizarAsync(CulturaEntity entity)
        {
            _context.Culturas.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0
                ? "Atualizado com sucesso."
                : "Não foi possível atualizar. Feche a tela e tente novamente.";

            return entity;
        }

        public async Task<bool> InativarAsync(Guid id)
        {
            var entity = await _context.Culturas.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = false;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }

        public async Task<bool> AtivarAsync(Guid id)
        {
            var entity = await _context.Culturas.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = true;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }
    }
}
