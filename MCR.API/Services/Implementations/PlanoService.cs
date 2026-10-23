using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    /// <summary>
    /// Planos são IMUTÁVEIS após criação: não há Atualizar/Excluir por design.
    /// </summary>
    public class PlanoService : IPlanoService
    {
        private readonly DbContextMCR _context;

        public PlanoService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlanoEntity>> ObterTodosAsync()
        {
            var retorno = await _context.Planos
                .AsNoTracking()
                .Where(p => !p.Excluido)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            retorno.ForEach(p => p.Sucesso = true);
            return retorno;
        }

        public async Task<PlanoEntity?> ObterPorIdAsync(Guid id)
        {
            return await _context.Planos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido);
        }

        public async Task<PlanoEntity> CadastrarAsync(PlanoEntity entity)
        {
            entity.Nome = entity.Nome?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(entity.Nome))
            {
                entity.Sucesso = false;
                entity.Mensagem = "O campo Nome é obrigatório.";
                return entity;
            }

            var nomeNormalizado = entity.Nome.ToLower();
            var nomeExiste = await _context.Planos.AnyAsync(p => p.Nome.ToLower() == nomeNormalizado);
            if (nomeExiste)
            {
                entity.Sucesso = false;
                entity.Mensagem = $"Já existe um plano com o nome \"{entity.Nome}\".";
                return entity;
            }

            entity.Id = Guid.NewGuid();
            entity.Ativo = true;
            entity.Excluido = false;

            _context.Planos.Add(entity);
            var retorno = await _context.SaveChangesAsync();

            entity.Sucesso = retorno > 0;
            entity.Mensagem = retorno > 0
                ? "Plano criado com sucesso! Planos não podem ser alterados após a criação."
                : "Não foi possível criar o plano. Tente novamente.";
            return entity;
        }
    }
}
