using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.DTOs;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] string nome = null, [FromQuery] string email = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(ApiResponse<object>.Ok(await _service.ObterTodosPaginadoAsync(nome, email, page, pageSize)));

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var result = await _service.ObterPorIdAsync(id);
            if (result == null) return NotFound(ApiResponse<object>.Fail("Usuário não encontrado."));
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] CriarUsuarioRequest request)
        {
            var entity = new Entities.UsuarioEntity
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
                Document = request.Document
            };
            var result = await _service.CadastrarAsync(entity, request.Password, request.Role);
            if (result == null)
                return BadRequest(ApiResponse<object>.Fail("Erro ao cadastrar usuário."));
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.UsuarioEntity entity)
        {
            entity.Id = id;
            return Ok(ApiResponse<object>.Ok(await _service.AtualizarAsync(entity)));
        }

        [HttpPost("{id}/bloquear")]
        public async Task<IActionResult> Bloquear(Guid id)
            => Ok(ApiResponse<object>.Ok(new { success = await _service.BloquearAsync(id.ToString()) }));

        [HttpPost("{id}/desbloquear")]
        public async Task<IActionResult> Desbloquear(Guid id)
            => Ok(ApiResponse<object>.Ok(new { success = await _service.DesbloquearAsync(id.ToString()) }));
    }

    public class CriarUsuarioRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Role { get; set; }
    }
}
