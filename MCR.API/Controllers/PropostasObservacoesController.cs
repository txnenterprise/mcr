using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasObservacoesController : Controller
    {
        private readonly IPropostasObservacoesService _service;

        public PropostasObservacoesController(IPropostasObservacoesService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SalvarObservacao(Guid propostaId, string observacao)
        {
            try
            {
                var usuarioIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var usuarioId = Guid.TryParse(usuarioIdStr, out var uid) ? uid : Guid.Empty;
                var result = await _service.SalvarObservacaoAsync(propostaId, observacao, usuarioId);
                return Json(new { success = result, message = result ? "Observação salva!" : "Erro ao salvar observação." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }
    }
}
