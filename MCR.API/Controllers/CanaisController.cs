using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/canais")]
    [Authorize]
    public class CanaisController : ControllerBase
    {
        private readonly ICanalService _service;
        public CanaisController(ICanalService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] string nome = null, [FromQuery] Guid? corretoraId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(ApiResponse<object>.Ok(await _service.ObterTodosPaginadoAsync(nome, corretoraId, page, pageSize)));

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var result = await _service.ObterPorIdAsync(id);
            if (result == null) return NotFound(ApiResponse<object>.Fail("Canal não encontrado."));
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("por-corretora/{corretoraId}")]
        public async Task<IActionResult> ObterPorCorretora(Guid corretoraId)
            => Ok(ApiResponse<object>.Ok(await _service.ObterPorCorretoraAsync(corretoraId)));

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.CanalEntity entity)
            => CreatedAtAction(nameof(ObterPorId), new { id = entity.Id }, ApiResponse<object>.Ok(await _service.CadastrarAsync(entity)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.CanalEntity entity)
        {
            entity.Id = id;
            return Ok(ApiResponse<object>.Ok(await _service.AtualizarAsync(entity)));
        }
    }
}
