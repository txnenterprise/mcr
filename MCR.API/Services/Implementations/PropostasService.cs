using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations;

public class PropostasService : IPropostasService
{
    private readonly ILogger<PropostasService> _logger;
    private readonly DbContextMCR _context;
    private readonly IHttpContextAccessor _contextAccessor;

    public PropostasService(
        ILogger<PropostasService> logger,
        DbContextMCR context,
        IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _context = context;
        _contextAccessor = contextAccessor;
    }

    public async Task<PropostasEntity> ObterPorIdAsync(Guid id)
    {
        var proposta = await _context.Propostas
            .Include(p => p.Cliente)
            .Include(p => p.Cultura)
            .Include(p => p.Safra)
            .Include(p => p.Corretora)
            .Include(p => p.Canal)
            .Include(p => p.PontoAtendimento)
            .Include(p => p.Usuario)
            .Include(p => p.PropostasProdutos)
            .Include(p => p.PropostasTipoSolo)
            .Include(p => p.PropostasClassificacaoSolo)
            .Include(p => p.PropostasSegurados)
            .Include(p => p.PropostasClientePropriedades)
            .Include(p => p.PropostasVistoria)
            .Include(p => p.PropostasBeneficiarios)
            .Include(p => p.PropostasFormaPagamentos)
            .Include(p => p.PropostasDocumentos)
            .Include(p => p.PropostasObservacoes)
            .FirstOrDefaultAsync(p => p.Id == id && !p.Excluido);

        return proposta;
    }

    public async Task<IEnumerable<PropostasEntity>> ObterTodosPaginadoAsync(ObterPropostasPaginadoRequestDTO request)
    {
        int pagina = request.Page;
        int tamanho = request.PageSize;

        var query = _context.Propostas
            .Include(p => p.Cliente)
            .Include(p => p.Cultura)
            .Include(p => p.PontoAtendimento)
            .Include(p => p.PropostasProdutos)
                .ThenInclude(pp => pp.Seguradora)
            .Include(p => p.PropostasSegurados)
            .Include(p => p.PropostasStatus)
            .Where(p => !p.Excluido && p.Ativo).AsQueryable();

        if (request.PontoAtendimentoId.HasValue)
            query = query.Where(p => p.PontoAtendimentoId == request.PontoAtendimentoId.Value);

        if (request.CanalId.HasValue)
            query = query.Where(p => p.CanalId == request.CanalId.Value);

        if (request.CorretoraId.HasValue)
            query = query.Where(p => p.CorretoraId == request.CorretoraId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(p => p.Status == request.Status);

        if (request.DataInicio.HasValue)
            query = query.Where(p => p.DataCotacao >= request.DataInicio.Value);

        if (request.DataFim.HasValue)
            query = query.Where(p => p.DataCotacao <= request.DataFim.Value);

        if (!string.IsNullOrWhiteSpace(request.Codigo))
            query = query.Where(p => p.CodigoCotacao != null && p.CodigoCotacao.Contains(request.Codigo));

        if (request.StatusFilter != null && request.StatusFilter.Any())
            query = query.Where(p => request.StatusFilter.Contains(p.Status));

        query = query.OrderByDescending(p => p.CodigoProposta);

        return await query
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();
    }

    public async Task<PropostasEntity> CadastrarAsync(PropostasEntity proposta)
    {
        proposta.Id = Guid.NewGuid();
        proposta.DataCotacao = DateTime.UtcNow;
        proposta.Status = "Proposta em negociação";
        proposta.Ativo = true;
        proposta.Excluido = false;

        _context.Propostas.Add(proposta);
        await _context.SaveChangesAsync();

        return proposta;
    }

    public async Task<(bool Sucesso, string Mensagem, Guid PropostaId)> GerarPropostaAsync(Guid cotacaoAgricolaId, Guid? usuarioId = null)
    {
        try
        {
            if (_context.PropostasProdutos.Any(x => x.CotacoesAgricolaPropostaId == cotacaoAgricolaId && !x.Proposta!.Excluido))
                return (false, "Já existe uma proposta ativa para esta opção.", Guid.Empty);

            var cotacaoProposta = await _context.CotacoesAgricolaProposta
                .Include(x => x.Produto)
                .Include(x => x.Seguradora)
                .Include(x => x.CotacoesAgricola)
                    .ThenInclude(ca => ca.CotacoesAgricolaClassificacaoSolo)
                .Include(x => x.CotacoesAgricola)
                    .ThenInclude(ca => ca.CotacoesAgricolaTipoSolo)
                .FirstOrDefaultAsync(x => x.Id == cotacaoAgricolaId && !x.Excluido);

            if (cotacaoProposta == null)
                return (false, "Proposta de cotação agrícola não encontrada ou excluída.", Guid.Empty);

            var cotacaoAgricola = cotacaoProposta.CotacoesAgricola;
            if (cotacaoAgricola == null)
                return (false, "Cotação agrícola não encontrada.", Guid.Empty);

            if (!cotacaoAgricola.ClienteId.HasValue)
                return (false, "Cotação não possui cliente vinculado. Selecione um cliente antes de gerar a proposta.", Guid.Empty);

            var proposta = new PropostasEntity
            {
                Id = Guid.NewGuid(),
                CotacaoAgricolaId = cotacaoAgricola.Id,
                ClienteId = cotacaoAgricola.ClienteId.Value,
                CulturaId = cotacaoAgricola.CulturaId,
                SafraId = cotacaoAgricola.SafraId,
                Estado = cotacaoAgricola.Estado,
                Municipio = cotacaoAgricola.Municipio,
                AreaTotal = cotacaoAgricola.AreaTotal,
                IsModalidadeProdutividade = cotacaoAgricola.IsModalidadeProdutividade,
                PrecoSaca = cotacaoAgricola.PrecoSaca,
                ValorCusteio = cotacaoAgricola.ValorCusteio,
                PlantioConsorciado = cotacaoAgricola.PlantioConsorciado,
                LavouraIrrigada = cotacaoAgricola.LavouraIrrigada,
                PlantioDireto = cotacaoAgricola.PlantioDireto,
                PosCana = cotacaoAgricola.PosCana,
                CustoProducao = cotacaoAgricola.CustoProducao,
                SubvencaoFederal = cotacaoAgricola.SubvencaoFederal,
                SubvencaoEstadual = cotacaoAgricola.SubvencaoEstadual,
                CorretoraId = cotacaoAgricola.CorretoraId,
                CanalId = cotacaoAgricola.CanalId,
                PontoAtendimentoId = cotacaoAgricola.PontoAtendimentoId,
                CodigoCotacao = cotacaoAgricola.CodigoCotacao,
                DataCotacao = DateTime.UtcNow,
                Status = "Proposta em negociação",
                Ativo = true,
                Excluido = false,
                UsuarioId = usuarioId ?? cotacaoAgricola.UsuarioId
            };

            proposta.PropostasFormaPagamentos.Add(new PropostasFormaPagamentosEntity
            {
                PropostaId = proposta.Id,
                Id = Guid.NewGuid(),
                ProdutoId = cotacaoProposta.ProdutoId,
                FormaDePagamento = cotacaoProposta.Produto.FormaDePagamento,
                Parcelamento = cotacaoProposta.Produto.Parcelamento,
            });

            var produtoProposta = new PropostasProdutosEntity
            {
                Id = Guid.NewGuid(),
                PropostaId = proposta.Id,
                CotacoesAgricolaPropostaId = cotacaoProposta.Id,
                Opcao = cotacaoProposta.Opcao,
                SeguradoraId = cotacaoProposta.SeguradoraId,
                ProdutoId = cotacaoProposta.ProdutoId,
                RegulacaoSinistro = cotacaoProposta.RegulacaoSinistro,
                ProdutividadeEsperada = cotacaoProposta.ProdutividadeEsperada,
                NivelCobertura = cotacaoProposta.NivelCobertura,
                ProdutividadeSegurada = cotacaoProposta.ProdutividadeSegurada,
                LMIProducaoHectare = cotacaoProposta.LMIProducaoHectare,
                LMIReplantioHectare = cotacaoProposta.LMIReplantioHectare,
                LMIProducaoTotal = cotacaoProposta.LMIProducaoTotal,
                LMIReplantioTotal = cotacaoProposta.LMIReplantioTotal,
                PremioTotal = cotacaoProposta.PremioTotal,
                SubvencaoFederal = cotacaoProposta.SubvencaoFederal,
                SubvencaoEstadual = cotacaoProposta.SubvencaoEstadual,
                ParcelaSegurado = cotacaoProposta.ParcelaSegurado,
                CustoHectare = cotacaoProposta.CustoHectare,
                CustoScHectare = cotacaoProposta.CustoScHectare,
                CustoScTotal = cotacaoProposta.CustoScTotal
            };

            var statusProposta = new PropostasStatusEntity
            {
                Id = Guid.NewGuid(),
                PropostaId = proposta.Id,
                Status = "Proposta em negociação",
                DataStatus = DateTime.UtcNow,
            };

            var tiposSoloProposta = cotacaoAgricola.CotacoesAgricolaTipoSolo?
                .Select(ts => new PropostasTipoSoloEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = proposta.Id,
                    TipoSolo = ts.TipoSolo
                }).ToList() ?? new List<PropostasTipoSoloEntity>();

            var classificacoesSoloProposta = cotacaoAgricola.CotacoesAgricolaClassificacaoSolo?
                .Select(cs => new PropostasClassificacaoSoloEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = proposta.Id,
                    ClassificacaoSolo = cs.ClassificacaoSolo
                }).ToList() ?? new List<PropostasClassificacaoSoloEntity>();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Propostas.AddAsync(proposta);
                    await _context.PropostasProdutos.AddAsync(produtoProposta);
                    await _context.PropostasStatus.AddAsync(statusProposta);
                    if (tiposSoloProposta.Any())
                        await _context.PropostasTipoSolo.AddRangeAsync(tiposSoloProposta);
                    if (classificacoesSoloProposta.Any())
                        await _context.PropostasClassificacaoSolo.AddRangeAsync(classificacoesSoloProposta);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return (true, "Proposta criada com sucesso.", proposta.Id);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Erro ao salvar proposta: {ex.Message}", Guid.Empty);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar proposta a partir da cotação {CotacaoAgricolaId}", cotacaoAgricolaId);
            return (false, $"Erro ao criar proposta: {ex.Message}", Guid.Empty);
        }
    }

    public async Task<bool> EnviarTransmissaoAsync(Guid propostaId)
    {
        try
        {
            var propostaValidate = await _context.Propostas
                .Include(t => t.PropostasSegurados)
                .Include(t => t.PropostasClientePropriedades)
                .Include(t => t.PropostasVistoria)
                .Include(t => t.PropostasBeneficiarios)
                .Include(t => t.PropostasFormaPagamentos)
                .Where(x => x.Id == propostaId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (propostaValidate == null)
                return false;

            var proposta = await _context.Propostas.Where(x => x.Id == propostaId).FirstOrDefaultAsync();
            if (proposta == null)
                return false;

            var usuarioId = _contextAccessor.GetUserId();
            if (!usuarioId.HasValue)
                return false;

            var perfilUsuario = _contextAccessor.GetRole();
            var lastStatus = _context.PropostasStatus
                .Where(x => x.PropostaId == propostaId)
                .OrderByDescending(o => o.DataStatus)
                .FirstOrDefault();

            proposta.Status = "Aguardando transmissão";
            _context.PropostasStatus.Add(new PropostasStatusEntity
            {
                PropostaId = proposta.Id,
                DataStatus = DateTime.UtcNow,
                Status = proposta.Status,
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId.Value,
                StatusAnterior = lastStatus?.Status,
                UsuarioPerfil = perfilUsuario
            });

            _context.Entry(proposta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar transmissão da proposta {PropostaId}", propostaId);
            return false;
        }
    }

    public async Task<PropostaValidacaoDTO> ObterStatusValidacaoAsync(Guid propostaId)
    {
        var dto = new PropostaValidacaoDTO
        {
            Valida = true,
            Erros = new List<string>(),
            Status = string.Empty
        };

        if (propostaId == Guid.Empty)
        {
            _logger.LogWarning("ObterStatusValidacao: propostaId is Guid.Empty - hidden field not rendered correctly");
            dto.Valida = false;
            dto.Erros.Add("ID da proposta não informado na página");
            return dto;
        }

        var proposta = await _context.Propostas
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == propostaId);

        if (proposta == null)
        {
            var existsRaw = await _context.Database.ExecuteSqlRawAsync(
                $"SELECT count(*) FROM \"Propostas\" WHERE \"Id\" = '{propostaId}'");
            _logger.LogWarning("ObterStatusValidacao: proposta {Id} não encontrada via EF. ExistsRaw={Exists}. DbSet count={Count}",
                propostaId, existsRaw, await _context.Propostas.AsNoTracking().CountAsync());
            dto.Valida = false;
            dto.Erros.Add($"Proposta não encontrada (id={propostaId})");
            return dto;
        }

        dto.Status = proposta.Status ?? string.Empty;

        var segurados = await _context.PropostasSegurados.Where(s => s.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var riscos = await _context.PropostasClientePropriedades.Where(r => r.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var vistoria = await _context.PropostasVistoria.Where(v => v.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var beneficiarios = await _context.PropostasBeneficiarios.Where(b => b.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var formasPagamento = await _context.PropostasFormaPagamentos.Where(f => f.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var observacoes = await _context.PropostasObservacoes.Where(o => o.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var questionario = await _context.PropostasQuestionario.Where(q => q.PropostaId == propostaId).AsNoTracking().ToListAsync();
        var documentos = await _context.PropostasDocumentos.Where(d => d.PropostaId == propostaId).AsNoTracking().ToListAsync();

        dto.SeguradosPreenchido = segurados.Any();
        dto.RiscosPreenchido = riscos.Any();
        dto.VistoriaPreenchido = vistoria.Any();
        dto.BeneficiariosPreenchido = beneficiarios.Any();
        dto.FormasPagamentoPreenchido = formasPagamento.Any();
        dto.ObservacoesPreenchido = observacoes.Any();
        dto.QuestionariosPreenchido = questionario.Any();
        dto.DocumentosPreenchido = documentos.Any();

        return dto;
    }

    public async Task<byte[]> BaixarKmlTalhaoAsync(Guid talhaoId)
    {
        _logger.LogWarning("Download de KML para talhão {TalhaoId} não implementado, retornando null", talhaoId);
        return null;
    }

    public async Task<string> ObterStatusProposta(Guid propostaId)
    {
        var proposta = await _context.Propostas
            .Where(p => p.Id == propostaId)
            .Select(p => p.Status)
            .FirstOrDefaultAsync();
        return proposta ?? string.Empty;
    }

    public async Task<(bool Sucesso, string Mensagem, MCR.API.Propostas.Domain.DTO.PropostaDetalhesDTO? Proposta)> ObterPropostaDetalhes(Guid propostaId)
    {
        try
        {
            var proposta = await _context.Propostas
                .Include(p => p.Cliente)
                .Include(p => p.Cultura)
                .Include(p => p.Safra)
                .Include(p => p.Corretora)
                .Include(p => p.Canal)
                .Include(p => p.PontoAtendimento)
                .Include(p => p.Usuario)
                .Include(p => p.PropostasProdutos)
                    .ThenInclude(p => p.Seguradora)
                .Include(p => p.PropostasProdutos)
                    .ThenInclude(p => p.Produto)
                .Include(p => p.PropostasTipoSolo)
                .Include(p => p.PropostasClassificacaoSolo)
                .Include(p => p.PropostasSegurados)
                .FirstOrDefaultAsync(p => p.Id == propostaId && !p.Excluido);

            if (proposta == null)
                return (false, "Proposta não encontrada ou excluída", null);

            var dto = new MCR.API.Propostas.Domain.DTO.PropostaDetalhesDTO
            {
                Id = proposta.Id,
                CotacaoAgricolaId = proposta.CotacaoAgricolaId,
                CodigoCotacao = proposta.CodigoCotacao,
                ClienteId = proposta.ClienteId,
                ClienteName = proposta.Cliente?.Nome,
                ClienteCpf = proposta.Cliente?.CPF,
                CulturaId = proposta.CulturaId,
                CulturaName = proposta.Cultura?.Nome,
                SafraId = proposta.SafraId,
                SafraName = proposta.Safra?.AnoReferencia,
                Estado = proposta.Estado,
                Municipio = proposta.Municipio,
                AreaTotal = proposta.AreaTotal,
                IsModalidadeProdutividade = proposta.IsModalidadeProdutividade,
                PrecoSaca = proposta.PrecoSaca,
                ValorCusteio = proposta.ValorCusteio,
                PlantioConsorciado = proposta.PlantioConsorciado,
                LavouraIrrigada = proposta.LavouraIrrigada,
                PlantioDireto = proposta.PlantioDireto,
                PosCana = proposta.PosCana,
                CustoProducao = proposta.CustoProducao,
                SubvencaoFederal = proposta.SubvencaoFederal,
                SubvencaoEstadual = proposta.SubvencaoEstadual,
                CorretoraId = proposta.CorretoraId,
                CorretoraName = proposta.Corretora?.RazaoSocial,
                CanalId = proposta.CanalId,
                CanalName = proposta.Canal?.RazaoSocial,
                PontoAtendimentoId = proposta.PontoAtendimentoId,
                PontoAtendimentoName = proposta.PontoAtendimento?.RazaoSocial,
                DataCotacao = proposta.DataCotacao,
                Status = proposta.Status,
                UsuarioId = proposta.UsuarioId,
                UsuarioName = proposta.Usuario?.UserName,
                UsuarioTelefone = proposta.Usuario?.PhoneNumber,
                UsuarioEmail = proposta.Usuario?.Email
            };

            foreach (var produto in proposta.PropostasProdutos)
            {
                dto.Produtos.Add(new MCR.API.Propostas.Domain.DTO.PropostaDetalheProdutoDTO
                {
                    Id = produto.Id,
                    PropostaId = produto.PropostaId,
                    Opcao = produto.Opcao,
                    SeguradoraId = produto.SeguradoraId,
                    SeguradoraName = produto.Seguradora?.RazaoSocial,
                    ProdutoId = produto.ProdutoId,
                    ProdutoName = produto.Produto?.NomeProduto,
                    RegulacaoSinistro = produto.RegulacaoSinistro,
                    ProdutividadeEsperada = produto.ProdutividadeEsperada.ToString(),
                    NivelCobertura = produto.NivelCobertura,
                    ProdutividadeSegurada = produto.ProdutividadeSegurada,
                    LMIProducaoHectare = produto.LMIProducaoHectare,
                    LMIReplantioHectare = produto.LMIReplantioHectare,
                    LMIProducaoTotal = produto.LMIProducaoTotal,
                    LMIReplantioTotal = produto.LMIReplantioTotal,
                    PremioTotal = produto.PremioTotal,
                    SubvencaoFederal = produto.SubvencaoFederal,
                    SubvencaoEstadual = produto.SubvencaoEstadual,
                    ParcelaSegurado = produto.ParcelaSegurado,
                    CustoHectare = produto.CustoHectare,
                    SacasPorHa = produto.CustoScHectare,
                    SacasPorAlqueire = produto.CustoScTotal
                });
            }

            foreach (var ts in proposta.PropostasTipoSolo)
            {
                dto.TiposSolo.Add(new MCR.API.Propostas.Domain.DTO.PropostaDetalheTipoSoloDTO
                {
                    Id = ts.Id, PropostaId = ts.PropostaId, TipoSolo = ts.TipoSolo,
                    TipoSoloName = ts.TipoSolo switch { 1 => "Tipo1", 2 => "Tipo2", 3 => "Tipo3", _ => $"Tipo{ts.TipoSolo}" }
                });
            }

            foreach (var cs in proposta.PropostasClassificacaoSolo)
            {
                dto.ClassificacoesSolo.Add(new MCR.API.Propostas.Domain.DTO.PropostaDetalheClassificacaoSoloDTO
                {
                    Id = cs.Id, PropostaId = cs.PropostaId,
                    ClassificacaoSolo = cs.ClassificacaoSolo, ClassificacaoSoloName = cs.ClassificacaoSolo
                });
            }

            return (true, "Proposta obtida com sucesso", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar detalhes da proposta {PropostaId}", propostaId);
            return (false, $"Erro ao buscar detalhes: {ex.Message}", null);
        }
    }
}
