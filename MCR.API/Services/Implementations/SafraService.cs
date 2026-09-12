using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class SafraService : ISafraService
    {
        private readonly DbContextMCR _context;

        public SafraService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<SafraEntity> ObterPorIdAsync(Guid id)
        {
            var entity = await _context.Safras
                .FirstOrDefaultAsync(c => c.Id == id && !c.Excluido);

            if (entity == null)
            {
                return new SafraEntity
                {
                    Sucesso = false,
                    Mensagem = "Cadastro não encontrado ou excluído."
                };
            }

            entity.Sucesso = true;
            return entity;
        }

        public async Task<IEnumerable<SafraEntity>> ObterTodosPaginadoAsync(string descricao = null, int? anoReferencia = null, bool? ativo = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Safras
                .Where(c => !c.Excluido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(c => c.Descricao.Contains(descricao));

            if (anoReferencia.HasValue)
                query = query.Where(c => c.AnoReferencia.Contains(anoReferencia.Value.ToString()));

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

        public async Task<SafraEntity> CadastrarAsync(SafraEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;

            if (string.IsNullOrEmpty(entity.AnoReferencia))
            {
                entity.Sucesso = false;
                entity.Mensagem = "Ano de Referência é obrigatório.";
                return entity;
            }

            var anoReferenciaExiste = await _context.Safras
                .Where(c => c.AnoReferencia == entity.AnoReferencia)
                .CountAsync();

            if (anoReferenciaExiste > 0)
            {
                entity.Sucesso = false;
                entity.Mensagem = $"O Ano de Referência {entity.AnoReferencia} já foi cadastrado.";
                return entity;
            }

            _context.Safras.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0
                ? "Cadastrado com sucesso."
                : "Não foi possível cadastrar. Feche a tela e tente novamente.";

            return entity;
        }

        public async Task<SafraEntity> AtualizarAsync(SafraEntity entity)
        {
            var anoReferenciaExiste = await _context.Safras
                .Where(c => c.AnoReferencia == entity.AnoReferencia)
                .FirstOrDefaultAsync();

            if (anoReferenciaExiste != null && !string.IsNullOrEmpty(anoReferenciaExiste.AnoReferencia) && anoReferenciaExiste.Id != entity.Id)
            {
                entity.Sucesso = false;
                entity.Mensagem = $"O Ano de Referência {entity.AnoReferencia} já foi cadastrado.";
                return entity;
            }

            _context.Safras.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0
                ? "Atualizado com sucesso."
                : "Não foi possível atualizar. Feche a tela e tente novamente.";

            return entity;
        }

        public async Task<bool> InativarAsync(Guid id)
        {
            var entity = await _context.Safras.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = false;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }

        public async Task<bool> AtivarAsync(Guid id)
        {
            var entity = await _context.Safras.FindAsync(id);
            if (entity == null)
                return false;

            entity.Ativo = true;
            _context.Entry(entity).State = EntityState.Modified;
            var retorno = await _context.SaveChangesAsync();
            return retorno > 0;
        }
    }
}
