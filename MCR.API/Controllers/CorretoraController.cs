using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class CorretoraController : Controller
    {
        private readonly ICorretoraService _service;
        private readonly IPlanoService _planoService;

        public CorretoraController(ICorretoraService service, IPlanoService planoService)
        {
            _service = service;
            _planoService = planoService;
        }

        private async Task CarregarPlanos()
        {
            var planos = (await _planoService.ObterTodosAsync()).ToList();
            ViewBag.Planos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                planos.Select(p => new { p.Id, p.Nome }), "Id", "Nome");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            var todas = (await _service.ObterTodosAsync()).ToList();

            if (!string.IsNullOrEmpty(pesquisaNome))
                todas = todas.Where(c => c.RazaoSocial?.Contains(pesquisaNome, StringComparison.OrdinalIgnoreCase) == true).ToList();

            var totalPages = (int)Math.Ceiling(todas.Count / (double)pageSize);
            var pagina = todas.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new CorretoraModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = totalPages,
                ListaCorretoras = pagina
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            await CarregarPlanos();
            return View(new CorretoraModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(CorretoraModel model)
        {
            var cadastro = await _service.CadastrarAsync(model.Corretora);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Corretora.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var corretora = await _service.ObterPorIdAsync(id);
            if (corretora == null)
                return RedirectToAction(nameof(Index));

            await CarregarPlanos();
            return View(new CorretoraModel { Corretora = corretora });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CorretoraModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.Corretora);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Corretora.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet]
        public IActionResult Configurar(Guid id) => View(new CorretoraModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configurar(CorretoraModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.Corretora);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Configurar));

            model.Corretora.Mensagem = cadastro.Mensagem;
            return View(model);
        }
    }
}
