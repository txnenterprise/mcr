using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/sinistros")]
    [Authorize]
    public class SinistrosController : ControllerBase
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasStatusService _statusService;

        public SinistrosController(IPropostasService propostasService, IPropostasStatusService statusService)
        {
            _propostasService = propostasService;
            _statusService = statusService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] ObterPropostasPaginadoRequestDTO request)
        {
            request.StatusFilter = new List<string>
            {
                "Sinistro comunicar", "Sinistro pendencia", "Sinistro aberto regulacao",
                "Sinistro aguard pagamento", "Sinistro deferido pago", "Sinistro indeferido", "Sinistro cancelado"
            };
            var propostas = await _propostasService.ObterTodosPaginadoAsync(request);
            return Ok(ApiResponse<object>.Ok(propostas));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var proposta = await _propostasService.ObterPorIdAsync(id);
            if (proposta == null)
                return NotFound(ApiResponse<object>.Fail("Sinistro não encontrado."));
            return Ok(ApiResponse<object>.Ok(proposta));
        }

        [HttpPost("gerar/{cotacaoAgricolaId}")]
        public async Task<IActionResult> GerarSinistro(Guid cotacaoAgricolaId)
        {
            var result = await _propostasService.GerarPropostaAsync(cotacaoAgricolaId);
            if (!result.Sucesso)
                return BadRequest(ApiResponse<object>.Fail(result.Mensagem));
            return Ok(ApiResponse<object>.Ok(new { propostaId = result.PropostaId }, result.Mensagem));
        }

        [HttpPost("{id}/transmitir")]
        public async Task<IActionResult> EnviarTransmissao(Guid id)
        {
            var result = await _propostasService.EnviarTransmissaoAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível transmitir o sinistro."));
            return Ok(ApiResponse<object>.Ok(null, "Sinistro transmitido com sucesso."));
        }

        [HttpGet("{id}/status-validacao")]
        public async Task<IActionResult> ObterStatusValidacao(Guid id)
        {
            var validacao = await _propostasService.ObterStatusValidacaoAsync(id);
            return Ok(ApiResponse<object>.Ok(validacao));
        }

        [HttpGet("{id}/status")]
        public async Task<IActionResult> ObterStatus(Guid id)
        {
            var status = await _statusService.ObterStatusPorPropostaAsync(id);
            return Ok(ApiResponse<object>.Ok(status));
        }
    }
}
