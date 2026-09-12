using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasRiscosController : Controller
    {
        private readonly IPropostaRiscoService _service;

        public PropostasRiscosController(IPropostaRiscoService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularPropostaRiscos(PropostasRiscosCadastrarDTO model)
        {
            if (model.Riscos == null || !model.Riscos.Any())
                return Json(new { success = false, message = "Nenhuma propriedade selecionada." });

            var riscoIds = model.Riscos
                .Where(r => r.Selecionado)
                .Select(r => r.PropriedadeId)
                .ToList();

            if (!riscoIds.Any())
                return Json(new { success = false, message = "Nenhuma propriedade selecionada." });

            var result = await _service.VincularRiscosAsync(model.PropostaId, riscoIds);
            return Json(new { success = result, message = result ? "Riscos vinculados com sucesso!" : "Erro ao vincular riscos." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPropostaRisco(Guid riscoId)
        {
            var result = await _service.RemoverRiscoAsync(riscoId);
            return Json(new { success = result, message = result ? "Risco removido com sucesso!" : "Erro ao remover risco." });
        }
    }
}
