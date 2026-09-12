using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;
using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Controllers
{
    public class PropostasBeneficiariosController : Controller
    {
        private readonly IPropostasBeneficiariosService _service;

        public PropostasBeneficiariosController(IPropostasBeneficiariosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObterBeneficiariosPorProposta(Guid propostaId)
        {
            var result = await _service.ObterPorPropostaAsync(propostaId);
            return Ok(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarBeneficiarios(PropostasBeneficiariosDTO request)
        {
            var result = await _service.SalvarBeneficiariosAsync(request);
            return Json(new { success = result.Sucesso, message = result.Mensagem });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverPropostaBeneficiario(Guid propostaBeneficiarioId)
        {
            var result = await _service.RemoverBeneficiarioAsync(propostaBeneficiarioId);
            return Json(new { success = result, message = result ? "Beneficiário removido com sucesso!" : "Erro ao remover beneficiário." });
        }
    }
}
