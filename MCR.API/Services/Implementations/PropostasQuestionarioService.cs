using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations;

public class PropostasQuestionarioService : IPropostasQuestionarioService
{
    private readonly DbContextMCR _context;

    public PropostasQuestionarioService(DbContextMCR context)
    {
        _context = context;
    }

    public async Task<object> ObterQuestionarioAsync(Guid propostaId)
    {
        var proposta = await _context.Propostas.FirstOrDefaultAsync(x => x.Id == propostaId);
        if (proposta == null)
            return new PropostasQuestionarioDTO { PropostaId = propostaId };

        var familiaresLista = new List<PropostasQuestionarioFamiliarDTO>();

        // 1. Titular (o próprio cliente)
        var clienteTitular = await _context.Clientes
            .Where(c => c.Id == proposta.ClienteId && !c.Excluido)
            .Select(c => new PropostasQuestionarioFamiliarDTO
            {
                ClienteId = c.Id,
                VinculoFamiliarId = null,
                Nome = c.Nome,
                CPF = c.CPF,
                Telefone = string.IsNullOrWhiteSpace(c.Celular) ? c.Telefone : c.Celular,
                Email = c.Email,
                Relacao = "Titular",
                Selecionado = false
            }).FirstOrDefaultAsync();

        if (clienteTitular != null)
            familiaresLista.Add(clienteTitular);

        // 2. Vínculos familiares
        var vinculos = await _context.VinculosFamiliares
            .Where(v => v.ClienteId == proposta.ClienteId && v.Ativo && !v.Excluido)
            .ToListAsync();

        foreach (var v in vinculos)
        {
            familiaresLista.Add(new PropostasQuestionarioFamiliarDTO
            {
                ClienteId = v.ClienteId,
                VinculoFamiliarId = v.Id,
                Nome = v.Nome,
                CPF = v.Cpf,
                Telefone = v.Telefone,
                Email = v.Email,
                Relacao = v.RelacaoParental,
                Selecionado = false
            });
        }

        // 3. Questionário salvo (para cruzar dados)
        var questionario = await _context.PropostasQuestionario
            .Include(x => x.Familiares)
            .Include(x => x.Bancos)
                .ThenInclude(b => b.Beneficiario)
            .FirstOrDefaultAsync(x => x.PropostaId == propostaId);

        // 4. Cruzar: marcar selecionados
        if (questionario?.Familiares != null)
        {
            foreach (var item in familiaresLista)
            {
                var correspondente = questionario.Familiares
                    .FirstOrDefault(x => x.ClienteId == item.ClienteId &&
                                        x.VinculoFamiliarId == item.VinculoFamiliarId &&
                                        x.CPF == item.CPF);
                item.QuestionarioFamiliarId = correspondente?.Id;
                item.Selecionado = correspondente != null;
            }
        }

        // 5. Bancos do cliente (Beneficiários)
        var bancosCliente = await _context.Beneficiarios
            .Where(b => !b.Excluido)
            .Select(b => new PropostasQuestionarioBancoDTO
            {
                Id = b.Id,
                BeneficiarioId = b.Id,
                IsCliente = false,
                Beneficiario = new PropostaBeneficiarioItemDTO
                {
                    Id = b.Id,
                    Nome = b.Nome,
                    CNPJ = b.CNPJ,
                    Banco = b.Banco,
                    Agencia = b.Agencia,
                    Conta = b.Conta
                }
            }).ToListAsync();

        // 6. Montar DTO
        var dto = new PropostasQuestionarioDTO
        {
            PropostaId = propostaId,
            Familiares = familiaresLista,
            Bancos = questionario?.Bancos?.Select(b => new PropostasQuestionarioBancoDTO
            {
                Id = b.Id,
                QuestionarioId = b.QuestionarioId,
                BeneficiarioId = b.BeneficiarioId,
                NumeroCreditoCedula = b.NumeroCreditoCedula,
                DataVencimento = b.DataVencimento,
                Selecionado = true,
                IsCliente = b.Beneficiario?.ClienteId != null,
                Beneficiario = b.Beneficiario != null ? new PropostaBeneficiarioItemDTO
                {
                    Id = b.Beneficiario.Id,
                    Nome = b.Beneficiario.Nome,
                    CNPJ = b.Beneficiario.Documento,
                    Banco = b.Beneficiario.Banco,
                    Agencia = b.Beneficiario.Agencia,
                    Conta = b.Beneficiario.Conta
                } : null
            }).ToList() ?? new()
        };

        if (questionario != null)
        {
            dto.Id = questionario.Id;
            dto.SistemaPlantio = questionario.SistemaPlantio;
            dto.LavouraPlantada = questionario.LavouraPlantada;
            dto.PossuiDanosPreExistentes = questionario.PossuiDanosPreExistentes;
            dto.ConheceZARC = questionario.ConheceZARC;
            dto.PossuiOutroSeguro = questionario.PossuiOutroSeguro;
            dto.LavouraImplantadaAposOutraArea = questionario.LavouraImplantadaAposOutraArea;
            dto.NotasFiscaisProprioSegurado = questionario.NotasFiscaisProprioSegurado;
            dto.CulturaAnteriorId = questionario.CulturaAnteriorId;
            dto.CulturaAnterior = questionario.CulturaAnterior?.Nome;
            dto.LavouraIrrigada = questionario.LavouraIrrigada;
            dto.PossuiCreditoBancario = questionario.PossuiCreditoBancario;
        }

        return dto;
    }

    public async Task<bool> SalvarQuestionarioAsync(Guid propostaId, object dados)
    {
        try
        {
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<PropostasQuestionarioCulturasDTO>> ListarCulturasAsync()
    {
        return await _context.Culturas
            .Select(x => new PropostasQuestionarioCulturasDTO
            {
                Id = x.Id,
                Text = x.Nome
            })
            .ToListAsync();
    }
}
