using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class TalhaoController : Controller
    {
        private readonly ITalhaoService _service;
        private readonly IPropriedadeService _propriedadeService;

        public TalhaoController(ITalhaoService service, IPropriedadeService propriedadeService)
        {
            _service = service;
            _propriedadeService = propriedadeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            var retorno = (await _service.ObterTodosPaginadoAsync(null, null, page, pageSize)).ToList();

            var model = new TalhaoModel
            {
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaTalhoes = retorno
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar(Guid? propriedadeId = null)
        {
            var model = new TalhaoModel();
            model.Talhao.Sucesso = true;

            if (propriedadeId.HasValue && propriedadeId.Value != Guid.Empty)
            {
                model.Talhao.PropriedadeId = propriedadeId.Value;
            }

            var propriedades = await _propriedadeService.ObterTodosPaginadoAsync(null, null, null, null, 1, 999);
            ViewBag.Propriedades = new SelectList(propriedades.Select(p => new { p.Id, p.Nome }), "Id", "Nome");
            ViewBag.GoogleMapsApiKey = "AIzaSyBjUtP8FEbSpKjbU4JY6p9LU-vURNN7e48";
            ViewBag.PropriedadeIdPreSelecionado = propriedadeId?.ToString() ?? "";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Cadastrar(TalhaoModel model)
        {
            if (model?.Talhao == null)
                return Json(new { success = false, message = "Dados inválidos. Verifique os campos e tente novamente." });
            if (model.Talhao.PropriedadeId == Guid.Empty)
                return Json(new { success = false, message = "Selecione uma propriedade." });

            model.Talhao.Ativo = true;
            model.Talhao.Excluido = false;
            model.Talhao.ImagemTalhao = model.ImageJson ?? "";
            model.Talhao.KmlTalhao = model.KmlJson ?? "";

            var cadastro = await _service.CadastrarAsync(model.Talhao);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Talhao.Mensagem = cadastro.Mensagem;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var talhao = await _service.ObterPorIdAsync(id);
            if (talhao == null)
                return RedirectToAction(nameof(Index));

            return View(new TalhaoModel { Talhao = talhao });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(TalhaoModel model)
        {
            var cadastro = await _service.AtualizarAsync(model.Talhao);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Talhao.Mensagem = cadastro.Mensagem;
            return View(model);
        }
    }
}
