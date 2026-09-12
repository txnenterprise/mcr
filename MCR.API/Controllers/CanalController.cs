using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class CanalController : Controller
    {
        private readonly ICanalService _service;
        private readonly ICorretoraService _corretoraService;

        public CanalController(ICanalService service, ICorretoraService corretoraService)
        {
            _service = service;
            _corretoraService = corretoraService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, int page = 1, int pageSize = 10)
        {
            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaNome, null, page, pageSize)).ToList();

            var model = new CanalModel
            {
                PesquisaNome = pesquisaNome,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaCanais = retorno
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            var corretoraLista = (await _corretoraService.ObterTodosAsync()).ToList();
            var model = new CanalModel();

            if (corretoraLista.Count == 1)
                model.Canal.CorretoraId = corretoraLista.First().Id;

            ViewBag.Corretoras = new SelectList(corretoraLista, "Id", "RazaoSocial");
            ViewBag.CorretoraCount = corretoraLista.Count;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(CanalModel model)
        {
            model.Canal.ImagemLogo = model.ObjectImagem;
            var resultado = await _service.CadastrarAsync(model.Canal);

            if (resultado.Sucesso)
                return RedirectToAction(nameof(Index));

            var corretoraLista = (await _corretoraService.ObterTodosAsync()).ToList();
            ViewBag.Corretoras = new SelectList(corretoraLista, "Id", "RazaoSocial");
            ViewBag.CorretoraCount = corretoraLista.Count;

            model.Canal.Mensagem = resultado.Mensagem;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListarCanaisPorCorretora(Guid corretoraId)
        {
            var canais = await _service.ObterPorCorretoraAsync(corretoraId);
            var lista = canais?.Select(c => new { id = c.Id.ToString(), text = c.RazaoSocial ?? c.NomeFantasia }).ToList() ?? new();
            return Json(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var canal = await _service.ObterPorIdAsync(id);
            if (canal == null)
                return RedirectToAction(nameof(Index));

            var corretoraLista = (await _corretoraService.ObterTodosAsync()).ToList();
            ViewBag.Corretoras = new SelectList(corretoraLista, "Id", "RazaoSocial");
            ViewBag.CorretoraCount = corretoraLista.Count;

            return View(new CanalModel { Canal = canal });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(CanalModel model)
        {
            model.Canal.ImagemLogo = model.ObjectImagem;
            var resultado = await _service.AtualizarAsync(model.Canal);

            if (resultado.Sucesso)
                return RedirectToAction(nameof(Index));

            var corretoraLista = (await _corretoraService.ObterTodosAsync()).ToList();
            ViewBag.Corretoras = new SelectList(corretoraLista, "Id", "RazaoSocial");
            ViewBag.CorretoraCount = corretoraLista.Count;

            model.Canal.Mensagem = resultado.Mensagem;
            return View(model);
        }
    }
}
