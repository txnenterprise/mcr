using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class SeguradoraController : Controller
    {
        private readonly ISeguradoraService _service;

        public SeguradoraController(ISeguradoraService service)
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

            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaNome, ativo, page, pageSize)).ToList();

            ViewBag.PesquisaNome = pesquisaNome;
            ViewBag.PesquisaAtivoInativo = pesquisaAtivoInativo;
            ViewBag.PaginaAtual = page;

            var model = new Models.SeguradoraModel
            {
                ListaSeguradoras = retorno
            };

            return View(model);
        }

        [HttpGet("Seguradora/Cadastrar")]
        public IActionResult Cadastrar()
        {
            var model = new Models.ViewModels.SeguradoraModel();
            model.Seguradora.Sucesso = true;
            return View(model);
        }

        [HttpPost("Seguradora/Cadastrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Models.ViewModels.SeguradoraModel model)
        {
            var nomeExistente = (await _service.ObterTodosPaginadoAsync(model.Seguradora.NomeFantasia)).Any(s => s.NomeFantasia == model.Seguradora.NomeFantasia && s.Ativo);
            if (nomeExistente)
            {
                model.Seguradora.Sucesso = false;
                model.Seguradora.Mensagem = "Já existe uma seguradora com esse nome.";
                return View(model);
            }

            model.Seguradora.Logo = model.ObjectImagem;
            var cadastro = await _service.CadastrarAsync(model.Seguradora);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Seguradora.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet("Seguradora/Editar/{id}")]
        public async Task<IActionResult> Editar(Guid id)
        {
            var seguradora = await _service.ObterPorIdAsync(id);
            return View(new Models.ViewModels.SeguradoraModel { Seguradora = seguradora });
        }

        [HttpPost("Seguradora/Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Models.ViewModels.SeguradoraModel model)
        {
            var nomeExistente = (await _service.ObterTodosPaginadoAsync(model.Seguradora.NomeFantasia))
                .Any(s => s.NomeFantasia == model.Seguradora.NomeFantasia && s.Ativo && s.Id != model.Seguradora.Id);
            if (nomeExistente)
            {
                model.Seguradora.Sucesso = false;
                model.Seguradora.Mensagem = "Já existe uma seguradora com esse nome.";
                return View(model);
            }

            model.Seguradora.Logo = model.ObjectImagem;
            var cadastro = await _service.AtualizarAsync(model.Seguradora);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Seguradora.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpPost("Seguradora/Inativar/{id}")]
        public async Task<IActionResult> Inativar(Guid id)
        {
            await _service.InativarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Seguradora/Ativar/{id}")]
        public async Task<IActionResult> Ativar(Guid id)
        {
            await _service.AtivarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
