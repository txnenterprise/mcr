using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class SubvencaoEstadualController : Controller
    {
        private readonly ISubvencaoEstadualService _service;
        private readonly ICulturaService _culturaService;

        public SubvencaoEstadualController(ISubvencaoEstadualService service, ICulturaService culturaService)
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

            var retorno = (await _service.ObterTodosPaginadoAsync(null, null, null, ativo, page, pageSize)).ToList();

            var model = new SubvencaoEstadualModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaSubvencoesEstaduais = retorno
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            var model = new SubvencaoEstadualModel();
            var culturas = (await _culturaService.ObterTodosPaginadoAsync()).ToList();
            ViewBag.Cultura = culturas.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Nome,
                Text = c.Nome
            }).ToList();
            ViewBag.Estados = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new("AC", "AC"), new("AL", "AL"), new("AP", "AP"), new("AM", "AM"),
                new("BA", "BA"), new("CE", "CE"), new("DF", "DF"), new("ES", "ES"),
                new("GO", "GO"), new("MA", "MA"), new("MT", "MT"), new("MS", "MS"),
                new("MG", "MG"), new("PA", "PA"), new("PB", "PB"), new("PR", "PR"),
                new("PE", "PE"), new("PI", "PI"), new("RJ", "RJ"), new("RN", "RN"),
                new("RS", "RS"), new("RO", "RO"), new("RR", "RR"), new("SC", "SC"),
                new("SP", "SP"), new("SE", "SE"), new("TO", "TO")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(SubvencaoEstadualModel model)
        {
            var cadastro = await _service.CadastrarAsync(model.SubvencaoEstadual);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.SubvencaoEstadual.Mensagem = cadastro.Mensagem;
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
            ViewBag.Estados = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>
            {
                new("AC", "AC"), new("AL", "AL"), new("AP", "AP"), new("AM", "AM"),
                new("BA", "BA"), new("CE", "CE"), new("DF", "DF"), new("ES", "ES"),
                new("GO", "GO"), new("MA", "MA"), new("MT", "MT"), new("MS", "MS"),
                new("MG", "MG"), new("PA", "PA"), new("PB", "PB"), new("PR", "PR"),
                new("PE", "PE"), new("PI", "PI"), new("RJ", "RJ"), new("RN", "RN"),
                new("RS", "RS"), new("RO", "RO"), new("RR", "RR"), new("SC", "SC"),
                new("SP", "SP"), new("SE", "SE"), new("TO", "TO")
            };
            return View(new SubvencaoEstadualModel { SubvencaoEstadual = entity });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(SubvencaoEstadualModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.SubvencaoEstadual);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.SubvencaoEstadual.Mensagem = cadastro.Mensagem;
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
