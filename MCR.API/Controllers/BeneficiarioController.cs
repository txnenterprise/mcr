using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class BeneficiarioController : Controller
    {
        private readonly IBeneficiarioService _service;
        private readonly IBancoService _bancoService;

        public BeneficiarioController(IBeneficiarioService service, IBancoService bancoService)
        {
            _service = service;
            _bancoService = bancoService;
        }

        private void CarregarBancos(string? selecionado = null)
        {
            var bancos = _bancoService.ObterTodos().OrderBy(b => b.Nome).ToList();
            ViewBag.Bancos = new SelectList(bancos.Select(b => new { id = b.Nome, text = b.Nome }), "id", "text", selecionado);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaCNPJ, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            bool? ativo = pesquisaAtivoInativo switch
            {
                "Sim" => true,
                "Não" => false,
                _ => null
            };

            var retorno = (await _service.ObterTodosPaginadoAsync(pesquisaNome, pesquisaCNPJ, ativo, page, pageSize)).ToList();

            var model = new BeneficiarioModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaCNPJ = pesquisaCNPJ,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = retorno.FirstOrDefault()?.TotalPages ?? 0,
                ListaBeneficiarios = retorno
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            CarregarBancos();
            return View(new BeneficiarioModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(BeneficiarioModel model)
        {
            model.Beneficiario.ImagemCNPJ = model.ObjectImagem;
            var cadastro = await _service.CadastrarAsync(model.Beneficiario);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Beneficiario.Mensagem = cadastro.Mensagem;
            CarregarBancos(model.Beneficiario.Banco);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var beneficiario = await _service.ObterPorIdAsync(id);
            if (beneficiario == null)
                return RedirectToAction(nameof(Index));

            var model = new BeneficiarioModel
            {
                Beneficiario = beneficiario,
                ObjectImagem = beneficiario.ImagemCNPJ
            };

            CarregarBancos(beneficiario.Banco);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(BeneficiarioModel model)
        {
            model.Beneficiario.ImagemCNPJ = model.ObjectImagem;
            var cadastro = await _service.AtualizarAsync(model.Beneficiario);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Beneficiario.Mensagem = cadastro.Mensagem;
            CarregarBancos(model.Beneficiario.Banco);
            return View(model);
        }
    }
}
