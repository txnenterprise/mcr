using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Models.Dashboard;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly DbContextMCR _context;

        public DashboardService(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<string> GetUserLevelAsync(ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin"))
                return "admin";
            if (user.IsInRole("Corretor"))
                return "corretor";
            if (user.IsInRole("GestorCanal"))
                return "gestor";
            if (user.IsInRole("Consultor"))
                return "consultor";
            return "consultor";
        }

        public async Task<InitialDataResponse> GetInitialDataAsync(ClaimsPrincipal user)
        {
            var userLevel = await GetUserLevelAsync(user);
            var response = new InitialDataResponse();

            switch (userLevel)
            {
                case "admin":
                    response.Corretoras = await _context.CorretoraEntity
                        .Where(c => c.Ativa)
                        .Select(c => new SelectOption { Id = c.Id, Nome = c.NomeFantasia })
                        .ToListAsync();

                    response.Canais = await _context.Canais
                        .Where(c => c.Ativo)
                        .Select(c => new SelectOption { Id = c.Id, Nome = c.NomeFantasia })
                        .ToListAsync();

                    response.PontosAtendimento = await _context.PontosAtendimento
                        .Where(pa => pa.Ativo)
                        .Select(pa => new SelectOption { Id = pa.Id, Nome = pa.NomeFantasia })
                        .ToListAsync();
                    break;

                case "corretor":
                    var corretorId = GetUserId(user);
                    response.Canais = await _context.Canais
                        .Where(c => c.Ativo && c.CorretoraId == corretorId)
                        .Select(c => new SelectOption { Id = c.Id, Nome = c.NomeFantasia })
                        .ToListAsync();

                    var canalIdsCorretor = response.Canais.Select(c => c.Id).ToList();
                    response.PontosAtendimento = await _context.PontosAtendimento
                        .Where(pa => pa.Ativo && canalIdsCorretor.Contains(pa.CanalId))
                        .Select(pa => new SelectOption { Id = pa.Id, Nome = pa.NomeFantasia })
                        .ToListAsync();
                    break;

                case "gestor":
                    var gestorId = GetUserId(user);
                    response.Canais = await _context.Canais
                        .Where(c => c.Ativo && c.UsuarioEstruturaNegocio.Any(u => u.UsuarioId == gestorId))
                        .Select(c => new SelectOption { Id = c.Id, Nome = c.NomeFantasia })
                        .ToListAsync();

                    var canalIdsGestor = response.Canais.Select(c => c.Id).ToList();
                    response.PontosAtendimento = await _context.PontosAtendimento
                        .Where(pa => pa.Ativo && canalIdsGestor.Contains(pa.CanalId) &&
                               pa.UsuarioEstruturaNegocio.Any(u => u.UsuarioId == gestorId))
                        .Select(pa => new SelectOption { Id = pa.Id, Nome = pa.NomeFantasia })
                        .ToListAsync();
                    break;

                case "consultor":
                    var consultorId = GetUserId(user);
                    response.PontosAtendimento = await _context.UsuariosPontoAtendimento
                        .Where(u => u.UsuarioId == consultorId && u.Ativo)
                        .Select(u => new SelectOption { Id = u.PontoAtendimentoId, Nome = u.PontoAtendimento.NomeFantasia })
                        .ToListAsync();
                    break;
            }

            response.Culturas = await _context.Culturas
                .Where(c => c.Ativo)
                .Select(c => new SelectOption { Id = c.Id, Nome = c.Nome })
                .ToListAsync();

            response.Safras = await _context.Safras
                .Where(s => s.Ativo)
                .Select(s => new SelectOption { Id = s.Id, Nome = s.Descricao })
                .ToListAsync();

            response.Seguradoras = await _context.SeguradoraEntity
                .Where(s => s.Ativo)
                .Select(s => new SelectOption { Id = s.Id, Nome = s.NomeFantasia })
                .ToListAsync();

            return response;
        }

        public async Task<List<SelectOption>> GetPontosAtendimentoAsync(List<Guid> canalIds, ClaimsPrincipal user)
        {
            if (canalIds == null || !canalIds.Any())
                return new List<SelectOption>();

            var userLevel = await GetUserLevelAsync(user);
            var query = _context.PontosAtendimento
                .Where(pa => pa.Ativo && canalIds.Contains(pa.CanalId));

            switch (userLevel)
            {
                case "gestor":
                    var gestorId = GetUserId(user);
                    query = query.Where(pa => pa.UsuarioEstruturaNegocio.Any(u => u.UsuarioId == gestorId));
                    break;
                case "consultor":
                    var consultorId = GetUserId(user);
                    query = query.Where(pa => pa.UsuarioPontoAtendimento.Any(u => u.UsuarioId == consultorId));
                    break;
            }

            return await query
                .Select(pa => new SelectOption { Id = pa.Id, Nome = pa.NomeFantasia })
                .ToListAsync();
        }

        public async Task<DashboardDataResponse> GetDashboardDataAsync(DashboardFilterRequest request, ClaimsPrincipal user)
        {
            var response = new DashboardDataResponse();

            var query = _context.Propostas
                .Where(p => p.Ativo && !p.Excluido)
                .Include(p => p.PropostasProdutos)
                    .ThenInclude(pp => pp.Seguradora)
                .Include(p => p.Cultura)
                .Include(p => p.Safra)
                .Include(p => p.Canal)
                .Include(p => p.PontoAtendimento)
                .Include(p => p.Corretora)
                .AsQueryable();

            if (request.CorretoraIds.Any())
                query = query.Where(p => p.CorretoraId.HasValue && request.CorretoraIds.Contains(p.CorretoraId.Value));

            if (request.CanalIds.Any())
                query = query.Where(p => p.CanalId.HasValue && request.CanalIds.Contains(p.CanalId.Value));

            if (request.PaIds.Any())
                query = query.Where(p => p.PontoAtendimentoId.HasValue && request.PaIds.Contains(p.PontoAtendimentoId.Value));

            if (request.CulturaIds.Any())
                query = query.Where(p => request.CulturaIds.Contains(p.CulturaId));

            if (request.SafraIds.Any())
                query = query.Where(p => request.SafraIds.Contains(p.SafraId));

            if (request.StatusInicial.Any())
            {
                var mappedStatusInicial = MapStatusValues(request.StatusInicial);
                query = query.Where(p => !string.IsNullOrEmpty(p.Status) && mappedStatusInicial.Contains(p.Status));
            }

            if (request.StatusFinal.Any())
            {
                var mappedStatusFinal = MapStatusValues(request.StatusFinal);
                query = query.Where(p => !string.IsNullOrEmpty(p.Status) && mappedStatusFinal.Contains(p.Status));
            }

            if (request.SeguradoraIds.Any())
                query = query.Where(p => p.PropostasProdutos.Any(pp => request.SeguradoraIds.Contains(pp.SeguradoraId)));

            var propostas = await query.ToListAsync();

            response.StatusData = ProcessStatusData(propostas, request.UserLevel);
            response.Subtotal = CalculateSubtotal(response.StatusData);
            response.Area = ProcessAreaChartData(propostas);
            response.Seguradora = ProcessSeguradoraChartData(propostas);
            response.Status = ProcessStatusChartData(propostas);

            return response;
        }

        private static decimal CalcularTaxaPremioSobreLmi(decimal premioTotal, decimal lmiTotal)
        {
            if (lmiTotal <= 0) return 0m;
            return premioTotal / lmiTotal * 100m;
        }

        private Dictionary<string, StatusData> ProcessStatusData(List<PropostasEntity> propostas, string userLevel)
        {
            var statusData = new Dictionary<string, StatusData>();
            var propostasPorStatus = propostas
                .GroupBy(p => p.Status)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var statusGroup in propostasPorStatus)
            {
                var propostasStatus = statusGroup.Value;
                var totalLMI = propostasStatus.Sum(p => p.PropostasProdutos.Sum(pp => pp.LMIProducaoTotal + pp.LMIReplantioTotal));
                var totalPremio = propostasStatus.Sum(p => p.PropostasProdutos.Sum(pp => pp.PremioTotal));
                var totalArea = propostasStatus.Sum(p => p.AreaTotal);

                statusData[statusGroup.Key] = new StatusData
                {
                    Area = totalArea,
                    Qtd = propostasStatus.Count,
                    LMI = totalLMI,
                    Premio = totalPremio,
                    Produtividade = propostasStatus.Any() && propostasStatus.Any(p => p.PropostasProdutos.Any())
                        ? propostasStatus.Where(p => p.PropostasProdutos.Any()).Average(p => p.PropostasProdutos.Average(pp => pp.LMIProducaoHectare))
                        : 0,
                    Taxa = CalcularTaxaPremioSobreLmi(totalPremio, totalLMI)
                };
            }

            return statusData;
        }

        private SubtotalData CalculateSubtotal(Dictionary<string, StatusData> statusData)
        {
            if (!statusData.Any()) return new SubtotalData();

            var sumLmi = statusData.Values.Sum(s => s.LMI);
            var sumPremio = statusData.Values.Sum(s => s.Premio);

            return new SubtotalData
            {
                Area = statusData.Values.Sum(s => s.Area),
                Qtd = statusData.Values.Sum(s => s.Qtd),
                LMI = sumLmi,
                Premio = sumPremio,
                Produtividade = statusData.Values.Where(s => s.Produtividade > 0).Any()
                    ? statusData.Values.Where(s => s.Produtividade > 0).Average(s => s.Produtividade)
                    : 0,
                Taxa = CalcularTaxaPremioSobreLmi(sumPremio, sumLmi)
            };
        }

        private static Dictionary<string, string> GetStatusMapping()
        {
            return new Dictionary<string, string>
            {
                { "aguardando_transmissao", "Aguardando transmissão" },
                { "cotacao_negociacao", "Cotação em negociação" },
                { "cotacao_realizada_sucesso", "Cotação realizada com sucesso" },
                { "proposta_andamento", "Proposta em andamento" },
                { "proposta_negociacao", "Proposta em negociação" },
                { "proposta_pendencia", "Proposta com pendência" },
                { "proposta_transmitida", "Proposta transmitida (Em análise)" },
                { "proposta_aceita", "Proposta aceita → Devolutiva da Seguradora" },
                { "apolice_emitida", "Apólice emitida" },
                { "solicitar_endosso", "Solicitar Endosso" },
                { "endosso_pendencia", "Endosso com Pendência" },
                { "endosso_transmitido", "Endosso Transmitido" },
                { "endosso_emitido", "Endosso Emitido" },
                { "proposta_recusada", "Proposta recusada → Devolutiva da Seguradora" },
                { "proposta_cancelada", "Proposta cancelada" },
                { "apolice_cancelada", "Apólice cancelada" },
                { "cotacao_encerrada", "Cotação encerrada sem sucesso" }
            };
        }

        private List<string> MapStatusValues(List<string> frontendStatusValues)
        {
            var statusMapping = GetStatusMapping();
            var mappedValues = new List<string>();

            foreach (var status in frontendStatusValues)
            {
                if (statusMapping.ContainsKey(status))
                    mappedValues.Add(statusMapping[status]);
                else
                    mappedValues.Add(status);
            }

            return mappedValues;
        }

        private ChartData ProcessAreaChartData(List<PropostasEntity> propostas)
        {
            var areaData = propostas
                .Where(p => p.Cultura != null)
                .GroupBy(p => p.Cultura.Nome)
                .Select(g => new { Cultura = g.Key, Area = g.Sum(p => p.AreaTotal) })
                .OrderByDescending(x => x.Area)
                .Take(10)
                .ToList();

            return new ChartData
            {
                Labels = areaData.Select(x => x.Cultura).ToList(),
                Values = areaData.Select(x => x.Area).ToList()
            };
        }

        private ChartData ProcessSeguradoraChartData(List<PropostasEntity> propostas)
        {
            var seguradoraData = propostas
                .SelectMany(p => p.PropostasProdutos)
                .Where(pp => pp.Seguradora != null)
                .GroupBy(pp => pp.Seguradora.NomeFantasia)
                .Select(g => new { Seguradora = g.Key, Area = g.Sum(pp => pp.Proposta.AreaTotal) })
                .OrderByDescending(x => x.Area)
                .Take(10)
                .ToList();

            return new ChartData
            {
                Labels = seguradoraData.Select(x => x.Seguradora).ToList(),
                Values = seguradoraData.Select(x => x.Area).ToList()
            };
        }

        private ChartData ProcessStatusChartData(List<PropostasEntity> propostas)
        {
            var statusData = propostas
                .Where(p => !string.IsNullOrEmpty(p.Status))
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key, Area = g.Sum(p => p.AreaTotal) })
                .ToList();

            return new ChartData
            {
                Labels = statusData.Select(x => x.Status).ToList(),
                Values = statusData.Select(x => x.Area).ToList()
            };
        }

        private static Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        public int GetTotalPropostas()
        {
            return _context.Propostas.Count();
        }

        public async Task<DashboardDataResponse> GetSinistroDashboardDataAsync(DashboardFilterRequest request, ClaimsPrincipal user)
        {
            var response = new DashboardDataResponse();
            var sinistroStatus = new[] { "comunicar_sinistro", "sinistro_em_analise", "sinistro_aprovado", "deferido_pago", "indeferido", "sinistro_cancelado" };

            var query = _context.Propostas
                .Where(p => p.Ativo && !p.Excluido && sinistroStatus.Contains(p.Status))
                .Include(p => p.PropostasProdutos).ThenInclude(pp => pp.Seguradora)
                .Include(p => p.Cultura).Include(p => p.Safra)
                .Include(p => p.Canal).Include(p => p.PontoAtendimento).Include(p => p.Corretora)
                .AsQueryable();

            if (request.CorretoraIds.Any())
                query = query.Where(p => p.CorretoraId.HasValue && request.CorretoraIds.Contains(p.CorretoraId.Value));
            if (request.CanalIds.Any())
                query = query.Where(p => p.CanalId.HasValue && request.CanalIds.Contains(p.CanalId.Value));
            if (request.PaIds.Any())
                query = query.Where(p => p.PontoAtendimentoId.HasValue && request.PaIds.Contains(p.PontoAtendimentoId.Value));
            if (request.CulturaIds.Any())
                query = query.Where(p => request.CulturaIds.Contains(p.CulturaId));
            if (request.SafraIds.Any())
                query = query.Where(p => request.SafraIds.Contains(p.SafraId));
            if (request.SeguradoraIds.Any())
                query = query.Where(p => p.PropostasProdutos.Any(pp => request.SeguradoraIds.Contains(pp.SeguradoraId)));

            var propostas = await query.ToListAsync();
            response.StatusData = ProcessStatusData(propostas, request.UserLevel);
            response.Subtotal = CalculateSubtotal(response.StatusData);
            response.Area = ProcessAreaChartData(propostas);
            response.Seguradora = ProcessSeguradoraChartData(propostas);
            response.Status = ProcessStatusChartData(propostas);

            return response;
        }

        public async Task<ResumoExecutivoResponse> GetResumoExecutivoAsync(ClaimsPrincipal user)
        {
            var statusesFinalizados = new[]
            {
                "Proposta aceita → Devolutiva da Seguradora",
                "Apólice emitida",
                "Proposta recusada → Devolutiva da Seguradora",
                "Proposta cancelada",
                "Apólice cancelada",
                "Cotação encerrada sem sucesso"
            };

            var hoje = DateTime.UtcNow.Date;

            return new ResumoExecutivoResponse
            {
                TotalClientes = await _context.Clientes.CountAsync(c => !c.Excluido),
                ClientesAtivos = await _context.Clientes.CountAsync(c => !c.Excluido && c.Ativo),
                TotalCorretoras = await _context.CorretoraEntity.CountAsync(),
                TotalCanais = await _context.Canais.CountAsync(),
                TotalPontosAtendimento = await _context.PontosAtendimento.CountAsync(),
                TotalPropostas = await _context.Propostas.CountAsync(p => p.Ativo && !p.Excluido),
                PropostasEmAndamento = await _context.Propostas.CountAsync(p =>
                    p.Ativo && !p.Excluido &&
                    !statusesFinalizados.Contains(p.Status)),
                CotacoesHoje = await _context.Propostas.CountAsync(p =>
                    p.Ativo && !p.Excluido &&
                    p.DataCotacao.HasValue && p.DataCotacao.Value.Date == hoje &&
                    p.Status == "Proposta em negociação"),
                TotalPremio = await _context.PropostasProdutos.SumAsync(pp => pp.PremioTotal),
                TotalLMI = await _context.PropostasProdutos.SumAsync(pp => pp.LMIProducaoTotal + pp.LMIReplantioTotal),
                TotalArea = await _context.Propostas.Where(p => p.Ativo && !p.Excluido).SumAsync(p => p.AreaTotal)
            };
        }

        public async Task<List<PerformanceCorretoraResponse>> GetPerformanceCorretoraAsync(DashboardFilterRequest request, ClaimsPrincipal user)
        {
            var query = _context.Propostas
                .Where(p => p.Ativo && !p.Excluido && p.Corretora != null)
                .Include(p => p.PropostasProdutos).Include(p => p.Corretora)
                .AsQueryable();

            if (request.CorretoraIds.Any())
                query = query.Where(p => p.CorretoraId.HasValue && request.CorretoraIds.Contains(p.CorretoraId.Value));
            if (request.SafraIds.Any())
                query = query.Where(p => request.SafraIds.Contains(p.SafraId));

            var propostas = await query.ToListAsync();

            return propostas
                .GroupBy(p => new { p.Corretora!.Id, p.Corretora.NomeFantasia })
                .Select(g => new PerformanceCorretoraResponse
                {
                    Nome = g.Key.NomeFantasia,
                    TotalPropostas = g.Count(),
                    TotalPremio = g.SelectMany(p => p.PropostasProdutos).Sum(pp => pp.PremioTotal),
                    TotalArea = g.Sum(p => p.AreaTotal),
                    TotalLMI = g.SelectMany(p => p.PropostasProdutos).Sum(pp => pp.LMIProducaoTotal + pp.LMIReplantioTotal),
                    TaxaConversao = g.Count() > 0 ? (decimal)g.Count(p => p.Status == "Apólice emitida") / g.Count() * 100 : 0
                })
                .OrderByDescending(x => x.TotalPremio)
                .ToList();
        }

        public async Task<EvolucaoTemporalResponse> GetEvolucaoTemporalAsync(DashboardFilterRequest request, ClaimsPrincipal user)
        {
            var query = _context.Propostas
                .Where(p => p.Ativo && !p.Excluido)
                .Include(p => p.PropostasProdutos)
                .AsQueryable();

            if (request.CulturaIds.Any())
                query = query.Where(p => request.CulturaIds.Contains(p.CulturaId));
            if (request.SafraIds.Any())
                query = query.Where(p => request.SafraIds.Contains(p.SafraId));

            var propostas = await query.ToListAsync();

            var grouped = propostas
                .Where(p => p.DataCotacao.HasValue)
                .GroupBy(p => p.DataCotacao!.Value.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Mes = g.Key,
                    Premio = g.SelectMany(p => p.PropostasProdutos).Sum(pp => pp.PremioTotal),
                    Qtd = g.Count(),
                    Area = g.Sum(p => p.AreaTotal)
                })
                .ToList();

            return new EvolucaoTemporalResponse
            {
                Meses = grouped.Select(g => g.Mes).ToList(),
                PremioPorMes = grouped.Select(g => g.Premio).ToList(),
                PropostasPorMes = grouped.Select(g => g.Qtd).ToList(),
                AreaPorMes = grouped.Select(g => g.Area).ToList()
            };
        }
    }
}
