using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/propriedades")]
    [Authorize]
    public class PropriedadesController : ControllerBase
    {
        private readonly IPropriedadeService _propriedadeService;

        public PropriedadesController(IPropriedadeService propriedadeService)
        {
            _propriedadeService = propriedadeService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string nome = null, [FromQuery] string estado = null,
            [FromQuery] string cidade = null, [FromQuery] bool? ativo = null,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var props = await _propriedadeService.ObterTodosPaginadoAsync(nome, estado, cidade, ativo, page, pageSize);
            return Ok(ApiResponse<object>.Ok(props));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var prop = await _propriedadeService.ObterPorIdAsync(id);
            if (prop == null)
                return NotFound(ApiResponse<object>.Fail("Propriedade não encontrada."));
            return Ok(ApiResponse<object>.Ok(prop));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.PropriedadeEntity propriedade)
        {
            var result = await _propriedadeService.CadastrarAsync(propriedade);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.PropriedadeEntity propriedade)
        {
            propriedade.Id = id;
            var result = await _propriedadeService.AtualizarAsync(propriedade);
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
