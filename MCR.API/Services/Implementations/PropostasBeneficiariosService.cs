using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Services.Implementations;

public class PropostasBeneficiariosService : IPropostasBeneficiariosService
{
    private readonly DbContextMCR _context;

    public PropostasBeneficiariosService(DbContextMCR context)
    {
        _context = context;
    }

    public async Task<PropostasBeneficiariosDTO> ObterBeneficiariosPorPropostaAsync(Guid propostaId)
    {
        var dto = new PropostasBeneficiariosDTO();

        var proposta = await _context.Propostas.FirstOrDefaultAsync(x => x.Id == propostaId);
        if (proposta == null) return dto;

        var vinculados = await _context.PropostasBeneficiarios
            .Where(pb => pb.PropostaId == propostaId)
            .ToListAsync();

        // 1. Cliente titular
        var clientes = await _context.Clientes
            .Where(x => x.Id == proposta.ClienteId)
            .Select(c => new PropostaBeneficiarioItemDTO
            {
                ClienteId = c.Id,
                Nome = c.Nome,
                Documento = c.CPF,
                Banco = c.Banco,
                Agencia = c.Agencia,
                Conta = c.Conta,
                Percentual = 0
            }).ToListAsync();

        // 2. Beneficiarios cadastrados
        var beneficiarios = await _context.Beneficiarios
            .Where(x => !x.Excluido)
            .Select(b => new PropostaBeneficiarioItemDTO
            {
                BeneficiarioId = b.Id,
                Nome = b.Nome,
                Documento = b.CNPJ,
                Banco = b.Banco,
                Agencia = b.Agencia,
                Conta = b.Conta,
                Percentual = 0
            }).ToListAsync();

        // 3. Cruzar percentuais já vinculados
        foreach (var pessoa in clientes.Concat(beneficiarios))
        {
            var vinculado = vinculados.FirstOrDefault(v =>
                (pessoa.ClienteId.HasValue && v.ClienteId == pessoa.ClienteId) ||
                (pessoa.BeneficiarioId.HasValue && v.BeneficiarioId == pessoa.BeneficiarioId));
            if (vinculado != null)
                pessoa.Percentual = vinculado.Percentual;
        }

        dto.PessoasBeneficiarios.AddRange(clientes.Concat(beneficiarios));
        return dto;
    }

    public async Task<IEnumerable<PropostasBeneficiariosEntity>> ObterPorPropostaAsync(Guid propostaId)
    {
        return await _context.PropostasBeneficiarios
            .Where(pb => pb.PropostaId == propostaId)
            .ToListAsync();
    }

    public async Task<(bool Sucesso, string Mensagem)> SalvarBeneficiariosAsync(PropostasBeneficiariosDTO request)
    {
        try
        {
            var selecionados = request.PessoasBeneficiarios.Where(p => p.Selecionado).ToList();

            var vinculadosExistentes = await _context.PropostasBeneficiarios
                .Where(pb => pb.PropostaId == request.PropostaId)
                .ToListAsync();

            foreach (var sel in selecionados)
            {
                var existente = vinculadosExistentes
                    .FirstOrDefault(e => (sel.ClienteId.HasValue && e.ClienteId == sel.ClienteId) ||
                                         (sel.BeneficiarioId.HasValue && e.BeneficiarioId == sel.BeneficiarioId));

                if (existente != null)
                {
                    existente.Percentual = sel.Percentual;
                    _context.PropostasBeneficiarios.Update(existente);
                }
                else
                {
                    var cliente = sel.ClienteId.HasValue
                        ? await _context.Clientes.FirstOrDefaultAsync(c => c.Id == sel.ClienteId.Value)
                        : null;

                    var beneficiario = sel.BeneficiarioId.HasValue
                        ? await _context.Beneficiarios.FirstOrDefaultAsync(b => b.Id == sel.BeneficiarioId.Value)
                        : null;

                    _context.PropostasBeneficiarios.Add(new PropostasBeneficiariosEntity
                    {
                        Id = Guid.NewGuid(),
                        PropostaId = request.PropostaId,
                        ClienteId = sel.ClienteId,
                        BeneficiarioId = sel.BeneficiarioId,
                        Nome = sel.Nome ?? cliente?.Nome ?? beneficiario?.Nome,
                        Documento = sel.Documento ?? cliente?.CPF ?? beneficiario?.CNPJ,
                        Percentual = sel.Percentual,
                        Banco = sel.Banco ?? cliente?.Banco ?? beneficiario?.Banco,
                        Agencia = sel.Agencia ?? cliente?.Agencia ?? beneficiario?.Agencia,
                        Conta = sel.Conta ?? cliente?.Conta ?? beneficiario?.Conta
                    });
                }
            }

            var paraRemover = vinculadosExistentes
                .Where(e => !selecionados.Any(s =>
                    (s.ClienteId.HasValue && e.ClienteId == s.ClienteId) ||
                    (s.BeneficiarioId.HasValue && e.BeneficiarioId == s.BeneficiarioId)))
                .ToList();

            _context.PropostasBeneficiarios.RemoveRange(paraRemover);

            await _context.SaveChangesAsync();
            return (true, "Beneficiários salvos com sucesso!");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao salvar: {ex.Message}");
        }
    }

    public async Task<bool> RemoverBeneficiarioAsync(Guid id)
    {
        try
        {
            var beneficiario = await _context.PropostasBeneficiarios
                .FirstOrDefaultAsync(b => b.Id == id);

            if (beneficiario == null)
                return false;

            _context.PropostasBeneficiarios.Remove(beneficiario);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
