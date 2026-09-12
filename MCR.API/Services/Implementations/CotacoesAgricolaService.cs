using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MCR.API.CotacoesAgricola.Domain.DTO;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations;

public class CotacoesAgricolaService : ICotacoesAgricolaService
{
    private readonly DbContextMCR _context;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUsuarioEstruturaNegocioService _usuarioEstruturaNegocioService;
    private readonly ILogger<CotacoesAgricolaService> _logger;

    public CotacoesAgricolaService(
        DbContextMCR context,
        IHttpContextAccessor contextAccessor,
        IUsuarioEstruturaNegocioService usuarioEstruturaNegocioService,
        ILogger<CotacoesAgricolaService> logger)
    {
        _context = context;
        _contextAccessor = contextAccessor;
        _usuarioEstruturaNegocioService = usuarioEstruturaNegocioService;
        _logger = logger;
    }

    public async Task<CotacoesAgricolaEntity> ObterPorIdAsync(Guid id)
    {
        var retorno = await _context.CotacoesAgricola
            .Include(c => c.Usuario)
            .Include(c => c.Cliente)
            .Include(c => c.Cultura)
            .Include(c => c.Safra)
            .Include(c => c.CotacoesAgricolaTipoSolo)
            .Include(c => c.CotacoesAgricolaClassificacaoSolo)
            .FirstOrDefaultAsync(c => c.Id == id && !c.Excluido);

        if (retorno == null)
        {
            return new CotacoesAgricolaEntity
            {
                Sucesso = false,
                Mensagem = "Cotação não encontrada ou excluída."
            };
        }

        retorno.Sucesso = true;
        return retorno;
    }

    public async Task<IEnumerable<CotacoesAgricolaEntity>> ObterTodosPaginadoAsync(
        string? codigoCotacao = null, Guid? safraId = null, Guid? culturaId = null,
        string? municipio = null, DateTime? dataCotacao = null, string? cpfCliente = null,
        int page = 1, int pageSize = 20)
    {
        var query = _context.CotacoesAgricola
            .Include(t => t.Safra)
            .Include(t => t.Cliente)
            .Include(t => t.Cultura)
            .Where(c => !c.Excluido)
            .AsQueryable();

        var permissoes = await _usuarioEstruturaNegocioService.ObterUsuarioPermissoes();

        if (permissoes.CorretoraIds != null && permissoes.CorretoraIds.Any())
            query = query.Where(x => x.CorretoraId.HasValue && permissoes.CorretoraIds.Contains(x.CorretoraId.Value));

        if (permissoes.CanalIds != null && permissoes.CanalIds.Any())
            query = query.Where(x => x.CanalId.HasValue && permissoes.CanalIds.Contains(x.CanalId.Value));

        if (permissoes.PontoAtendimentoIds != null && permissoes.PontoAtendimentoIds.Any())
            query = query.Where(x => x.PontoAtendimentoId.HasValue && permissoes.PontoAtendimentoIds.Contains(x.PontoAtendimentoId.Value));

        if (!string.IsNullOrEmpty(codigoCotacao))
            query = query.Where(c => c.CodigoCotacao != null && c.CodigoCotacao.Contains(codigoCotacao));

        if (safraId.HasValue)
            query = query.Where(c => c.SafraId == safraId.Value);

        if (culturaId.HasValue)
            query = query.Where(c => c.CulturaId == culturaId.Value);

        if (!string.IsNullOrEmpty(municipio))
            query = query.Where(c => c.Municipio != null && c.Municipio.Contains(municipio));

        if (dataCotacao.HasValue)
            query = query.Where(c => c.DataCotacao.HasValue && c.DataCotacao.Value.Date == dataCotacao.Value.Date);

        if (!string.IsNullOrEmpty(cpfCliente))
            query = query.Where(c => c.ClienteCPF != null && c.ClienteCPF.Contains(cpfCliente));

        var totalItens = await query.AsNoTracking().CountAsync();
        var totalPaginas = (int)Math.Ceiling(totalItens / (double)pageSize);

        var cotacoesPaginadas = await query
            .AsNoTracking()
            .OrderByDescending(c => c.DataCotacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        foreach (var item in cotacoesPaginadas)
        {
            item.Sucesso = true;
            item.TotalItems = totalItens;
            item.TotalPages = totalPaginas;
        }

        return cotacoesPaginadas;
    }

    public async Task<CotacoesAgricolaEntity> CadastrarAsync(CotacoesAgricolaEntity entity)
    {
        entity.Ativo = true;
        entity.Excluido = false;
        entity.DataCotacao = DateTime.Now;
        entity.Status = "Em Andamento";

        entity.CodigoCotacao = await GerarCodigoCotacao();

        _context.CotacoesAgricola.Add(entity);
        var retorno = await _context.SaveChangesAsync();

        if (retorno > 0)
        {
            await SalvarHistoricoStatus(entity.Id, "Em Andamento", null);
            try
            {
                entity.MotorDiagnostico = await MotorDeCotacao(entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar MotorDeCotacao para cotação {CotacaoId} após cadastro", entity.Id);
                entity.MotorDiagnostico = null;
            }
        }

        entity.Sucesso = retorno > 0;
        entity.Mensagem = retorno > 0
            ? "Cotação cadastrada com sucesso."
            : "Não foi possível cadastrar a cotação. Feche a tela e tente novamente.";

        return entity;
    }

    public async Task<CotacoesAgricolaEntity> AtualizarAsync(CotacoesAgricolaEntity entity)
    {
        var existing = await _context.CotacoesAgricola
            .Include(c => c.CotacoesAgricolaTipoSolo)
            .Include(c => c.CotacoesAgricolaClassificacaoSolo)
            .FirstOrDefaultAsync(c => c.Id == entity.Id);

        if (existing == null)
        {
            entity.Sucesso = false;
            entity.Mensagem = "Cotação não encontrada.";
            return entity;
        }

        existing.ClienteId = entity.ClienteId;
        existing.ClienteCPF = entity.ClienteCPF;
        existing.ClienteNome = entity.ClienteNome;
        existing.CulturaId = entity.CulturaId;
        existing.SafraId = entity.SafraId;
        existing.Estado = entity.Estado;
        existing.Municipio = entity.Municipio;
        existing.AreaTotal = entity.AreaTotal;
        existing.IsModalidadeProdutividade = entity.IsModalidadeProdutividade;
        existing.PrecoSaca = entity.PrecoSaca;
        existing.ValorCusteio = entity.ValorCusteio;
        existing.PlantioConsorciado = entity.PlantioConsorciado;
        existing.LavouraIrrigada = entity.LavouraIrrigada;
        existing.PlantioDireto = entity.PlantioDireto;
        existing.PosCana = entity.PosCana;
        existing.CustoProducao = entity.CustoProducao;
        existing.SubvencaoFederal = entity.SubvencaoFederal;
        existing.SubvencaoEstadual = entity.SubvencaoEstadual;
        existing.CorretoraId = entity.CorretoraId;
        existing.CanalId = entity.CanalId;
        existing.PontoAtendimentoId = entity.PontoAtendimentoId;
        existing.DataCotacao = DateTime.UtcNow;

        // Forçar atualização dos campos escalares via SQL raw
        await _context.Database.ExecuteSqlRawAsync(
            @"UPDATE ""CotacoesAgricola"" SET
                ""ClienteId"" = {0}, ""ClienteCPF"" = {1}, ""ClienteNome"" = {2},
                ""CulturaId"" = {3}, ""SafraId"" = {4}, ""Estado"" = {5}, ""Municipio"" = {6},
                ""AreaTotal"" = {7}, ""IsModalidadeProdutividade"" = {8}, ""PrecoSaca"" = {9},
                ""ValorCusteio"" = {10}, ""PlantioConsorciado"" = {11}, ""LavouraIrrigada"" = {12},
                ""PlantioDireto"" = {13}, ""PosCana"" = {14}, ""CustoProducao"" = {15},
                ""SubvencaoFederal"" = {16}, ""SubvencaoEstadual"" = {17},
                ""CorretoraId"" = {18}, ""CanalId"" = {19}, ""PontoAtendimentoId"" = {20},
                ""DataCotacao"" = {22}
            WHERE ""Id"" = {21}",
            existing.ClienteId, existing.ClienteCPF, existing.ClienteNome,
            existing.CulturaId, existing.SafraId, existing.Estado, existing.Municipio,
            existing.AreaTotal, existing.IsModalidadeProdutividade, existing.PrecoSaca,
            existing.ValorCusteio, existing.PlantioConsorciado, existing.LavouraIrrigada,
            existing.PlantioDireto, existing.PosCana, existing.CustoProducao,
            existing.SubvencaoFederal, existing.SubvencaoEstadual,
            existing.CorretoraId, existing.CanalId, existing.PontoAtendimentoId,
            existing.Id, (DateTime?)existing.DataCotacao);

        _logger.LogInformation("[ATUALIZAR-PRE] Id={Id}, AreaTotal={AreaTotal}, CulturaId={CulturaId}, PrecoSaca={PrecoSaca}, ValorCusteio={ValorCusteio}, SubvFederal={SF}, SubvEstadual={SE}",
            existing.Id, existing.AreaTotal, existing.CulturaId, existing.PrecoSaca, existing.ValorCusteio, existing.SubvencaoFederal, existing.SubvencaoEstadual);

        // Remove registros antigos (shadow FK = CotacoesAgricolaId)
        _context.CotacoesAgricolaTipoSolo.RemoveRange(
            await _context.CotacoesAgricolaTipoSolo
                .Where(x => EF.Property<Guid>(x, "CotacoesAgricolaId") == existing.Id)
                .ToListAsync());
        _context.CotacoesAgricolaClassificacaoSolo.RemoveRange(
            await _context.CotacoesAgricolaClassificacaoSolo
                .Where(x => EF.Property<Guid>(x, "CotacoesAgricolaId") == existing.Id)
                .ToListAsync());

        // Adiciona novos registros diretamente no DbSet com shadow FK manual
        foreach (var item in entity.CotacoesAgricolaTipoSolo ?? new List<CotacoesAgricolaTipoSoloEntity>())
        {
            _context.Entry(item).Property("CotacoesAgricolaId").CurrentValue = existing.Id;
            _context.CotacoesAgricolaTipoSolo.Add(item);
            _logger.LogInformation("[SAVEDEBUG] Add TipoSolo={Valor}, ShadowFK={FK}", item.TipoSolo, existing.Id);
        }
        foreach (var item in entity.CotacoesAgricolaClassificacaoSolo ?? new List<CotacoesAgricolaClassificacaoSoloEntity>())
        {
            _context.Entry(item).Property("CotacoesAgricolaId").CurrentValue = existing.Id;
            _context.CotacoesAgricolaClassificacaoSolo.Add(item);
            _logger.LogInformation("[SAVEDEBUG] Add ClassifSolo={Valor}, ShadowFK={FK}", item.ClassificacaoSolo, existing.Id);
        }

        var retorno = await _context.SaveChangesAsync();
        _logger.LogInformation("[SAVEDEBUG] SaveChangesAsync retornou {r}", retorno);

        try
        {
            _context.Entry(existing).State = EntityState.Detached;
            existing = await _context.CotacoesAgricola
                .Include(c => c.Cliente)
                .Include(c => c.CotacoesAgricolaTipoSolo)
                .Include(c => c.CotacoesAgricolaClassificacaoSolo)
                .FirstOrDefaultAsync(c => c.Id == entity.Id) ?? existing;

            _logger.LogInformation("[ATUALIZAR-POST-SAVE] Id={Id}, AreaTotal={AreaTotal}, CulturaId={CulturaId}, PrecoSaca={PrecoSaca}, ValorCusteio={ValorCusteio}, TipoSolo={TS}, ClassSolo={CS}",
                existing.Id, existing.AreaTotal, existing.CulturaId, existing.PrecoSaca, existing.ValorCusteio,
                string.Join(",", existing.CotacoesAgricolaTipoSolo?.Select(t => t.TipoSolo) ?? Enumerable.Empty<int>()),
                string.Join(",", existing.CotacoesAgricolaClassificacaoSolo?.Select(c => c.ClassificacaoSolo) ?? Enumerable.Empty<string>()));

            existing.MotorDiagnostico = await MotorDeCotacao(existing.Id);

            _logger.LogInformation("[ATUALIZAR-MOTOR-RESULT] Id={Id}, QtdProdutos={QP}, QtdPropostas={QPR}, Motivo={Motivo}",
                existing.Id,
                existing.MotorDiagnostico?.QtdProdutosElegiveis ?? 0,
                existing.MotorDiagnostico?.QtdPropostasGravadas ?? 0,
                existing.MotorDiagnostico?.MotivoSemPropostas ?? "(nenhum)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar MotorDeCotacao para cotação {CotacaoId} após atualização", existing.Id);
            existing.MotorDiagnostico = null;
        }

        existing.Sucesso = true;
        existing.Mensagem = "Cotação atualizada com sucesso.";

        await _context.Database.ExecuteSqlRawAsync(
            "UPDATE \"CotacoesAgricola\" SET \"PlantioConsorciado\" = {0}, \"LavouraIrrigada\" = {1}, \"PlantioDireto\" = {2}, \"PosCana\" = {3} WHERE \"Id\" = {4}",
            existing.PlantioConsorciado, existing.LavouraIrrigada, existing.PlantioDireto, existing.PosCana, existing.Id);

        return existing;
    }

    public async Task<bool> ExcluirAsync(Guid id)
    {
        var entity = await _context.CotacoesAgricola.FirstOrDefaultAsync(c => c.Id == id);
        if (entity == null) return false;

        entity.Excluido = true;
        _context.CotacoesAgricola.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RegistrarInsucessoAsync(Guid id)
    {
        var entity = await _context.CotacoesAgricola.FirstOrDefaultAsync(c => c.Id == id);
        if (entity == null) return false;

        var statusAnterior = entity.Status;
        entity.Status = "Cotação encerrada sem sucesso";
        entity.DataInsucesso = DateTime.Now;
        _context.CotacoesAgricola.Update(entity);

        var retorno = await _context.SaveChangesAsync() > 0;
        if (retorno)
            await SalvarHistoricoStatus(id, "Cotação encerrada sem sucesso", statusAnterior);

        return retorno;
    }

    public async Task<bool> ReabrirAsync(Guid id)
    {
        var entity = await _context.CotacoesAgricola.FirstOrDefaultAsync(c => c.Id == id);
        if (entity == null) return false;
        if (!entity.DataInsucesso.HasValue) return false;

        var statusAnterior = entity.Status;
        entity.Status = "Em Andamento";
        entity.DataInsucesso = null;
        _context.CotacoesAgricola.Update(entity);

        var retorno = await _context.SaveChangesAsync() > 0;
        if (retorno)
            await SalvarHistoricoStatus(id, "Em Andamento", statusAnterior);

        return retorno;
    }

    public async Task<byte[]> GerarPdfAsync(Guid id)
    {
        var pdfData = await ObterCotacaoPdf(id);
        if (pdfData == null) return Array.Empty<byte>();

        var html = RenderPdfHtml(pdfData);
        return System.Text.Encoding.UTF8.GetBytes(html);
    }

    public async Task<byte[]> DownloadPdfAsync(Guid id)
    {
        return await GerarPdfAsync(id);
    }

    public async Task<CotacaoAgricolaDadosPropostasDTO> ObterDadosPropostaCotacao(Guid cotacaoId, string order = "segurada")
    {
        var cotacao = await _context.CotacoesAgricola
            .Include(x => x.CotacoesAgricolaTipoSolo)
            .Include(x => x.CotacoesAgricolaClassificacaoSolo)
            .FirstOrDefaultAsync(x => x.Id == cotacaoId);

        if (cotacao == null)
        {
            return new CotacaoAgricolaDadosPropostasDTO
            {
                CotacaoAgricolaId = cotacaoId,
                HasClient = false,
                MotivoSemPropostas = "Cotação não encontrada."
            };
        }

        var query = _context.CotacoesAgricolaProposta
            .Include(t => t.Produto)
            .Include(t => t.Seguradora)
            .Include(t => t.CotacoesAgricola)
            .Where(x => x.CotacaoAgricolaId == cotacaoId)
            .AsQueryable();

        if (order == "parcela")
            query = query.OrderBy(x => x.ParcelaSegurado);
        else
            query = query.OrderByDescending(x => x.ProdutividadeSegurada);

        var propostas = await query.Take(4).ToListAsync();

        var coberturas = propostas.Select(p => new CotacaoAgricolaPropostaDTO
        {
            TipoOferta = p.TipoOferta ?? "Municipal",
            OpcaoSeguradora = p.Opcao,
            Seguradora = p.Seguradora?.RazaoSocial ?? string.Empty,
            NomeProduto = p.Produto?.NomeProduto ?? string.Empty,
            RegulacaoSinistro = p.RegulacaoSinistro,
            ProdutividadeEsperada = p.ProdutividadeEsperada.ToString(),
            CotacaoAgricolaPropostaId = p.Id,
            NivelCobertura = p.NivelCobertura,
            ProdutividadeSegurada = p.ProdutividadeSegurada,
            LMIProducaoHectare = p.LMIProducaoHectare,
            LMIReplantioHectare = p.LMIReplantioHectare,
            LMIProducaoTotal = p.LMIProducaoTotal,
            LMIReplantioTotal = p.LMIReplantioTotal,
            PremioTotal = p.PremioTotal.ToString("C2"),
            SubvencaoFederal = p.SubvencaoFederal.ToString("C2"),
            SubvencaoEstadual = p.SubvencaoEstadual.ToString("C2"),
            ParcelaSegurado = p.ParcelaSegurado.ToString("C2"),
            CustoHectare = p.CustoHectare.ToString("C2"),
            SacasPorHa = p.CustoScHectare.ToString("N2"),
            SacasPorAlqueire = p.CustoScTotal.ToString("N2"),
            TiposSoloProduto = p.Produto?.TipoSolo != null && p.Produto.TipoSolo.Length > 0
                ? string.Join(", ", p.Produto.TipoSolo.Select(t => t?.Replace("Tipo", "Tipo ") ?? string.Empty))
                : null
        }).ToList();

        var dto = new CotacaoAgricolaDadosPropostasDTO
        {
            Coberturas = coberturas,
            CotacaoAgricolaId = cotacaoId,
            HasClient = propostas.Any() ? propostas.First().CotacoesAgricola?.ClienteId.HasValue == true : false
        };

        if (coberturas.Count == 0)
        {
            dto.MotivoSemPropostas = await DiagnosticarSemPropostas(cotacao);
            dto.ErrosValidacao = ValidarCamposMotorCotacao(cotacao);
        }

        return dto;
    }

    public Task<List<string>> ObterAcoesPorStatus(string status, string role)
    {
        var acoes = new List<string>();

        switch (status)
        {
            case "Em Andamento":
                acoes.Add("PDF");
                acoes.Add("Insucesso");
                if (role == "Administrador" || role == "Corretor")
                    acoes.Add("Excluir");
                break;

            case "Cotação realizada com sucesso":
                acoes.Add("PDF");
                acoes.Add("Ver Propostas");
                acoes.Add("Insucesso");
                break;

            case "Cotação em negociação":
                acoes.Add("PDF");
                acoes.Add("Ver Propostas");
                acoes.Add("Insucesso");
                break;

            case "Cotação encerrada sem sucesso":
                if (role == "Administrador" || role == "Corretor")
                    acoes.Add("Reabrir");
                break;
        }

        return Task.FromResult(acoes);
    }

    public async Task VerificarCotacoesEmAberto()
    {
        const int DiasParaEncerramento = 30;
        var dataLimite = DateTime.UtcNow.AddDays(-DiasParaEncerramento);

        var cotacoesParaEncerrar = await _context.CotacoesAgricola
            .Where(c => c.Status == "Cotação em negociação" && c.DataCotacao <= dataLimite)
            .ToListAsync();

        foreach (var cotacao in cotacoesParaEncerrar)
        {
            var statusAnterior = cotacao.Status;
            cotacao.Status = "Cotação encerrada sem sucesso";
            cotacao.DataInsucesso = DateTime.UtcNow;
            _context.Entry(cotacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await SalvarHistoricoStatus(cotacao.Id, "Cotação encerrada sem sucesso", statusAnterior);
        }
    }

    public async Task<IEnumerable<object>> ObterTiposSoloPorCulturaSafraAsync(Guid culturaId, Guid safraId)
    {
        var tiposSolo = await _context.Produtos
            .Where(x => x.Ativo && !x.Excluido && x.CulturaId == culturaId && x.SafraId == safraId)
            .Select(x => x.TipoSolo)
            .ToListAsync();

        var mapa = new Dictionary<string, (int value, string text)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Tipo1"] = (1, "Tipo 1"),
            ["Tipo2"] = (2, "Tipo 2"),
            ["Tipo3"] = (3, "Tipo 3")
        };

        var tiposDistintos = tiposSolo
            .Where(t => t != null)
            .SelectMany(t => t!)
            .Distinct()
            .ToList();

        return tiposDistintos
            .Where(t => mapa.TryGetValue(t, out _))
            .Select(t => new { value = mapa[t].value.ToString(), text = mapa[t].text })
            .OrderBy(x => int.Parse(x.value))
            .ToList();
    }

    public async Task<IEnumerable<string>> ObterClassificacoesSoloPorCulturaSafraAsync(Guid culturaId, Guid safraId)
    {
        var classificacoes = await _context.Produtos
            .Where(x => x.Ativo && !x.Excluido && x.CulturaId == culturaId && x.SafraId == safraId)
            .Select(x => x.ClassificacaoSolosAceitos)
            .ToListAsync();

        return classificacoes
            .Where(t => t != null)
            .SelectMany(t => t!)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    private async Task<string> GerarCodigoCotacao()
    {
        var data = DateTime.Now;
        var ultimaCotacao = await _context.CotacoesAgricola
            .Where(c => c.DataCotacao.HasValue && c.DataCotacao.Value.Date == data.Date)
            .OrderByDescending(c => c.CodigoCotacao)
            .FirstOrDefaultAsync();

        int sequencial = 1;
        if (ultimaCotacao != null && !string.IsNullOrEmpty(ultimaCotacao.CodigoCotacao))
        {
            var ultimoSequencial = int.Parse(ultimaCotacao.CodigoCotacao.Substring(8));
            sequencial = ultimoSequencial + 1;
        }

        return $"{data:yyyyMMdd}{sequencial:D4}";
    }

    private async Task SalvarHistoricoStatus(Guid cotacaoId, string status, string? statusAnterior)
    {
        var historico = new CotacoesAgricolaStatusEntity
        {
            Id = Guid.NewGuid(),
            CotacaoAgricolaId = cotacaoId,
            Status = status,
            StatusAnterior = statusAnterior,
            DataStatus = DateTime.UtcNow,
            UsuarioId = _contextAccessor.GetUserId() ?? Guid.Empty
        };

        _context.CotacoesAgricolaStatus.Add(historico);
        await _context.SaveChangesAsync();
    }

    private static List<MotorCotacaoErroValidacaoDTO> ValidarCamposMotorCotacao(CotacoesAgricolaEntity cotacao)
    {
        var erros = new List<MotorCotacaoErroValidacaoDTO>();

        if (cotacao.CulturaId == Guid.Empty)
            erros.Add(new() { Campo = "CulturaId", Mensagem = "Cultura não selecionada." });
        if (cotacao.SafraId == Guid.Empty)
            erros.Add(new() { Campo = "SafraId", Mensagem = "Safra não selecionada." });
        if (string.IsNullOrWhiteSpace(cotacao.Estado))
            erros.Add(new() { Campo = "Estado", Mensagem = "Estado não informado." });
        if (string.IsNullOrWhiteSpace(cotacao.Municipio))
            erros.Add(new() { Campo = "Municipio", Mensagem = "Município não informado." });
        if (cotacao.AreaTotal <= 0)
            erros.Add(new() { Campo = "AreaTotal", Mensagem = "Área total deve ser maior que zero." });
        if (!cotacao.CorretoraId.HasValue)
            erros.Add(new() { Campo = "CorretoraId", Mensagem = "Corretora não selecionada." });
        if (!cotacao.CanalId.HasValue)
            erros.Add(new() { Campo = "CanalId", Mensagem = "Canal não selecionado." });
        if (!cotacao.PontoAtendimentoId.HasValue)
            erros.Add(new() { Campo = "PontoAtendimentoId", Mensagem = "Ponto de Atendimento não selecionado." });
        if (cotacao.CotacoesAgricolaTipoSolo == null || !cotacao.CotacoesAgricolaTipoSolo.Any())
            erros.Add(new() { Campo = "TipoSolo", Mensagem = "Nenhum tipo de solo selecionado." });
        if (cotacao.CotacoesAgricolaClassificacaoSolo == null || !cotacao.CotacoesAgricolaClassificacaoSolo.Any())
            erros.Add(new() { Campo = "ClassificacaoSolo", Mensagem = "Nenhuma classificação de solo selecionada." });
        if (cotacao.IsModalidadeProdutividade)
        {
            if (!cotacao.PrecoSaca.HasValue || cotacao.PrecoSaca.Value <= 0)
                erros.Add(new() { Campo = "PrecoSaca", Mensagem = "Preço da saca não informado (obrigatório para modalidade Produtividade)." });
        }
        else
        {
            if (!cotacao.ValorCusteio.HasValue || cotacao.ValorCusteio.Value <= 0)
                erros.Add(new() { Campo = "ValorCusteio", Mensagem = "Valor de custeio não informado (obrigatório para modalidade Custeio)." });
        }

        return erros;
    }

    private async Task<MotorCotacaoDiagnosticoDTO?> MotorDeCotacao(Guid cotacaoAgricolaId)
    {
        var cotacao = await _context.CotacoesAgricola
            .Include(x => x.PontoAtendimento)
            .Include(x => x.Cliente)
            .Include(x => x.CotacoesAgricolaTipoSolo)
            .Include(x => x.CotacoesAgricolaClassificacaoSolo)
            .FirstOrDefaultAsync(x => x.Id == cotacaoAgricolaId);

        if (cotacao == null) return null;

        var errosValidacao = ValidarCamposMotorCotacao(cotacao);
        if (errosValidacao.Any())
        {
            var passos = await DiagnosticarPassoAPasso(cotacao);
            return new MotorCotacaoDiagnosticoDTO
            {
                CotacaoId = cotacao.Id,
                Estado = cotacao.Estado,
                Municipio = cotacao.Municipio,
                CulturaId = cotacao.CulturaId,
                SafraId = cotacao.SafraId,
                CanalId = cotacao.CanalId,
                PontoAtendimentoId = cotacao.PontoAtendimentoId,
                AreaTotal = cotacao.AreaTotal,
                IsModalidadeProdutividade = cotacao.IsModalidadeProdutividade,
                QtdProdutosElegiveis = 0,
                QtdPropostasGravadas = 0,
                MotivoSemPropostas = "Campos obrigatórios não preenchidos.",
                ErrosValidacao = errosValidacao,
                PassoAPasso = passos
            };
        }

        _context.CotacoesAgricolaProposta.RemoveRange(
            _context.CotacoesAgricolaProposta.Where(x => x.CotacaoAgricolaId == cotacao.Id));
        await _context.SaveChangesAsync();

        var produtosElegiveis = await BuscarProdutosElegiveis(cotacao);

        if (!produtosElegiveis.Any())
        {
            var passos = await DiagnosticarPassoAPasso(cotacao);
            return new MotorCotacaoDiagnosticoDTO
            {
                CotacaoId = cotacao.Id,
                Estado = cotacao.Estado,
                Municipio = cotacao.Municipio,
                CulturaId = cotacao.CulturaId,
                SafraId = cotacao.SafraId,
                CanalId = cotacao.CanalId,
                PontoAtendimentoId = cotacao.PontoAtendimentoId,
                AreaTotal = cotacao.AreaTotal,
                IsModalidadeProdutividade = cotacao.IsModalidadeProdutividade,
                QtdProdutosElegiveis = 0,
                QtdPropostasGravadas = 0,
                MotivoSemPropostas = await DiagnosticarSemPropostas(cotacao),
                ErrosValidacao = await DiagnosticarErrosSemPropostas(cotacao),
                PassoAPasso = passos
            };
        }

        var qtdAntes = await _context.CotacoesAgricolaProposta.CountAsync(x => x.CotacaoAgricolaId == cotacao.Id);
        await GerarPropostas(cotacao, produtosElegiveis);
        var qtdDepois = await _context.CotacoesAgricolaProposta.CountAsync(x => x.CotacaoAgricolaId == cotacao.Id);

        string? detalheTaxas = null;
        Guid? produtoIdElegivel = null;
        string? nomeProdutoElegivel = null;

        if (qtdDepois == 0 && produtosElegiveis.Any())
        {
            var primeiro = produtosElegiveis.First();
            produtoIdElegivel = primeiro.Id;
            nomeProdutoElegivel = primeiro.NomeProduto;
            detalheTaxas = ObterDetalheTaxasParaDiagnostico(primeiro, cotacao);
        }

        var errosDiagnostico = qtdDepois == 0 ? await DiagnosticarErrosSemPropostas(cotacao) : null;

        return new MotorCotacaoDiagnosticoDTO
        {
            CotacaoId = cotacao.Id,
            Estado = cotacao.Estado,
            Municipio = cotacao.Municipio,
            CulturaId = cotacao.CulturaId,
            SafraId = cotacao.SafraId,
            CanalId = cotacao.CanalId,
            PontoAtendimentoId = cotacao.PontoAtendimentoId,
            AreaTotal = cotacao.AreaTotal,
            IsModalidadeProdutividade = cotacao.IsModalidadeProdutividade,
            QtdProdutosElegiveis = produtosElegiveis.Count,
            QtdPropostasGravadas = qtdDepois,
            MotivoSemPropostas = qtdDepois == 0 ? "Produtos elegíveis encontrados mas nenhuma proposta gravada." : null,
            ProdutoIdElegivel = produtoIdElegivel,
            NomeProdutoElegivel = nomeProdutoElegivel,
            DetalheTaxas = detalheTaxas,
            ErrosValidacao = errosDiagnostico
        };
    }

    private async Task<List<ProdutosEntity>> BuscarProdutosElegiveis(CotacoesAgricolaEntity cotacao)
    {
        var query = _context.Produtos
            .Include(x => x.Taxas)
            .Include(x => x.Seguradora)
            .Include(x => x.ProdutosCanalPontoAtendimento)
            .Include(x => x.ProdutosSubvencoesEstaduais)
                .ThenInclude(ps => ps.SubvencaoEstadual)
            .Include(x => x.SubvencaoEstadual)
            .Include(x => x.SubvencaoFederal)
            .Where(x => x.Ativo && !x.Excluido)
            .AsQueryable();

        if (cotacao.PontoAtendimentoId.HasValue && cotacao.CanalId.HasValue)
        {
            query = query.Where(x => x.ProdutosCanalPontoAtendimento
                .Any(c => c.PontoAtendimentoId == cotacao.PontoAtendimentoId.Value && c.CanalId == cotacao.CanalId.Value));
        }

        query = query.Where(x =>
            x.CulturaId == cotacao.CulturaId &&
            x.SafraId == cotacao.SafraId);

        query = query.Where(x => x.AreaMinimaTotal <= cotacao.AreaTotal);

        if (!string.IsNullOrEmpty(cotacao.Municipio))
        {
            var municipioNorm = cotacao.Municipio.Trim().ToLower();
            query = query.Where(x => x.Taxas.Any(t =>
                t.UF == cotacao.Estado &&
                (t.Municipio ?? "").ToLower() == municipioNorm));
        }

        if (cotacao.IsModalidadeProdutividade)
        {
            query = query.Where(x =>
                x.Modalidade == "Produtividade" &&
                x.ValorSacaMinimo <= cotacao.PrecoSaca &&
                x.ValorSacaMaximo >= cotacao.PrecoSaca);
        }
        else
        {
            query = query.Where(x =>
                x.Modalidade == "Custeio" &&
                x.ValorCusteioMinimo <= cotacao.ValorCusteio &&
                x.ValorCusteioMaximo >= cotacao.ValorCusteio);
        }

        var tiposSoloAceitos = cotacao.CotacoesAgricolaTipoSolo?
            .Select(a => a.TipoSolo switch
            {
                1 => "Tipo1",
                2 => "Tipo2",
                3 => "Tipo3",
                _ => null
            })
            .Where(t => t != null)
            .ToList() ?? new List<string?>();

        if (tiposSoloAceitos.Any())
            query = query.Where(x => x.TipoSolo.Any(t => tiposSoloAceitos.Contains(t)));

        var classificacoesSolo = cotacao.CotacoesAgricolaClassificacaoSolo?
            .Select(a => a.ClassificacaoSolo)
            .ToList() ?? new List<string>();

        if (classificacoesSolo.Any())
            query = query.Where(x => x.ClassificacaoSolosAceitos.Any(c => classificacoesSolo.Contains(c)));

        query = query.Where(x =>
            (!cotacao.PlantioConsorciado || x.AceitaPlantioConsorciado) &&
            (!cotacao.LavouraIrrigada || x.LavouraIrrigada) &&
            (!cotacao.PlantioDireto || x.AceitaPlantioConvencional) &&
            (!cotacao.PosCana || x.AceitaPlantioPosCanaDeAcucar));

        return await query.ToListAsync();
    }

    private async Task GerarPropostas(CotacoesAgricolaEntity cotacao, List<ProdutosEntity> produtos)
    {
        var propostas = new List<CotacoesAgricolaPropostaEntity>();
        var opcao = 1;
        decimal[] niveisCobertura = { 75m, 70m, 65m };

        foreach (var produto in produtos)
        {
            if (opcao > 4) break;

            var taxaMunicipio = produto.Taxas.FirstOrDefault(x =>
                x.UF == cotacao.Estado &&
                string.Equals((x.Municipio ?? "").Trim(), (cotacao.Municipio ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                CpfConsideradoMunicipal(x.Cpf));

            ProdutosTaxasEntity? taxaCliente = null;
            if (cotacao.Cliente != null)
            {
                taxaCliente = produto.Taxas.FirstOrDefault(x =>
                    x.UF == cotacao.Estado &&
                    string.Equals((x.Municipio ?? "").Trim(), (cotacao.Municipio ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    x.Cpf == cotacao.Cliente.CPF);
            }
            else if (cotacao.ClienteCPF != null)
            {
                taxaCliente = produto.Taxas.FirstOrDefault(x =>
                    x.UF == cotacao.Estado &&
                    string.Equals((x.Municipio ?? "").Trim(), (cotacao.Municipio ?? "").Trim(), StringComparison.OrdinalIgnoreCase) &&
                    x.Cpf == cotacao.ClienteCPF);
            }

            var taxasParaProcessar = new List<(ProdutosTaxasEntity taxa, string tipoOferta)>();

            if (taxaMunicipio != null)
                taxasParaProcessar.Add((taxaMunicipio, "Municipal"));

            if (taxaCliente != null)
                taxasParaProcessar.Add((taxaCliente, "Personalizada"));

            if (!taxasParaProcessar.Any()) continue;

            foreach (var (taxa, tipoOferta) in taxasParaProcessar)
            {
                var niveisDisponiveis = niveisCobertura.Where(nivel => ObterTaxaNivelCobertura(taxa, nivel) > 0);
                if (!niveisDisponiveis.Any()) continue;

                foreach (var nivel in niveisDisponiveis)
                {
                    var valores = CalcularValoresProposta(produto, cotacao, taxa, nivel);

                    var proposta = new CotacoesAgricolaPropostaEntity
                    {
                        Id = Guid.NewGuid(),
                        CotacaoAgricolaId = cotacao.Id,
                        Opcao = opcao,
                        TipoOferta = tipoOferta,
                        SeguradoraId = produto.SeguradoraId,
                        ProdutoId = produto.Id,
                        RegulacaoSinistro = produto.RegulacaoSinistro,
                        ProdutividadeEsperada = valores.ProdutividadeEsperada,
                        NivelCobertura = nivel,
                        ProdutividadeSegurada = valores.ProdutividadeSegurada,
                        LMIProducaoHectare = valores.LMIProducaoHectare,
                        LMIReplantioHectare = valores.LMIReplantioHectare,
                        LMIProducaoTotal = valores.LMIProducaoTotal,
                        LMIReplantioTotal = valores.LMIReplantioTotal,
                        PremioTotal = valores.PremioTotal,
                        SubvencaoFederal = valores.SubvencaoFederal,
                        SubvencaoEstadual = valores.SubvencaoEstadual,
                        ParcelaSegurado = valores.ParcelaSegurado,
                        CustoHectare = valores.CustoHectare,
                        CustoScHectare = valores.SacasPorHa,
                        CustoScTotal = valores.SacasPorAlqueire,
                        Ativo = true,
                        Excluido = false,
                        Sucesso = true
                    };

                    propostas.Add(proposta);
                }
            }

            opcao++;
        }

        if (propostas.Any())
        {
            string? statusAnterior = null;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.CotacoesAgricolaProposta.AddRangeAsync(propostas);
                    statusAnterior = cotacao.Status;
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE \"CotacoesAgricola\" SET \"Status\" = {0} WHERE \"Id\" = {1}",
                        "Cotação realizada com sucesso", cotacao.Id);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao gravar propostas da cotação {CotacaoId}. Inner: {Inner}",
                        cotacao.Id, ex.InnerException?.Message);
                    throw;
                }
            }
            await SalvarHistoricoStatus(cotacao.Id, "Cotação realizada com sucesso", statusAnterior);
        }
    }

    private static decimal ObterTaxaNivelCobertura(ProdutosTaxasEntity taxa, decimal nivelCobertura)
    {
        return nivelCobertura switch
        {
            65 => taxa.TaxaNc65,
            70 => taxa.TaxaNc70 ?? 0,
            75 => taxa.TaxaNc75 ?? 0,
            _ => 0
        };
    }

    private static bool CpfConsideradoMunicipal(string? cpf)
    {
        var t = cpf?.Trim() ?? "";
        return t.Length == 0 || t == "0";
    }

    /// <summary>
    /// Gera texto de diagnóstico quando há produto elegível mas nenhuma proposta (ex.: taxa só com CPF ou taxas 65/70/75 zeradas).
    /// </summary>
    private string ObterDetalheTaxasParaDiagnostico(ProdutosEntity produto, CotacoesAgricolaEntity cotacao)
    {
        var municipioCotacao = (cotacao.Municipio ?? string.Empty).Trim();
        var estado = cotacao.Estado ?? "";
        var taxasParaUFMunicipio = (produto.Taxas ?? Enumerable.Empty<ProdutosTaxasEntity>())
            .Where(t => t.UF == estado &&
                string.Equals((t.Municipio ?? "").Trim(), municipioCotacao, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (taxasParaUFMunicipio.Count == 0)
            return $"Produto não possui taxa cadastrada para {estado}/{municipioCotacao}.";

        var taxasMunicipais = taxasParaUFMunicipio.Where(t => CpfConsideradoMunicipal(t.Cpf)).ToList();
        var taxasCliente = taxasParaUFMunicipio.Where(t => !CpfConsideradoMunicipal(t.Cpf)).ToList();

        if (taxasMunicipais.Count == 0 && taxasCliente.Count > 0)
        {
            var cpfCotacao = cotacao.Cliente?.CPF?.Trim() ?? cotacao.ClienteCPF?.Trim() ?? "";
            return $"Produto possui {taxasCliente.Count} taxa(s) para {estado}/{municipioCotacao} apenas com CPF (taxa cliente). Cotação sem CPF ou CPF diferente do cadastrado.";
        }

        if (taxasMunicipais.Count > 0)
        {
            var algumaComNivel = taxasMunicipais.Any(t =>
                ObterTaxaNivelCobertura(t, 65) > 0 || ObterTaxaNivelCobertura(t, 70) > 0 || ObterTaxaNivelCobertura(t, 75) > 0);
            if (!algumaComNivel)
                return "Taxa municipal encontrada mas TaxaNc65/70/75 zeradas no cadastro do produto.";
        }

        return "Taxa encontrada para Estado/Município; verificar outros critérios (ex.: subvenção, área).";
    }

    private (decimal ProdutividadeEsperada, decimal ProdutividadeSegurada,
        decimal LMIProducaoHectare, decimal LMIReplantioHectare,
        decimal LMIProducaoTotal, decimal LMIReplantioTotal,
        decimal PremioTotal, decimal SubvencaoFederal, decimal SubvencaoEstadual,
        decimal ParcelaSegurado, decimal CustoHectare,
        decimal SacasPorHa, decimal SacasPorAlqueire)
        CalcularValoresProposta(ProdutosEntity produto, CotacoesAgricolaEntity cotacao,
            ProdutosTaxasEntity taxa, decimal nivelCobertura)
    {
        var produtividadeEsperada = CalcularProdutividadeEsperada(cotacao, produto, taxa);
        var produtividadeSegurada = produtividadeEsperada * (nivelCobertura / 100);

        var lmiProducaoHectare = cotacao.IsModalidadeProdutividade
            ? (produtividadeSegurada / (produto.KiloPorSaca ?? 60)) * (cotacao.PrecoSaca ?? 0)
            : cotacao.ValorCusteio ?? 0;

        var lmiReplantioHectare = produto.Replantio == "Cobertura de produção"
            ? lmiProducaoHectare * (produto.PorcentagemCoberturaProducao ?? 0) / 100
            : produto.ValorCoberturaAdicional ?? 0;

        var lmiProducaoTotal = lmiProducaoHectare * cotacao.AreaTotal;
        var lmiReplantioTotal = lmiReplantioHectare * cotacao.AreaTotal;

        var taxaNivel = ObterTaxaNivelCobertura(taxa, nivelCobertura);
        var taxaAjustada = GetTaxaCondensada(cotacao, produto, taxaNivel);
        var premioTotal = lmiProducaoTotal * (taxaAjustada / 100);

        if (produto.Replantio == "Cobertura adicional")
            premioTotal += lmiReplantioTotal * (produto.TaxaCoberturaAdicional ?? 0) / 100;

        var subvencaoFederal = CalcularSubvencaoFederalComPremio(produto, cotacao, premioTotal);
        var subvencaoEstadual = CalcularSubvencaoEstadualComPremio(produto, cotacao, premioTotal);
        var parcelaSegurado = premioTotal - subvencaoFederal - subvencaoEstadual;

        var custoHectare = parcelaSegurado / cotacao.AreaTotal;
        var precoSaca = cotacao.PrecoSaca ?? 0;
        var sacasPorHa = precoSaca > 0 ? custoHectare / precoSaca : 0;
        var sacasPorAlqueire = precoSaca > 0 ? (custoHectare * 2.42m) / precoSaca : 0;

        return (produtividadeEsperada, produtividadeSegurada,
            lmiProducaoHectare, lmiReplantioHectare,
            lmiProducaoTotal, lmiReplantioTotal,
            premioTotal, subvencaoFederal, subvencaoEstadual,
            parcelaSegurado, custoHectare, sacasPorHa, sacasPorAlqueire);
    }

    private static decimal CalcularProdutividadeEsperada(CotacoesAgricolaEntity cotacao, ProdutosEntity produto, ProdutosTaxasEntity taxa)
    {
        var prodEsperada = taxa.ProdutividadeEsperada;

        if (produto.AceitaPlantioConsorciado && cotacao.PlantioConsorciado)
            prodEsperada *= produto.PlantioConsorciadoAjusteProdutividade;

        if (produto.AceitaPlantioConvencional && cotacao.PlantioDireto)
            prodEsperada *= produto.PlantioConvencionalAjusteProdutividade;

        if (produto.LavouraIrrigada && cotacao.LavouraIrrigada)
            prodEsperada *= produto.LavouraIrrigadaAjusteProdutividade;

        if (produto.AceitaPlantioPosCanaDeAcucar && cotacao.PosCana)
            prodEsperada *= produto.PlantioPosCanaAjusteProdutividade;

        return prodEsperada;
    }

    private static decimal GetTaxaCondensada(CotacoesAgricolaEntity cotacao, ProdutosEntity produto, decimal taxaNivel)
    {
        decimal fatorAjuste = 1.0m;

        if (produto.AceitaPlantioConsorciado && cotacao.PlantioConsorciado)
            fatorAjuste *= produto.PlantioConsorciadoAjusteTaxa;

        if (produto.AceitaPlantioConvencional && cotacao.PlantioDireto)
            fatorAjuste *= produto.PlantioConvencionalAjusteTaxa;

        if (produto.LavouraIrrigada && cotacao.LavouraIrrigada)
            fatorAjuste *= produto.LavouraIrrigadaAjusteTaxa;

        if (produto.AceitaPlantioPosCanaDeAcucar && cotacao.PosCana)
            fatorAjuste *= produto.PlantioPosCanaAjusteTaxa;

        return taxaNivel * fatorAjuste;
    }

    private decimal CalcularSubvencaoFederalComPremio(ProdutosEntity produto, CotacoesAgricolaEntity cotacao, decimal premioTotal)
    {
        _logger.LogDebug("CalcularSubvencaoFederal: premioTotal={PremioTotal}", premioTotal);

        if (!cotacao.SubvencaoFederal || produto.SubvencaoFederal == null)
        {
            _logger.LogDebug("SubvencaoFederal nula ou desmarcada na cotação");
            return 0;
        }

        _logger.LogDebug("Porentagem: {Porentagem}", produto.SubvencaoFederal.Porentagem);
        var subvencaoCalculada = premioTotal * (produto.SubvencaoFederal.Porentagem / 100);
        // Aplicar limite: o que ultrapassar LimiteReal fica na parcela do segurado
        var subvencao = subvencaoCalculada > produto.SubvencaoFederal.LimiteReal
            ? produto.SubvencaoFederal.LimiteReal
            : subvencaoCalculada;
        _logger.LogDebug("Subvencao calculada: {SubvencaoCalculada}, LimiteReal: {LimiteReal}, Total aplicada: {Subvencao}",
            subvencaoCalculada, produto.SubvencaoFederal.LimiteReal, subvencao);

        return subvencao;
    }

    private decimal CalcularSubvencaoEstadualComPremio(ProdutosEntity produto, CotacoesAgricolaEntity cotacao, decimal premioTotal)
    {
        _logger.LogDebug("CalcularSubvencaoEstadual: premioTotal={PremioTotal}, UF={UF}", premioTotal, cotacao.Estado);

        if (!cotacao.SubvencaoEstadual)
        {
            _logger.LogDebug("SubvencaoEstadual desmarcada na cotação");
            return 0;
        }

        // Subvenção do estado do cliente/cotação: usa lista N:N ou fallback no vínculo antigo
        var subvencaoEstadual = produto.ProdutosSubvencoesEstaduais?
            .FirstOrDefault(ps => ps.SubvencaoEstadual?.Estado == cotacao.Estado)
            ?.SubvencaoEstadual
            ?? (produto.SubvencaoEstadual?.Estado == cotacao.Estado ? produto.SubvencaoEstadual : null);

        if (subvencaoEstadual == null)
        {
            _logger.LogDebug("Nenhuma subvenção estadual para UF {UF}", cotacao.Estado);
            return 0;
        }

        _logger.LogDebug("Porentagem: {Porentagem}, SubvencaoEstadual UF: {UF}", subvencaoEstadual.Porentagem, subvencaoEstadual.Estado);
        var subvencaoCalculada = premioTotal * (subvencaoEstadual.Porentagem / 100);
        // Aplicar limite: o que ultrapassar LimiteReal fica na parcela do segurado
        var subvencao = subvencaoCalculada > subvencaoEstadual.LimiteReal
            ? subvencaoEstadual.LimiteReal
            : subvencaoCalculada;
        _logger.LogDebug("Subvencao calculada: {SubvencaoCalculada}, LimiteReal: {LimiteReal}, Total aplicada: {Subvencao}",
            subvencaoCalculada, subvencaoEstadual.LimiteReal, subvencao);

        return subvencao;
    }

    public async Task<CotacaoAgricolaPdfDTO?> ObterCotacaoPdf(Guid cotacaoId)
    {
        var cotacao = await _context.CotacoesAgricola
            .Include(c => c.Usuario)
            .Include(c => c.Cliente)
            .Include(c => c.Safra)
            .Include(c => c.Cultura)
            .Include(c => c.CotacoesAgricolaTipoSolo)
            .Include(c => c.CotacoesAgricolaClassificacaoSolo)
            .Include(c => c.Canal)
            .Include(c => c.PontoAtendimento)
            .FirstOrDefaultAsync(c => c.Id == cotacaoId && !c.Excluido);

        if (cotacao == null) return null;

        var corretora = await _context.CorretoraEntity.FirstOrDefaultAsync(x => x.Id == cotacao.CorretoraId);
        if (corretora == null) return null;

        var coberturas = await ObterDadosPropostaCotacao(cotacaoId);

        return new CotacaoAgricolaPdfDTO
        {
            CotacaoAgricolaId = cotacaoId,
            DataCotacao = cotacao.DataCotacao ?? DateTime.UtcNow,
            NumeroCotacao = cotacao.CodigoCotacao ?? string.Empty,
            Corretora = corretora.RazaoSocial,
            Canal = cotacao.Canal?.RazaoSocial ?? string.Empty,
            PA = cotacao.PontoAtendimento?.RazaoSocial ?? string.Empty,
            Consultor = cotacao.Usuario?.Name ?? string.Empty,
            Contato = cotacao.Usuario?.PhoneNumber ?? string.Empty,
            Email = cotacao.Usuario?.Email ?? string.Empty,
            Cultura = cotacao.Cultura?.Nome ?? string.Empty,
            Safra = cotacao.Safra?.AnoReferencia ?? string.Empty,
            UF = cotacao.Estado ?? string.Empty,
            Municipio = cotacao.Municipio ?? string.Empty,
            CPF = cotacao.Cliente?.CPF ?? cotacao.ClienteCPF ?? string.Empty,
            NomeCliente = cotacao.Cliente?.Nome ?? cotacao.ClienteNome ?? string.Empty,
            AreaTotal = cotacao.AreaTotal.ToString(),
            TipoSolo = string.Join(", ", cotacao.CotacoesAgricolaTipoSolo?.Select(x => $"Tipo {x.TipoSolo}") ?? new List<string>()),
            ClassificacaoSolo = string.Join(", ", cotacao.CotacoesAgricolaClassificacaoSolo?.Select(x => x.ClassificacaoSolo) ?? new List<string>()),
            PlantioConsorciado = cotacao.PlantioConsorciado ? "Sim" : "Não",
            PlantioDireto = cotacao.PlantioDireto ? "Sim" : "Não",
            PlantioPosCanal = cotacao.PosCana ? "Sim" : "Não",
            LavouraIrrigada = cotacao.LavouraIrrigada ? "Sim" : "Não",
            IsModalidadeProdutividade = cotacao.IsModalidadeProdutividade,
            Modalidade = cotacao.IsModalidadeProdutividade ? "Produtividade" : "Custeio Agrícola",
            PrecoSaca = cotacao.PrecoSaca?.ToString("C2") ?? string.Empty,
            CusteioHa = cotacao.ValorCusteio?.ToString("C2") ?? string.Empty,
            Coberturas = coberturas.Coberturas.Select(c => new CoberturaPdfDTO
            {
                TipoOferta = c.TipoOferta,
                Seguradora = c.Seguradora,
                NomeProduto = c.NomeProduto,
                RegulacaoSinistro = c.RegulacaoSinistro,
                ProdutividadeEsperada = c.ProdutividadeEsperada,
                CotacaoAgricolaPropostaId = c.CotacaoAgricolaPropostaId ?? Guid.Empty,
                NivelCobertura = c.NivelCobertura,
                ProdutividadeSegurada = c.ProdutividadeSegurada,
                LMIProducaoHectare = c.LMIProducaoHectare,
                LMIReplantioHectare = c.LMIReplantioHectare,
                LMIProducaoTotal = c.LMIProducaoTotal,
                LMIReplantioTotal = c.LMIReplantioTotal,
                PremioTotal = c.PremioTotal,
                SubvencaoFederal = c.SubvencaoFederal,
                SubvencaoEstadual = c.SubvencaoEstadual,
                ParcelaSegurado = c.ParcelaSegurado,
                CustoHectare = c.CustoHectare,
                SacasPorHa = c.SacasPorHa,
                SacasPorAlqueire = c.SacasPorAlqueire,
                TiposSoloProduto = c.TiposSoloProduto
            }).ToList()
        };
    }

    private async Task<List<MotorCotacaoPassoDiagnosticoDTO>> DiagnosticarPassoAPasso(CotacoesAgricolaEntity cotacao)
    {
        var passos = new List<MotorCotacaoPassoDiagnosticoDTO>();
        var culturaNome = await _context.Culturas.Where(c => c.Id == cotacao.CulturaId).Select(c => c.Nome).FirstOrDefaultAsync() ?? "(não informada)";
        var safraDesc = await _context.Safras.Where(s => s.Id == cotacao.SafraId).Select(s => s.AnoReferencia).FirstOrDefaultAsync() ?? "(não informada)";

        var produtosCulturaSafra = await _context.Produtos
            .Include(p => p.ProdutosCanalPontoAtendimento)
            .Include(p => p.Taxas)
            .Where(x => x.Ativo && !x.Excluido && x.CulturaId == cotacao.CulturaId && x.SafraId == cotacao.SafraId)
            .ToListAsync();

        passos.Add(new MotorCotacaoPassoDiagnosticoDTO
        {
            Etapa = "Cultura / Safra",
            Sucesso = produtosCulturaSafra.Any(),
            Mensagem = produtosCulturaSafra.Any()
                ? $"{produtosCulturaSafra.Count} produto(s) encontrado(s) para {culturaNome} / safra {safraDesc}."
                : $"Nenhum produto cadastrado para {culturaNome} / safra {safraDesc}.",
            Dica = !produtosCulturaSafra.Any() ? "Verifique se existem produtos cadastrados para essa cultura e safra." : null
        });

        if (!produtosCulturaSafra.Any()) return passos;

        var produtosCanalPA = produtosCulturaSafra.Where(p =>
            p.ProdutosCanalPontoAtendimento.Any(c =>
                (!cotacao.CanalId.HasValue || c.CanalId == cotacao.CanalId) &&
                (!cotacao.PontoAtendimentoId.HasValue || c.PontoAtendimentoId == cotacao.PontoAtendimentoId)))
            .ToList();

        var excluidosCanalPA = produtosCulturaSafra.Except(produtosCanalPA).Select(p => new ProdutoExcluidoDiagnosticoDTO
        {
            NomeProduto = p.NomeProduto,
            Motivo = "Não vinculado ao Canal/PA selecionado"
        }).ToList();

        passos.Add(new MotorCotacaoPassoDiagnosticoDTO
        {
            Etapa = "Canal / Ponto de Atendimento",
            Sucesso = produtosCanalPA.Any(),
            Mensagem = produtosCanalPA.Any()
                ? $"{produtosCanalPA.Count} produto(s) vinculado(s) ao Canal/PA selecionado."
                : "Nenhum produto vinculado ao Canal e Ponto de Atendimento selecionados.",
            Dica = !produtosCanalPA.Any() ? "Verifique se os produtos estão vinculados ao Canal e PA corretos." : null,
            ProdutosExcluidos = excluidosCanalPA.Any() ? excluidosCanalPA : null
        });

        var produtosArea = produtosCanalPA.Where(p => p.AreaMinimaTotal <= cotacao.AreaTotal).ToList();
        var excluidosArea = produtosCanalPA.Except(produtosArea).Select(p => new ProdutoExcluidoDiagnosticoDTO
        {
            NomeProduto = p.NomeProduto,
            Motivo = $"Área mínima {p.AreaMinimaTotal} ha > informada {cotacao.AreaTotal} ha"
        }).ToList();
        var areasMinMax = produtosCanalPA.Any() ? $"Faixa aceita: {produtosCanalPA.Min(p => p.AreaMinimaTotal)} a {produtosCanalPA.Max(p => p.AreaMinimaTotal)} ha." : "";

        passos.Add(new MotorCotacaoPassoDiagnosticoDTO
        {
            Etapa = "Área Total",
            Sucesso = produtosArea.Any(),
            Mensagem = produtosArea.Any()
                ? $"Área informada ({cotacao.AreaTotal} ha) atende ao mínimo de {produtosArea.Count} produto(s)."
                : produtosCanalPA.Any()
                    ? $"Área informada ({cotacao.AreaTotal} ha) não atende ao mínimo. {areasMinMax}"
                    : "Sem produtos para verificar área.",
            Dica = !produtosArea.Any() && produtosCanalPA.Any() ? "Ajuste a Área Total para dentro da faixa aceita pelos produtos." : null,
            ProdutosExcluidos = excluidosArea.Any() ? excluidosArea : null
        });

        var produtosModalidade = produtosArea.Where(p =>
        {
            if (cotacao.IsModalidadeProdutividade)
                return p.Modalidade == "Produtividade" && cotacao.PrecoSaca.HasValue &&
                       p.ValorSacaMinimo <= cotacao.PrecoSaca.Value && p.ValorSacaMaximo >= cotacao.PrecoSaca.Value;
            else
                return p.Modalidade == "Custeio" && cotacao.ValorCusteio.HasValue &&
                       p.ValorCusteioMinimo <= cotacao.ValorCusteio.Value && p.ValorCusteioMaximo >= cotacao.ValorCusteio.Value;
        }).ToList();

        var modalidadeTxt = cotacao.IsModalidadeProdutividade ? "Produtividade" : "Custeio";
        string? msgModalidade = null;
        if (cotacao.IsModalidadeProdutividade && (!cotacao.PrecoSaca.HasValue || cotacao.PrecoSaca.Value <= 0))
            msgModalidade = "Preço da Saca não informado ou inválido.";
        else if (!cotacao.IsModalidadeProdutividade && (!cotacao.ValorCusteio.HasValue || cotacao.ValorCusteio.Value <= 0))
            msgModalidade = "Valor de Custeio não informado ou inválido.";

        var excluidosModalidade = new List<ProdutoExcluidoDiagnosticoDTO>();
        foreach (var p in produtosArea.Except(produtosModalidade))
        {
            if (p.Modalidade != modalidadeTxt)
                excluidosModalidade.Add(new() { NomeProduto = p.NomeProduto, Motivo = $"Produto é {p.Modalidade}, cotação usa {modalidadeTxt}" });
            else if (cotacao.IsModalidadeProdutividade && cotacao.PrecoSaca.HasValue)
                excluidosModalidade.Add(new() { NomeProduto = p.NomeProduto, Motivo = $"Preço Saca ({cotacao.PrecoSaca}) fora do range ({p.ValorSacaMinimo}–{p.ValorSacaMaximo})" });
            else if (!cotacao.IsModalidadeProdutividade && cotacao.ValorCusteio.HasValue)
                excluidosModalidade.Add(new() { NomeProduto = p.NomeProduto, Motivo = $"Custeio ({cotacao.ValorCusteio}) fora do range ({p.ValorCusteioMinimo}–{p.ValorCusteioMaximo})" });
        }

        if (msgModalidade != null)
        {
            passos.Add(new MotorCotacaoPassoDiagnosticoDTO
            {
                Etapa = "Modalidade / Preço",
                Sucesso = false,
                Mensagem = msgModalidade,
                Dica = cotacao.IsModalidadeProdutividade ? "Informe o Preço da Saca (obrigatório para modalidade Produtividade)." : "Informe o Valor de Custeio (obrigatório para modalidade Custeio)."
            });
        }
        else
        {
            string? dicaPreco = null;
            if (!produtosModalidade.Any() && produtosArea.Any())
            {
                if (cotacao.IsModalidadeProdutividade)
                {
                    var faixas = produtosArea.Where(p => p.Modalidade == "Produtividade").ToList();
                    if (faixas.Any())
                        dicaPreco = $"Preço da Saca informado ({cotacao.PrecoSaca}) não está dentro da faixa dos produtos ({faixas.Min(p => p.ValorSacaMinimo)} a {faixas.Max(p => p.ValorSacaMaximo)}). Ajuste o Preço da Saca.";
                    else
                        dicaPreco = "Nenhum produto Produtividade encontrado. Verifique a modalidade.";
                }
                else
                {
                    var faixas = produtosArea.Where(p => p.Modalidade == "Custeio").ToList();
                    if (faixas.Any())
                        dicaPreco = $"Valor de Custeio informado ({cotacao.ValorCusteio}) não está dentro da faixa dos produtos ({faixas.Min(p => p.ValorCusteioMinimo)} a {faixas.Max(p => p.ValorCusteioMaximo)}). Ajuste o Valor de Custeio.";
                    else
                        dicaPreco = "Nenhum produto Custeio encontrado. Verifique a modalidade.";
                }
            }
            passos.Add(new MotorCotacaoPassoDiagnosticoDTO
            {
                Etapa = $"Modalidade ({modalidadeTxt}) / Preço",
                Sucesso = produtosModalidade.Any(),
                Mensagem = produtosModalidade.Any()
                    ? $"{produtosModalidade.Count} produto(s) com a modalidade e preço compatíveis."
                    : $"Nenhum produto aceita {modalidadeTxt} com o preço informado.",
                Dica = dicaPreco,
                ProdutosExcluidos = excluidosModalidade.Any() ? excluidosModalidade : null
            });
        }

        var tiposSoloSelecionados = cotacao.CotacoesAgricolaTipoSolo?.Select(a => a.TipoSolo switch
        {
            1 => "Tipo1", 2 => "Tipo2", 3 => "Tipo3", _ => null
        }).Where(t => t != null).ToList() ?? new List<string?>();

        var produtosSolo = produtosModalidade.Where(p =>
            !tiposSoloSelecionados.Any() || p.TipoSolo.Any(t => tiposSoloSelecionados.Contains(t)))
            .ToList();

        var tiposSoloAceitosProdutos = produtosModalidade.SelectMany(p => p.TipoSolo ?? Array.Empty<string>()).Distinct().ToList();

        var excluidosSolo = tiposSoloSelecionados.Any()
            ? produtosModalidade.Except(produtosSolo).Select(p => new ProdutoExcluidoDiagnosticoDTO
            {
                NomeProduto = p.NomeProduto,
                Motivo = $"Produto aceita {string.Join(", ", p.TipoSolo ?? Array.Empty<string>())}, cotação tem {string.Join(", ", tiposSoloSelecionados)}"
            }).ToList()
            : new List<ProdutoExcluidoDiagnosticoDTO>();

        passos.Add(new MotorCotacaoPassoDiagnosticoDTO
        {
            Etapa = "Tipo de Solo",
            Sucesso = produtosSolo.Any(),
            Mensagem = !tiposSoloSelecionados.Any()
                ? "Nenhum tipo de solo selecionado na cotação."
                : produtosSolo.Any()
                    ? $"Tipos selecionados ({string.Join(", ", tiposSoloSelecionados)}) compatíveis com {produtosSolo.Count} produto(s)."
                    : $"Tipos selecionados ({string.Join(", ", tiposSoloSelecionados)}) não são aceitos por nenhum produto.",
            Dica = !produtosSolo.Any() && tiposSoloAceitosProdutos.Any()
                ? $"Produtos aceitam: {string.Join(", ", tiposSoloAceitosProdutos)}. Selecione um tipo de solo compatível."
                : !tiposSoloSelecionados.Any()
                    ? $"Selecione ao menos um tipo de solo. Produtos aceitam: {string.Join(", ", tiposSoloAceitosProdutos)}."
                    : null,
            ProdutosExcluidos = excluidosSolo.Any() ? excluidosSolo : null
        });

        var classSoloSelecionadas = cotacao.CotacoesAgricolaClassificacaoSolo?.Select(a => a.ClassificacaoSolo?.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToList() ?? new List<string>();

        var produtosClassSolo = produtosSolo.Where(p =>
            !classSoloSelecionadas.Any() || p.ClassificacaoSolosAceitos.Any(c => classSoloSelecionadas.Contains(c)))
            .ToList();

        var classSoloAceitasProdutos = produtosSolo.SelectMany(p => p.ClassificacaoSolosAceitos ?? Array.Empty<string>()).Distinct().ToList();

        var excluidosClassSolo = classSoloSelecionadas.Any()
            ? produtosSolo.Except(produtosClassSolo).Select(p => new ProdutoExcluidoDiagnosticoDTO
            {
                NomeProduto = p.NomeProduto,
                Motivo = $"Produto aceita {string.Join(", ", p.ClassificacaoSolosAceitos ?? Array.Empty<string>())}, cotação tem {string.Join(", ", classSoloSelecionadas)}"
            }).ToList()
            : new List<ProdutoExcluidoDiagnosticoDTO>();

        passos.Add(new MotorCotacaoPassoDiagnosticoDTO
        {
            Etapa = "Classificação de Solo",
            Sucesso = produtosClassSolo.Any(),
            Mensagem = !classSoloSelecionadas.Any()
                ? "Nenhuma classificação de solo selecionada na cotação."
                : produtosClassSolo.Any()
                    ? $"Classificações selecionadas ({string.Join(", ", classSoloSelecionadas)}) compatíveis com {produtosClassSolo.Count} produto(s)."
                    : $"Classificações selecionadas ({string.Join(", ", classSoloSelecionadas)}) não são aceitas por nenhum produto.",
            Dica = !produtosClassSolo.Any() && classSoloAceitasProdutos.Any()
                ? $"Produtos aceitam: {string.Join(", ", classSoloAceitasProdutos)}. Selecione uma classificação compatível."
                : !classSoloSelecionadas.Any()
                    ? $"Selecione ao menos uma classificação de solo. Produtos aceitam: {string.Join(", ", classSoloAceitasProdutos)}."
                    : null,
            ProdutosExcluidos = excluidosClassSolo.Any() ? excluidosClassSolo : null
        });

        var municipioTrim = cotacao.Municipio?.Trim() ?? "";
        var produtosMunicipio = produtosClassSolo.Where(p =>
            !string.IsNullOrWhiteSpace(municipioTrim) &&
            p.Taxas.Any(t => t.UF == cotacao.Estado && (t.Municipio ?? "").Trim().Equals(municipioTrim, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var excluidosTaxa = !string.IsNullOrWhiteSpace(municipioTrim)
            ? produtosClassSolo.Except(produtosMunicipio).Select(p => new ProdutoExcluidoDiagnosticoDTO
            {
                NomeProduto = p.NomeProduto,
                Motivo = "Sem taxa cadastrada para " + cotacao.Municipio + "/" + cotacao.Estado
            }).ToList()
            : new List<ProdutoExcluidoDiagnosticoDTO>();

        passos.Add(new MotorCotacaoPassoDiagnosticoDTO
        {
            Etapa = "Município / Taxa",
            Sucesso = produtosMunicipio.Any(),
            Mensagem = string.IsNullOrWhiteSpace(cotacao.Municipio)
                ? "Município não informado."
                : produtosMunicipio.Any()
                    ? $"{produtosMunicipio.Count} produto(s) com taxa para {cotacao.Municipio}/{cotacao.Estado}."
                    : $"Nenhum dos {produtosClassSolo.Count} produto(s) restantes possui taxa para {cotacao.Municipio}/{cotacao.Estado}.",
            Dica = !produtosMunicipio.Any() && !string.IsNullOrWhiteSpace(cotacao.Municipio)
                ? $"Verifique se existe taxa cadastrada para {cotacao.Municipio}/{cotacao.Estado} nos produtos."
                : null,
            ProdutosExcluidos = excluidosTaxa.Any() ? excluidosTaxa : null
        });

        return passos;
    }

    private async Task<string> DiagnosticarSemPropostas(CotacoesAgricolaEntity cotacao)
    {
        var passos = await DiagnosticarPassoAPasso(cotacao);
        return string.Join(" | ", passos.Where(p => !p.Sucesso).Select(p => p.Mensagem));
    }

    private async Task<List<MotorCotacaoErroValidacaoDTO>> DiagnosticarErrosSemPropostas(CotacoesAgricolaEntity cotacao)
    {
        var passos = await DiagnosticarPassoAPasso(cotacao);
        return passos.Where(p => !p.Sucesso).Select(p => new MotorCotacaoErroValidacaoDTO
        {
            Campo = p.Etapa,
            Mensagem = p.Mensagem + (p.Dica != null ? $" Dica: {p.Dica}" : "")
        }).ToList();
    }

    private static string RenderPdfHtml(CotacaoAgricolaPdfDTO model)
    {
        var html = $@"<html><head>
<meta charset='utf-8'/>
<style>
  body {{ font-family: Arial, sans-serif; font-size: 12px; }}
  table {{ width: 100%; border-collapse: collapse; margin-bottom: 15px; }}
  th, td {{ border: 1px solid #333; padding: 6px; text-align: left; }}
  th {{ background-color: #f0f0f0; }}
  h4 {{ text-align: center; }}
  .label {{ font-weight: bold; width: 30%; }}
</style></head><body>
<h4>COTAÇÃO SEGURO AGRÍCOLA</h4>
<p><strong>Número:</strong> {model.NumeroCotacao} | <strong>Data:</strong> {model.DataCotacao:dd/MM/yyyy HH:mm}</p>

<h5>DADOS DO ATENDIMENTO</h5>
<table><tr><td class='label'>Corretora</td><td>{model.Corretora}</td><td class='label'>Consultor</td><td>{model.Consultor}</td></tr>
<tr><td class='label'>Canal</td><td>{model.Canal}</td><td class='label'>Contato</td><td>{model.Contato}</td></tr>
<tr><td class='label'>P.A.</td><td>{model.PA}</td><td class='label'>E-mail</td><td>{model.Email}</td></tr></table>

<h5>DADOS DO RISCO</h5>
<table><tr><td class='label'>Cultura</td><td>{model.Cultura}</td><td class='label'>Safra</td><td>{model.Safra}</td></tr>
<tr><td class='label'>UF</td><td>{model.UF}</td><td class='label'>Município</td><td>{model.Municipio}</td></tr>
<tr><td class='label'>CPF</td><td>{model.CPF}</td><td class='label'>Cliente</td><td>{model.NomeCliente}</td></tr>
<tr><td class='label'>Área Total</td><td>{model.AreaTotal} ha</td><td class='label'>Tipo Solo</td><td>{model.TipoSolo}</td></tr>
<tr><td class='label'>Class. Solo</td><td>{model.ClassificacaoSolo}</td><td></td><td></td></tr>
<tr><td class='label'>Plantio Consorciado</td><td>{model.PlantioConsorciado}</td><td class='label'>Plantio Convencional</td><td>{model.PlantioDireto}</td></tr>
<tr><td class='label'>Pós-Cana</td><td>{model.PlantioPosCanal}</td><td class='label'>Lavoura Irrigada</td><td>{model.LavouraIrrigada}</td></tr></table>

<h5>DADOS DO SEGURO</h5>
<table><tr><td class='label'>Modalidade</td><td>{model.Modalidade}</td><td class='label'>{(!model.IsModalidadeProdutividade ? "Custeio/ha" : "Preço Saca")}</td><td>{(model.IsModalidadeProdutividade ? model.PrecoSaca : model.CusteioHa)}</td></tr></table>";

        if (model.Coberturas.Any())
        {
            html += "<h5>COBERTURAS</h5><table><tr><th>Opção</th><th>Seguradora</th><th>Produto</th><th>Nível Cobertura</th><th>Prod. Segurada</th><th>LMI Produção Total</th><th>Prêmio Total</th><th>Parcela Segurado</th></tr>";
            var idx = 1;
            foreach (var c in model.Coberturas)
            {
                html += $"<tr><td>{idx}</td><td>{c.Seguradora}</td><td>{c.NomeProduto}</td><td>{c.NivelCobertura}%</td><td>{c.ProdutividadeSegurada:N0}</td><td>{c.LMIProducaoTotal:C2}</td><td>{c.PremioTotal}</td><td>{c.ParcelaSegurado}</td></tr>";
                idx++;
            }
            html += "</table>";
        }

        html += @"<p><em>Essa Pré-cotação está sujeita a sofrer alterações de acordo com as políticas de subscrição das seguradoras.</em></p>
<p><em>O aceite da proposta dependerá da oferta de capacidade e da análise técnica da seguradora para o referido risco.</em></p>
</body></html>";

        return html;
    }

    public async Task<List<ProdutoDisponivelDTO>> ObterProdutosDisponiveisAsync(Guid? culturaId, Guid? safraId, Guid? canalId, Guid? pontoAtendimentoId, string? estado = null, string? municipio = null, decimal areaTotal = 0)
    {
        if (!culturaId.HasValue || !safraId.HasValue)
            return new List<ProdutoDisponivelDTO>();

        var query = _context.Produtos
            .Include(p => p.Taxas)
            .Include(p => p.Cultura)
            .Include(p => p.Safra)
            .Include(p => p.ProdutosCanalPontoAtendimento)
            .Where(p => p.Ativo && !p.Excluido && p.CulturaId == culturaId.Value && p.SafraId == safraId.Value)
            .AsQueryable();

        if (canalId.HasValue && pontoAtendimentoId.HasValue)
        {
            query = query.Where(p => p.ProdutosCanalPontoAtendimento
                .Any(c => c.CanalId == canalId.Value && c.PontoAtendimentoId == pontoAtendimentoId.Value));
        }

        var produtos = await query.OrderBy(p => p.NomeProduto).ToListAsync();
        var municipioTrim = municipio?.Trim() ?? "";

        return produtos.Select(p => new ProdutoDisponivelDTO
        {
            ProdutoId = p.Id,
            NomeProduto = p.NomeProduto,
            Modalidade = p.Modalidade,
            Cultura = p.Cultura?.Nome ?? "",
            Safra = p.Safra?.AnoReferencia ?? "",
            TipoSolo = p.TipoSolo != null ? string.Join(", ", p.TipoSolo) : "",
            ClassificacaoSolo = p.ClassificacaoSolosAceitos != null ? string.Join(", ", p.ClassificacaoSolosAceitos) : "",
            ValorSacaMinimo = p.Modalidade == "Produtividade" ? p.ValorSacaMinimo : null,
            ValorSacaMaximo = p.Modalidade == "Produtividade" ? p.ValorSacaMaximo : null,
            ValorCusteioMinimo = p.Modalidade == "Custeio" ? p.ValorCusteioMinimo : null,
            ValorCusteioMaximo = p.Modalidade == "Custeio" ? p.ValorCusteioMaximo : null,
            AreaMinima = p.AreaMinimaTotal,
            QtdTaxas = p.Taxas.Count,
            VinculadoCanalPA = canalId.HasValue && pontoAtendimentoId.HasValue
                && p.ProdutosCanalPontoAtendimento.Any(c => c.CanalId == canalId.Value && c.PontoAtendimentoId == pontoAtendimentoId.Value),
            AreaAtende = areaTotal > 0 && p.AreaMinimaTotal <= areaTotal,
            TaxaMunicipio = !string.IsNullOrEmpty(estado) && !string.IsNullOrEmpty(municipioTrim)
                ? p.Taxas.FirstOrDefault(t => t.UF == estado && (t.Municipio ?? "").Trim().Equals(municipioTrim, StringComparison.OrdinalIgnoreCase))?.Municipio ?? ""
                : "",
            TaxaNc65 = !string.IsNullOrEmpty(estado) && !string.IsNullOrEmpty(municipioTrim)
                ? p.Taxas.Where(t => t.UF == estado && (t.Municipio ?? "").Trim().Equals(municipioTrim, StringComparison.OrdinalIgnoreCase)).Select(t => t.TaxaNc65).FirstOrDefault()
                : 0,
            TaxaNc70 = !string.IsNullOrEmpty(estado) && !string.IsNullOrEmpty(municipioTrim)
                ? p.Taxas.Where(t => t.UF == estado && (t.Municipio ?? "").Trim().Equals(municipioTrim, StringComparison.OrdinalIgnoreCase)).Select(t => t.TaxaNc70 ?? 0).FirstOrDefault()
                : 0,
            TaxaNc75 = !string.IsNullOrEmpty(estado) && !string.IsNullOrEmpty(municipioTrim)
                ? p.Taxas.Where(t => t.UF == estado && (t.Municipio ?? "").Trim().Equals(municipioTrim, StringComparison.OrdinalIgnoreCase)).Select(t => t.TaxaNc75 ?? 0).FirstOrDefault()
                : 0,
            CpfTaxa = !string.IsNullOrEmpty(estado) && !string.IsNullOrEmpty(municipioTrim)
                ? p.Taxas.Where(t => t.UF == estado && (t.Municipio ?? "").Trim().Equals(municipioTrim, StringComparison.OrdinalIgnoreCase)).Select(t => t.Cpf ?? "").FirstOrDefault() ?? ""
                : ""
        }).ToList();
    }
}
