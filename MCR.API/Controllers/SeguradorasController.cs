using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/seguradoras")]
    [Authorize]
    public class SeguradorasController : ControllerBase
    {
        private readonly ISeguradoraService _service;

        public SeguradorasController(ISeguradoraService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] string nome = null, [FromQuery] bool? ativo = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(ApiResponse<object>.Ok(await _service.ObterTodosPaginadoAsync(nome, ativo, page, pageSize)));

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var result = await _service.ObterPorIdAsync(id);
            if (result == null) return NotFound(ApiResponse<object>.Fail("Seguradora não encontrada."));
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.SeguradoraEntity entity)
            => CreatedAtAction(nameof(ObterPorId), new { id = entity.Id }, ApiResponse<object>.Ok(await _service.CadastrarAsync(entity)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.SeguradoraEntity entity)
        {
            entity.Id = id;
            return Ok(ApiResponse<object>.Ok(await _service.AtualizarAsync(entity)));
        }

        [HttpPost("{id}/inativar")]
        public async Task<IActionResult> Inativar(Guid id)
            => Ok(ApiResponse<object>.Ok(new { success = await _service.InativarAsync(id) }));

        [HttpPost("{id}/ativar")]
        public async Task<IActionResult> Ativar(Guid id)
            => Ok(ApiResponse<object>.Ok(new { success = await _service.AtivarAsync(id) }));
    }
}
