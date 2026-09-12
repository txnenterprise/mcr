using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations;

public class PropostasFormaPagamentosService : IPropostasFormaPagamentosService
{
    private readonly DbContextMCR _context;

    public PropostasFormaPagamentosService(DbContextMCR context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PropostasFormaPagamentosEntity>> ObterPorPropostaIdAsync(Guid propostaId)
    {
        return await _context.PropostasFormaPagamentos
            .Where(x => x.PropostaId == propostaId)
            .ToListAsync();
    }
}
