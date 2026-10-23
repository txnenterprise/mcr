using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Entities;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    /// <summary>
    /// Cadastro de Planos — somente Administrador.
    /// Planos são IMUTÁVEIS após criação (sem Editar/Excluir por design).
    /// </summary>
    [Authorize(Roles = "Administrador")]
    [Route("Plano")]
    public class PlanoController : Controller
    {
        private readonly IPlanoService _service;

        public PlanoController(IPlanoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var planos = (await _service.ObterTodosAsync()).ToList();
            return View(planos);
        }

        [HttpGet("Cadastrar")]
        public IActionResult Cadastrar() => View(new PlanoEntity());

        [HttpPost("Cadastrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(PlanoEntity model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cadastro = await _service.CadastrarAsync(model);

            if (cadastro.Sucesso)
                return RedirectToAction(nameof(Index));

            model.Mensagem = cadastro.Mensagem;
            return View(model);
        }
    }
}
