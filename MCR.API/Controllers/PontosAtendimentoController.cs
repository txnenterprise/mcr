using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/pontos-atendimento")]
    [Authorize]
    public class PontosAtendimentoController : ControllerBase
    {
        private readonly IPontoAtendimentoService _service;
        public PontosAtendimentoController(IPontoAtendimentoService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] string nome = null, [FromQuery] Guid? canalId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(ApiResponse<object>.Ok(await _service.ObterTodosPaginadoAsync(nome, canalId, page, pageSize)));

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var result = await _service.ObterPorIdAsync(id);
            if (result == null) return NotFound(ApiResponse<object>.Fail("Ponto de atendimento não encontrado."));
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("por-canal/{canalId}")]
        public async Task<IActionResult> ObterPorCanal(Guid canalId)
            => Ok(ApiResponse<object>.Ok(await _service.ObterPorCanalAsync(canalId)));

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.PontoAtendimentoEntity entity)
            => CreatedAtAction(nameof(ObterPorId), new { id = entity.Id }, ApiResponse<object>.Ok(await _service.CadastrarAsync(entity)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.PontoAtendimentoEntity entity)
        {
            entity.Id = id;
            return Ok(ApiResponse<object>.Ok(await _service.AtualizarAsync(entity)));
        }
    }
}
