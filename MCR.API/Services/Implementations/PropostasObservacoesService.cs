using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations;

public class PropostasObservacoesService : IPropostasObservacoesService
{
    private readonly DbContextMCR _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PropostasObservacoesService(DbContextMCR context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<PropostasObservacoesEntity>> ObterPorPropostaAsync(Guid propostaId)
    {
        return await _context.PropostasObservacoes
            .Include(x => x.Usuario)
            .Where(x => x.PropostaId == propostaId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync();
    }

    public async Task<bool> SalvarObservacaoAsync(Guid propostaId, string observacao, Guid usuarioId)
    {
        try
        {
            var entity = new PropostasObservacoesEntity
            {
                Id = Guid.NewGuid(),
                PropostaId = propostaId,
                UsuarioId = usuarioId,
                DataCriacao = DateTime.UtcNow,
                Observacao = observacao
            };

            _context.PropostasObservacoes.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
