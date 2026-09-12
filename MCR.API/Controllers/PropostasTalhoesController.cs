using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasTalhoesController : Controller
    {
        private readonly IPropostaTalhaoService _service;

        public PropostasTalhoesController(IPropostaTalhaoService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularTalhoesProposta(PropostasTalhoesCadastrarDTO model)
        {
            var result = await _service.VincularTalhoesProposta(model);
            return Json(new { success = result.Sucesso, message = result.Mensagem });
        }
    }
}
