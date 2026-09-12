using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/corretoras")]
    [Authorize]
    public class CorretorasController : ControllerBase
    {
        private readonly ICorretoraService _service;
        public CorretorasController(ICorretoraService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> ObterTodas()
            => Ok(ApiResponse<object>.Ok(await _service.ObterTodosAsync()));

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var result = await _service.ObterPorIdAsync(id);
            if (result == null) return NotFound(ApiResponse<object>.Fail("Corretora não encontrada."));
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.CorretoraEntity entity)
            => CreatedAtAction(nameof(ObterPorId), new { id = entity.Id }, ApiResponse<object>.Ok(await _service.CadastrarAsync(entity)));

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.CorretoraEntity entity)
        {
            entity.Id = id;
            return Ok(ApiResponse<object>.Ok(await _service.AtualizarAsync(entity)));
        }
    }
}
