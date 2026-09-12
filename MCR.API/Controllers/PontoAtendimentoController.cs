using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class PontoAtendimentoController : Controller
    {
        private readonly IPontoAtendimentoService _service;
        private readonly ICanalService _canalService;

        public PontoAtendimentoController(IPontoAtendimentoService service, ICanalService canalService)
        {
            _service = service;
            _canalService = canalService;
        }

        private async Task CarregarCanais()
        {
            var canais = await _canalService.ObterTodosPaginadoAsync(null, null, 1, 999);
            ViewBag.Canais = new SelectList(canais.Select(c => new { c.Id, c.RazaoSocial }), "Id", "RazaoSocial");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaNome, null, page, pageSize)).ToList();

            var model = new PontoAtendimentoModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaPontosAtendimento = retorno
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            await CarregarCanais();
            return View(new PontoAtendimentoModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(PontoAtendimentoModel model)
        {
            model.PontoAtendimento.ImagemLogo = model.ObjectImagem ?? "";
            var cadastro = await _service.CadastrarAsync(model.PontoAtendimento);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.PontoAtendimento.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var pa = await _service.ObterPorIdAsync(id);
            if (pa == null)
                return RedirectToAction(nameof(Index));

            await CarregarCanais();
            return View(new PontoAtendimentoModel { PontoAtendimento = pa });
        }

        [HttpGet]
        public async Task<IActionResult> ListarPontosAtendimentoPorCanal(Guid canalId)
        {
            var pontos = await _service.ObterPorCanalAsync(canalId);
            var lista = pontos?.Select(p => new { id = p.Id.ToString(), text = p.RazaoSocial }).ToList() ?? new();
            return Json(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(PontoAtendimentoModel model)
        {
            model.PontoAtendimento.ImagemLogo = model.ObjectImagem ?? "";
            var cadastro = await _service.AtualizarAsync(model.PontoAtendimento);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            await CarregarCanais();
            model.PontoAtendimento.Mensagem = cadastro.Mensagem;
            return View(model);
        }
    }
}
