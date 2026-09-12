using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class SafraController : Controller
    {
        private readonly ISafraService _service;

        public SafraController(ISafraService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaDescricao, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            bool? ativo = pesquisaAtivoInativo switch
            {
                "Sim" => true,
                "Não" => false,
                _ => null
            };

            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaDescricao, null, ativo, page, pageSize)).ToList();

            ViewBag.PesquisaDescricao = pesquisaDescricao;
            ViewBag.PesquisaAtivoInativo = pesquisaAtivoInativo;
            ViewBag.PaginaAtual = page;

            return View(retorno);
        }

        [HttpGet("Safra/Cadastrar")]
        public IActionResult Cadastrar()
        {
            var model = new Models.ViewModels.SafraModel
            {
                Safra = new MCR.API.Entities.SafraEntity()
            };
            model.Safra.Sucesso = true;
            return View(model);
        }

        [HttpPost("Safra/Cadastrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Models.ViewModels.SafraModel model)
        {
            var cadastro = await _service.CadastrarAsync(model.Safra);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Safra.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet("Safra/Editar/{id}")]
        public async Task<IActionResult> Editar(Guid id)
        {
            var safra = await _service.ObterPorIdAsync(id);
            var model = new Models.ViewModels.SafraModel
            {
                Safra = safra
            };
            return View(model);
        }

        [HttpPost("Safra/Editar/{id?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Models.ViewModels.SafraModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.Safra);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Safra.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpPost("Safra/Inativar/{id}")]
        public async Task<IActionResult> Inativar(Guid id)
        {
            await _service.InativarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Safra/Ativar/{id}")]
        public async Task<IActionResult> Ativar(Guid id)
        {
            await _service.AtivarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
