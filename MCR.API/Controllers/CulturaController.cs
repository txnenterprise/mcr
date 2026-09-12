using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class CulturaController : Controller
    {
        private readonly ICulturaService _service;

        public CulturaController(ICulturaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            bool? ativo = pesquisaAtivoInativo switch
            {
                "Sim" => true,
                "Não" => false,
                _ => null
            };

            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaNome, null, null, ativo, page, pageSize)).ToList();

            ViewBag.PesquisaNome = pesquisaNome;
            ViewBag.PesquisaAtivoInativo = pesquisaAtivoInativo;
            ViewBag.PaginaAtual = page;

            return View(retorno);
        }

        [HttpGet("Cultura/Cadastrar")]
        public IActionResult Cadastrar()
        {
            return View(new Models.CulturaModel
            {
                Cultura = new MCR.API.Entities.CulturaEntity()
            });
        }

        [HttpPost("Cultura/Cadastrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Models.CulturaModel model)
        {
            var nomeExistente = (await _service.ObterTodosPaginadoAsync()).Any(c => c.Nome == model.Cultura.Nome && c.Ativo);
            if (nomeExistente)
            {
                model.Cultura.Sucesso = false;
                model.Cultura.Mensagem = "Já existe uma cultura com essa descrição.";
                return View(model);
            }

            var cadastro = await _service.CadastrarAsync(model.Cultura);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Cultura.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet("Cultura/Editar/{id}")]
        public async Task<IActionResult> Editar(Guid id)
        {
            var cultura = await _service.ObterPorIdAsync(id);
            return View(new Models.CulturaModel { Cultura = cultura });
        }

        [HttpPost("Cultura/Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Models.CulturaModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.Cultura);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Cultura.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpPost("Cultura/Inativar/{id}")]
        public async Task<IActionResult> Inativar(Guid id)
        {
            await _service.InativarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Cultura/Ativar/{id}")]
        public async Task<IActionResult> Ativar(Guid id)
        {
            await _service.AtivarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
