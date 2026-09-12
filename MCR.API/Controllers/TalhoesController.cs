using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/talhoes")]
    [Authorize]
    public class TalhoesController : ControllerBase
    {
        private readonly ITalhaoService _talhaoService;

        public TalhoesController(ITalhaoService talhaoService)
        {
            _talhaoService = talhaoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string descricao = null, [FromQuery] Guid? propriedadeId = null,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var talhoes = await _talhaoService.ObterTodosPaginadoAsync(descricao, propriedadeId, page, pageSize);
            return Ok(ApiResponse<object>.Ok(talhoes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var talhao = await _talhaoService.ObterPorIdAsync(id);
            if (talhao == null)
                return NotFound(ApiResponse<object>.Fail("Talhão não encontrado."));
            return Ok(ApiResponse<object>.Ok(talhao));
        }

        [HttpGet("por-propriedade/{propriedadeId}")]
        public async Task<IActionResult> ObterPorPropriedade(Guid propriedadeId)
        {
            var talhoes = await _talhaoService.ObterPorPropriedadeAsync(propriedadeId);
            return Ok(ApiResponse<object>.Ok(talhoes));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.TalhaoEntity talhao)
        {
            var result = await _talhaoService.CadastrarAsync(talhao);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.TalhaoEntity talhao)
        {
            talhao.Id = id;
            var result = await _talhaoService.AtualizarAsync(talhao);
            return Ok(ApiResponse<object>.Ok(result));
        }
    }
}
