using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class ProdutosCanalService : IProdutosCanalService
    {
        private readonly DbContextMCR _context;

        public ProdutosCanalService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<bool> AdicionarAsync(Guid produtoId, Guid pontoAtendimentoId)
        {
            try
            {
                var pontoAtendimento = await _context.PontosAtendimento
                    .FirstOrDefaultAsync(p => p.Id == pontoAtendimentoId);

                if (pontoAtendimento == null)
                    return false;

                var entity = new ProdutosCanalPontoAtendimentoEntity
                {
                    ProdutoId = produtoId,
                    CanalId = pontoAtendimento.CanalId,
                    PontoAtendimentoId = pontoAtendimentoId
                };

                _context.ProdutosCanalPontosAtendimento.Add(entity);
                var retorno = await _context.SaveChangesAsync();
                return retorno > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            try
            {
                var entity = await _context.ProdutosCanalPontosAtendimento.FindAsync(id);
                if (entity == null)
                    return false;

                _context.ProdutosCanalPontosAtendimento.Remove(entity);
                var retorno = await _context.SaveChangesAsync();
                return retorno > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
