using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MCR.API.CotacoesAgricola.Domain.DTO;
using MCR.API.Entities;
using MCR.API.Helpers;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Controllers
{
    [Authorize(Roles = "Administrador, Corretor, Consultor, Assistente, Gestor do Canal")]
    public class CotacaoAgricolaController : Controller
    {
        private readonly ICotacoesAgricolaService _cotacoesAgricolaService;
        private readonly ISafraService _safraService;
        private readonly ICulturaService _culturaService;
        private readonly ICorretoraService _corretoraService;
        private readonly ICanalService _canalService;
        private readonly IPontoAtendimentoService _pontoAtendimentoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CotacaoAgricolaController> _logger;
        private readonly DbContextMCR _context;

        public CotacaoAgricolaController(
            ICotacoesAgricolaService cotacoesAgricolaService,
            ISafraService safraService,
            ICulturaService culturaService,
            ICorretoraService corretoraService,
            ICanalService canalService,
            IPontoAtendimentoService pontoAtendimentoService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CotacaoAgricolaController> logger,
            DbContextMCR context)
        {
            _cotacoesAgricolaService = cotacoesAgricolaService;
            _safraService = safraService;
            _culturaService = culturaService;
            _corretoraService = corretoraService;
            _canalService = canalService;
            _pontoAtendimentoService = pontoAtendimentoService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string codigoCotacao, string safra, string cultura,
            string municipio, string dataCotacao, string cpfCliente,
            int page = 1, int pageSize = 10)
        {
            DateTime? data = null;
            if (DateTime.TryParse(dataCotacao, out var parsed))
                data = parsed;

            var retorno = (await _cotacoesAgricolaService.ObterTodosPaginadoAsync(
                codigoCotacao, null, null, municipio, data, cpfCliente, page, pageSize)).ToList();

            ViewBag.PesquisaCodigoCotacao = codigoCotacao;
            ViewBag.PesquisaSafra = safra;
            ViewBag.PesquisaCultura = cultura;
            ViewBag.PesquisaMunicipio = municipio;
            ViewBag.PesquisaDataCotacao = dataCotacao;
            ViewBag.PesquisaCpf = cpfCliente;
            ViewBag.PaginaAtual = page;
            ViewBag.TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0;
            ViewBag.UserRole = _httpContextAccessor.GetRole();

            var safras = await _safraService.ObterTodosPaginadoAsync(null, null, null, 1, 100);
            ViewBag.Safra = safras.Select(s => new SelectListItem
            {
                Text = $"{s.Descricao} - {s.AnoReferencia}",
                Value = $"{s.Descricao} - {s.AnoReferencia}"
            }).ToList();

            var culturas = await _culturaService.ObterTodosPaginadoAsync(null, null, null, null, 1, 100);
            ViewBag.Cultura = culturas.Select(c => new SelectListItem
            {
                Text = c.Nome,
                Value = c.Nome
            }).ToList();

            return View(retorno);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            await CarregarDropDowns();
            return View(new CotacoesAgricolaCadastrarDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(CotacoesAgricolaCadastrarDTO model)
        {
            if (!ModelState.IsValid)
            {
                var erros = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = erros });
            }

            try
            {
                var entity = model.ToEntity();
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                    entity.UsuarioId = userId;
                entity.Ativo = true;
                entity.Excluido = false;
                entity.DataCotacao = DateTime.UtcNow;
                entity.Status = "Em Andamento";

                // Validar que o ClienteId existe no banco (se informado)
                if (entity.ClienteId.HasValue)
                {
                    var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == entity.ClienteId.Value && !c.Excluido);
                    if (!clienteExiste)
                    {
                        return Json(new { success = false, message = "Cliente não encontrado. Verifique o CPF informado e tente novamente." });
                    }
                }

                var result = await _cotacoesAgricolaService.CadastrarAsync(entity);
                return Json(new
                {
                    success = result?.Sucesso ?? false,
                    message = result?.Mensagem ?? "Erro ao cadastrar cotação.",
                    cotacaoId = result?.Id.ToString(),
                    motorDiagnostico = result?.MotorDiagnostico,
                    errosValidacao = result?.MotorDiagnostico?.ErrosValidacao
                });
            }
            catch (DbUpdateException dbEx)
            {
                return Json(new { success = false, message = $"Erro de banco: {dbEx.InnerException?.Message ?? dbEx.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            await CarregarDropDowns();
            var cotacao = await _cotacoesAgricolaService.ObterPorIdAsync(id);
            if (cotacao == null || !cotacao.Sucesso)
            {
                TempData["Erro"] = cotacao?.Mensagem ?? "Cotação não encontrada.";
                return RedirectToAction(nameof(Index));
            }
            var model = CotacoesAgricolaCadastrarDTO.FromEntity(cotacao);
            ViewBag.UserRole = _httpContextAccessor.GetRole();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(CotacoesAgricolaCadastrarDTO model)
        {
            _logger.LogInformation("[GERENCIAR] PlantioConsorciado={PC}, LavouraIrrigada={LI}, PlantioDireto={PD}, PosCana={PosCana}, SubvFederal={SF}, SubvEstadual={SE}, TipoSolo={TS}, ClassSolo={CS}",
                model.PlantioConsorciado, model.LavouraIrrigada, model.PlantioDireto, model.PosCana,
                model.SubvencaoFederal, model.SubvencaoEstadual,
                model.TipoSolo, model.ClassificacaoSolo);

            _logger.LogInformation("[GERENCIAR-FIELDS] Id={Id}, CulturaId={CulturaId}, SafraId={SafraId}, AreaTotal={AreaTotal}, PrecoSaca={PrecoSaca}, ValorCusteio={ValorCusteio}, IsModalidade={IsModalidade}",
                model.Id, model.CulturaId, model.SafraId, model.AreaTotal, model.PrecoSaca, model.ValorCusteio, model.IsModalidadeProdutividade);

            if (!ModelState.IsValid)
            {
                var erros = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = erros });
            }

            try
            {
                var entity = model.ToEntity();
                _logger.LogInformation("[GERENCIAR-ENTITY] Id={Id}, AreaTotal={AreaTotal}, CulturaId={CulturaId}, PrecoSaca={PrecoSaca}, ValorCusteio={ValorCusteio}",
                    entity.Id, entity.AreaTotal, entity.CulturaId, entity.PrecoSaca, entity.ValorCusteio);
                var result = await _cotacoesAgricolaService.AtualizarAsync(entity);
                return Json(new
                {
                    success = result?.Sucesso ?? false,
                    message = result?.Mensagem ?? "Erro ao atualizar cotação.",
                    cotacaoId = result?.Id.ToString(),
                    motorDiagnostico = result?.MotorDiagnostico,
                    errosValidacao = result?.MotorDiagnostico?.ErrosValidacao
                });
            }
            catch (DbUpdateException dbEx)
            {
                return Json(new { success = false, message = $"Erro de banco: {dbEx.InnerException?.Message ?? dbEx.Message}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var result = await _cotacoesAgricolaService.ExcluirAsync(id);
            if (result)
                TempData["Sucesso"] = "Cotação excluída com sucesso!";
            else
                TempData["Erro"] = "Erro ao excluir cotação.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarInsucesso(Guid id)
        {
            try
            {
                var result = await _cotacoesAgricolaService.RegistrarInsucessoAsync(id);
                return Json(new { success = result, message = result ? "Insucesso registrado com sucesso!" : "Erro ao registrar insucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reabrir(Guid id)
        {
            try
            {
                var result = await _cotacoesAgricolaService.ReabrirAsync(id);
                return Json(new { success = result, message = result ? "Cotação reaberta com sucesso!" : "Erro ao reabrir cotação." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterAcoes(Guid id)
        {
            var cotacao = await _cotacoesAgricolaService.ObterPorIdAsync(id);
            if (cotacao == null || !cotacao.Sucesso)
                return Json(new { acoes = new List<string>() });

            var role = _httpContextAccessor.GetRole();
            var acoes = await _cotacoesAgricolaService.ObterAcoesPorStatus(cotacao.Status ?? string.Empty, role);
            return Json(new { acoes });
        }

        [HttpGet]
        public async Task<IActionResult> GerarPDF(Guid id)
        {
            try
            {
                var cotacaoPdf = await _cotacoesAgricolaService.ObterCotacaoPdf(id);
                if (cotacaoPdf == null)
                {
                    TempData["Erro"] = "Cotação não encontrada.";
                    return RedirectToAction(nameof(Gerenciar), new { id });
                }
                return View(cotacaoPdf);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao gerar PDF. " + ex.Message;
                return RedirectToAction(nameof(Gerenciar), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPDF(Guid id)
        {
            try
            {
                var cotacaoPdf = await _cotacoesAgricolaService.ObterCotacaoPdf(id);
                if (cotacaoPdf == null)
                {
                    TempData["Erro"] = "Cotação não encontrada.";
                    return RedirectToAction(nameof(Gerenciar), new { id });
                }

                var pdfBytes = PdfGenerator.GerarPDF(cotacaoPdf);
                string fileName = $"Cotacao-{cotacaoPdf.NumeroCotacao}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao gerar PDF. " + ex.Message;
                return RedirectToAction(nameof(Gerenciar), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterDadosPropostaCotacao(Guid cotacaoId, string order = "segurada")
        {
            var result = await _cotacoesAgricolaService.ObterDadosPropostaCotacao(cotacaoId, order);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTiposSoloPorCulturaSafra(Guid culturaId, Guid safraId)
        {
            if (culturaId == Guid.Empty || safraId == Guid.Empty)
                return Json(Array.Empty<object>());
            var lista = await _cotacoesAgricolaService.ObterTiposSoloPorCulturaSafraAsync(culturaId, safraId);
            return Json(lista);
        }

        [HttpGet]
        public async Task<IActionResult> ObterClassificacoesSoloPorCulturaSafra(Guid culturaId, Guid safraId)
        {
            if (culturaId == Guid.Empty || safraId == Guid.Empty)
                return Json(Array.Empty<string>());
            var lista = await _cotacoesAgricolaService.ObterClassificacoesSoloPorCulturaSafraAsync(culturaId, safraId);
            return Json(lista);
        }

        [HttpGet]
        public async Task<IActionResult> ObterCanaisPorCorretora(Guid corretoraId)
        {
            if (corretoraId == Guid.Empty)
                return Json(Enumerable.Empty<object>());
            var canais = await _canalService.ObterPorCorretoraAsync(corretoraId);
            var lista = canais.Select(x => new { id = x.Id.ToString(), text = x.RazaoSocial }).ToList();
            return Json(lista);
        }

        [HttpGet]
        public async Task<IActionResult> CotacaoAgricolaDadosPropostas(Guid cotacaoId, string order = "segurada")
        {
            return ViewComponent("CotacaoAgricolaDadosPropostas", new { cotacaoId, order });
        }

        private async Task CarregarDropDowns()
        {
            var culturas = await _culturaService.ObterTodosPaginadoAsync(null, null, null, null, 1, 999);
            ViewBag.Cultura = new SelectList(culturas.Select(x => new { id = x.Id, text = x.Nome }).OrderBy(x => x.text).ToList(), "id", "text");

            var safras = await _safraService.ObterTodosPaginadoAsync(null, null, null, 1, 999);
            ViewBag.Safra = new SelectList(safras.Select(x => new { id = x.Id, text = $"{x.Descricao} - {x.AnoReferencia}" }).ToList(), "id", "text");

            var corretoras = await _corretoraService.ObterTodosAsync();
            ViewBag.Corretora = new SelectList(corretoras.Select(x => new { id = x.Id, text = x.NomeFantasia }).ToList(), "id", "text");
            ViewBag.CorretoraCount = corretoras.Count();

            var todosCanais = await _canalService.ObterTodosPaginadoAsync(null, null, 1, 999);
            ViewBag.Canal = new SelectList(todosCanais.Select(x => new { id = x.Id, text = x.RazaoSocial }).ToList(), "id", "text");
            ViewBag.CanalCount = todosCanais.Count();

            var todosPontos = await _pontoAtendimentoService.ObterTodosPaginadoAsync(null, null, 1, 999);
            ViewBag.PontoAtendimento = new SelectList(todosPontos.Select(x => new { id = x.Id, text = x.RazaoSocial }).ToList(), "id", "text");
            ViewBag.PACount = todosPontos.Count();
        }

        [HttpGet]
        public async Task<IActionResult> ProdutosDisponiveis(Guid? culturaId, Guid? safraId, Guid? canalId, Guid? pontoAtendimentoId, string? estado = null, string? municipio = null, decimal areaTotal = 0)
        {
            var produtos = await _cotacoesAgricolaService.ObterProdutosDisponiveisAsync(culturaId, safraId, canalId, pontoAtendimentoId, estado, municipio, areaTotal);
            return Json(produtos);
        }
    }
}
