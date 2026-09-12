using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Models;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class PropriedadeController : Controller
    {
        private readonly IPropriedadeService _service;
        private readonly DbContextMCR _context;

        public PropriedadeController(IPropriedadeService service, DbContextMCR context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaCidade, string pesquisaEstado, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            bool? ativo = pesquisaAtivoInativo switch
            {
                "Sim" => true,
                "Não" => false,
                _ => null
            };

            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaNome, pesquisaEstado, pesquisaCidade, ativo, page, pageSize)).ToList();

            ViewBag.PesquisaNome = pesquisaNome;
            ViewBag.PesquisaCidade = pesquisaCidade;
            ViewBag.PesquisaEstado = pesquisaEstado;
            ViewBag.PesquisaAtivoInativo = pesquisaAtivoInativo ?? "Todos";
            ViewBag.PaginaAtual = page;
            ViewBag.TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0;

            return View(retorno);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            var model = new PropriedadeModel
            {
                Propriedade = new MCR.API.Entities.PropriedadeEntity()
            };
            model.Propriedade.Sucesso = true;
            model.Sucesso = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(PropriedadeModel model)
        {
            var cadastro = await _service.CadastrarAsync(model.Propriedade);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Sucesso = cadastro.Sucesso;
            model.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var prop = await _service.ObterPorIdAsync(id);

            if (prop != null && prop.Sucesso)
            {
                var talhoes = await _context.Talhoes
                    .Where(t => t.PropriedadeId == id && !t.Excluido)
                    .ToListAsync();
                prop.Talhoes = talhoes;
            }

            var model = new PropriedadeModel { Propriedade = prop };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(PropriedadeModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.Propriedade);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Sucesso = cadastro.Sucesso;
            model.Mensagem = cadastro.Mensagem;
            return View(model);
        }
    }
}
