using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Entities;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class CotacaoController : Controller
    {
        private readonly ICotacaoService _cotacaoService;
        private readonly IBancoService _bancoService;
        private readonly ICorretoraService _corretoraService;
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly IConfiguration _configuration;

        public CotacaoController(
            ICotacaoService cotacaoService,
            IBancoService bancoService,
            ICorretoraService corretoraService,
            UserManager<UsuarioEntity> userManager,
            IConfiguration configuration)
        {
            _cotacaoService = cotacaoService;
            _bancoService = bancoService;
            _corretoraService = corretoraService;
            _userManager = userManager;
            _configuration = configuration;
        }

        private async Task ObterIntegracoes()
        {
            var bancos = _bancoService.ObterTodos();
            ViewBag.Bancos = bancos.OrderBy(b => b.Nome).Select(a => new SelectListItem
            {
                Text = a.Nome,
                Value = a.Nome
            }).ToList();
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"] ?? "AIzaSyBjUtP8FEbSpKjbU4JY6p9LU-vURNN7e48";
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim, Guid? corretoraId, string numeroCotacao, string seguradora, string corretor, int page = 1)
        {
            await ObterIntegracoes();
            var list = await _cotacaoService.ObterTodosAsync(numeroCotacao, seguradora, dataInicio, dataFim, null, page, 20);
            var model = new CotacaoModel
            {
                ListaCotacoes = list?.ToList() ?? new List<CotacaoEntity>(),
                Sucesso = true
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> NovaCotacao()
        {
            await ObterIntegracoes();
            return View(new CotacaoModel { Sucesso = true });
        }

        [HttpPost]
        public async Task<IActionResult> NovaCotacao(CotacaoModel model)
        {
            try
            {
                if (model?.Cotacao == null)
                    return Json(new { success = false, message = "Dados inválidos." });

                var result = await _cotacaoService.CadastrarAsync(model.Cotacao);
                if (result.Sucesso)
                    return Json(new { success = true, message = "Cotação cadastrada com sucesso!", cotacaoId = result.Id.ToString() });

                return Json(new { success = false, message = result.Mensagem ?? "Erro ao cadastrar cotação." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao cadastrar cotação: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EnviarCotacao(string numeroCotacao)
        {
            if (string.IsNullOrEmpty(numeroCotacao))
                return RedirectToAction(nameof(Index));

            var result = await _cotacaoService.ObterPorNumeroCotacaoAsync(numeroCotacao);
            return View(result ?? new CotacaoEntity());
        }

        [HttpGet]
        public async Task<IActionResult> Visualizar(Guid id)
        {
            var cotacao = await _cotacaoService.ObterPorIdAsync(id);
            if (cotacao == null)
                return RedirectToAction(nameof(Index));

            await ObterIntegracoes();
            return View(new CotacaoModel { Cotacao = cotacao, Sucesso = true });
        }

        [HttpGet]
        public async Task<IActionResult> VisualizarCotacao(string numeroCotacao)
        {
            if (string.IsNullOrEmpty(numeroCotacao))
                return RedirectToAction(nameof(Index));

            var cotacao = await _cotacaoService.ObterPorNumeroCotacaoAsync(numeroCotacao);
            if (cotacao == null)
                return RedirectToAction(nameof(Index));

            await ObterIntegracoes();
            return View(new CotacaoModel { Cotacao = cotacao, Sucesso = true });
        }

        [HttpGet]
        public async Task<IActionResult> DashboardGeral(DateTime? dataInicio, DateTime? dataFim)
        {
            var user = await _userManager.GetUserAsync(User);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RelatorioDemonstrativoCotacao(Guid id)
        {
            var cotacao = await _cotacaoService.ObterPorIdAsync(id);
            if (cotacao == null)
                return RedirectToAction(nameof(Index));

            return View(cotacao);
        }

        [HttpGet]
        public async Task<IActionResult> EfetivarCotacao(string numeroCotacao)
        {
            if (string.IsNullOrEmpty(numeroCotacao))
                return Json(new { success = false, message = "Número da cotação não informado." });

            var cotacao = await _cotacaoService.ObterPorNumeroCotacaoAsync(numeroCotacao);
            if (cotacao == null)
                return Json(new { success = false, message = "Cotação não encontrada." });

            var result = await _cotacaoService.EfetivarAsync(cotacao.Id);
            return Json(new { success = result, message = result ? "Cotação efetivada com sucesso!" : "Erro ao efetivar cotação." });
        }

        [HttpGet]
        public async Task<IActionResult> DesfazerEfetivacaoCotacao(string numeroCotacao)
        {
            if (string.IsNullOrEmpty(numeroCotacao))
                return Json(new { success = false, message = "Número da cotação não informado." });

            var cotacao = await _cotacaoService.ObterPorNumeroCotacaoAsync(numeroCotacao);
            if (cotacao == null)
                return Json(new { success = false, message = "Cotação não encontrada." });

            var result = await _cotacaoService.DesfazerEfetivacaoAsync(cotacao.Id);
            return Json(new { success = result, message = result ? "Efetivação desfeita com sucesso!" : "Erro ao desfazer efetivação." });
        }

        [HttpGet]
        public async Task<IActionResult> GerarPdfRelatorioCotacao(Guid id)
        {
            return RedirectToAction(nameof(Visualizar), new { id });
        }
    }
}
