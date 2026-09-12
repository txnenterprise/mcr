using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class EstruturaNegocioService : IEstruturaNegocioService
    {
        private readonly DbContextMCR _context;

        public EstruturaNegocioService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IList<CanalEntity>> PesquisarCanaisPontosAtendimento(string razaoSocial, string ativo, string razaoSocialPA,
                                                                                string ativoPA, int page, int pageSize)
        {
            IQueryable<CanalEntity> query = _context.Canais
                .Where(c => !c.Excluido)
                .Include(c => c.PontosAtendimento);

            if (!string.IsNullOrWhiteSpace(razaoSocial))
                query = query.Where(c => c.RazaoSocial.Contains(razaoSocial));

            if (ativo == "Sim")
                query = query.Where(c => c.Ativo);
            else if (ativo == "Não")
                query = query.Where(c => !c.Ativo);

            if (!string.IsNullOrWhiteSpace(razaoSocialPA) || !string.IsNullOrEmpty(ativoPA))
            {
                query = query.Where(c => c.PontosAtendimento.Any(pa =>
                    (string.IsNullOrWhiteSpace(razaoSocialPA) || pa.RazaoSocial.Contains(razaoSocialPA)) &&
                    (ativoPA == "Sim" && pa.Ativo || ativoPA == "Não" && !pa.Ativo || string.IsNullOrEmpty(ativoPA))));
            }

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
    }
}
