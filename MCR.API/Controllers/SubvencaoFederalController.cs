using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class SubvencaoFederalController : Controller
    {
        private readonly ISubvencaoFederalService _service;
        private readonly ICulturaService _culturaService;

        public SubvencaoFederalController(ISubvencaoFederalService service, ICulturaService culturaService)
        {
            _service = service;
            _culturaService = culturaService;
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

            var retorno = (await _service.ObterTodosPaginadoAsync(null, null, ativo, page, pageSize)).ToList();

            var model = new SubvencaoFederalModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaSubvencoesFederais = retorno
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            var model = new SubvencaoFederalModel();
            var culturas = (await _culturaService.ObterTodosPaginadoAsync()).ToList();
            ViewBag.Cultura = culturas.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Nome,
                Text = c.Nome
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(SubvencaoFederalModel model)
        {
            var cadastro = await _service.CadastrarAsync(model.SubvencaoFederal);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.SubvencaoFederal.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var entity = await _service.ObterPorIdAsync(id);
            var culturas = (await _culturaService.ObterTodosPaginadoAsync()).ToList();
            ViewBag.Cultura = culturas.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Nome,
                Text = c.Nome
            }).ToList();
            return View(new SubvencaoFederalModel { SubvencaoFederal = entity });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(SubvencaoFederalModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.SubvencaoFederal);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.SubvencaoFederal.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Inativar(Guid id)
        {
            await _service.InativarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Ativar(Guid id)
        {
            await _service.AtivarAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
