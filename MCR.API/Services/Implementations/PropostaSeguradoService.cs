using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations;

public class PropostaSeguradoService : IPropostaSeguradoService
{
    private readonly DbContextMCR _context;

    public PropostaSeguradoService(DbContextMCR context)
    {
        _context = context;
    }

    public async Task<bool> VincularSeguradoAsync(Guid propostaId, Guid clienteId)
    {
        try
        {
            var proposta = await _context.Propostas
                .FirstOrDefaultAsync(p => p.Id == propostaId && !p.Excluido);

            if (proposta == null)
                return false;

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == clienteId);

            if (cliente == null)
                return false;

            var cpf = cliente.CPF ?? string.Empty;

            var seguradoExistente = await _context.PropostasSegurados
                .FirstOrDefaultAsync(ps => ps.PropostaId == propostaId && ps.CPF == cpf);

            if (seguradoExistente != null)
            {
                seguradoExistente.ClienteId = clienteId;
                seguradoExistente.Nome = cliente.Nome;
                seguradoExistente.CPF = cpf;
                seguradoExistente.Telefone = cliente.Telefone;
                seguradoExistente.Email = cliente.Email;
                seguradoExistente.RelacaoParental = "Titular";
                _context.PropostasSegurados.Update(seguradoExistente);
            }
            else
            {
                var novaSegurado = new PropostasSeguradosEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = propostaId,
                    ClienteId = clienteId,
                    Nome = cliente.Nome,
                    CPF = cpf,
                    Telefone = cliente.Telefone,
                    Email = cliente.Email,
                    RelacaoParental = "Titular"
                };

                await _context.PropostasSegurados.AddAsync(novaSegurado);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoverSeguradoAsync(Guid id)
    {
        try
        {
            var segurado = await _context.PropostasSegurados
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (segurado == null)
                return false;

            _context.PropostasSegurados.Remove(segurado);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<PropostasSeguradosDTO>> ObterPropostaSeguradosPorId(Guid propostaId)
    {
        return await _context.PropostasSegurados
            .Where(ps => ps.PropostaId == propostaId)
            .Select(ps => new PropostasSeguradosDTO
            {
                Id = ps.Id,
                PropostaId = ps.PropostaId,
                ClienteId = ps.ClienteId,
                Nome = ps.Nome,
                CPF = ps.CPF,
                Telefone = ps.Telefone,
                Email = ps.Email,
                RelacaoParental = ps.RelacaoParental
            })
            .ToListAsync();
    }
}
