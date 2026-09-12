using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasSeguradosController : Controller
    {
        private readonly IPropostaSeguradoService _service;

        public PropostasSeguradosController(IPropostaSeguradoService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularPropostaSegurado(Guid propostaId, Guid clienteId)
        {
            var result = await _service.VincularSeguradoAsync(propostaId, clienteId);
            return Json(new { success = result, message = result ? "Segurado vinculado com sucesso!" : "Erro ao vincular segurado." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPropostaSegurado(Guid propostaSeguradoId)
        {
            var result = await _service.RemoverSeguradoAsync(propostaSeguradoId);
            return Json(new { success = result, message = result ? "Segurado removido com sucesso!" : "Erro ao remover segurado." });
        }
    }
}
