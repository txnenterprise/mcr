using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasFormaPagamentosController : ControllerBase
    {
        private readonly IPropostasFormaPagamentosService _service;

        public PropostasFormaPagamentosController(IPropostasFormaPagamentosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetByPropostaId(Guid propostaId)
        {
            var formas = await _service.ObterPorPropostaIdAsync(propostaId);
            return Ok(formas);
        }
    }
}
