using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations;

public class PropostaRiscoService : IPropostaRiscoService
{
    private readonly DbContextMCR _context;

    public PropostaRiscoService(DbContextMCR context)
    {
        _context = context;
    }

    public async Task<bool> VincularRiscosAsync(Guid propostaId, List<Guid> riscoIds)
    {
        try
        {
            if (!riscoIds.Any())
                return false;

            var propriedades = await _context.Propriedades
                .Where(x => riscoIds.Contains(x.Id))
                .ToListAsync();

            if (!propriedades.Any())
                return false;

            var proposta = await _context.Propostas
                .FirstOrDefaultAsync(p => p.Id == propostaId && !p.Excluido);

            if (proposta == null)
                return false;

            var createRiscos = propriedades.Select(x => new PropostasClientePropriedadesEntity
            {
                Id = Guid.NewGuid(),
                ClienteId = proposta.ClienteId,
                PropostaId = propostaId,
                PropriedadeId = x.Id,
                Nome = x.Nome,
                Endereco = x.Endereco,
                CEP = x.CEP,
                Cidade = x.Cidade,
                Bairro = x.Bairro,
                Estado = x.Estado,
                CadastroAmbientalRural = x.CadastroAmbientalRural,
                ImagemGeralTodosTalhoes = x.ImagemGeralTodosTalhoes,
                MatriculaLote = x.MatriculaLote,
                Numero = x.Numero,
                SomaAreaTotalTalhao = x.SomaAreaTotalTalhao
            }).ToList();

            _context.PropostasClientePropriedades.AddRange(createRiscos);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoverRiscoAsync(Guid id)
    {
        try
        {
            var risco = await _context.PropostasClientePropriedades
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (risco == null)
                return false;

            if (_context.PropostasClientePropriedadesTalhoes.Any(a => a.PropostasClientePropriedadeId == id))
            {
                var talhoes = await _context.PropostasClientePropriedadesTalhoes
                    .Where(a => a.PropostasClientePropriedadeId == id)
                    .ToListAsync();
                _context.PropostasClientePropriedadesTalhoes.RemoveRange(talhoes);
            }

            _context.PropostasClientePropriedades.Remove(risco);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
