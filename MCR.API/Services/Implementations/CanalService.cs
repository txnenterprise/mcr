using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class CanalService : ICanalService
    {
        private readonly DbContextMCR _context;

        public CanalService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<CanalEntity> ObterPorIdAsync(Guid id)
        {
            var retorno = await _context.Canais.FindAsync(id);
            if (retorno == null || retorno.Excluido)
            {
                return new CanalEntity
                {
                    Sucesso = false,
                    Mensagem = "Canal não encontrado ou excluído."
                };
            }
            retorno.Sucesso = true;
            return retorno;
        }

        public async Task<IEnumerable<CanalEntity>> ObterTodosPaginadoAsync(string nome = null, Guid? corretoraId = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Canais.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.RazaoSocial.Contains(nome));

            if (corretoraId.HasValue)
                query = query.Where(c => c.CorretoraId == corretoraId.Value);

            query = query.Where(c => c.Excluido == false);

            var totalItens = await query.AsNoTracking().CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)pageSize);

            var canaisPaginados = await query
                .AsNoTracking()
                .OrderByDescending(c => c.RazaoSocial)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            foreach (var item in canaisPaginados)
            {
                item.Sucesso = true;
                item.TotalItems = totalItens;
                item.TotalPages = totalPaginas;
            }

            return canaisPaginados;
        }

        public async Task<IEnumerable<CanalEntity>> ObterPorCorretoraAsync(Guid corretoraId)
        {
            var retorno = await _context.Canais.Where(c => c.CorretoraId == corretoraId && c.Excluido == false).ToListAsync();
            retorno.ForEach(c => c.Sucesso = true);
            return retorno;
        }

        public async Task<CanalEntity> CadastrarAsync(CanalEntity entity)
        {
            entity.Ativo = true;
            entity.Excluido = false;

            _context.Canais.Add(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Cadastrado com sucesso." : "Não foi possível cadastrar. Feche a tela e tente novamente.";
            return entity;
        }

        public async Task<CanalEntity> AtualizarAsync(CanalEntity entity)
        {
            _context.Canais.Update(entity);
            var retorno = await _context.SaveChangesAsync();
            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0 ? "Atualizado com sucesso." : "Não foi possível atualizar. Feche a tela e tente novamente.";
            return entity;
        }
    }
}
