using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string nome = null, [FromQuery] string cpf = null,
            [FromQuery] string cidade = null, [FromQuery] string estado = null,
            [FromQuery] bool? ativo = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var clientes = await _clienteService.ObterTodosPaginadoAsync(nome, cpf, cidade, estado, ativo, page, pageSize);
            return Ok(ApiResponse<object>.Ok(clientes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var cliente = await _clienteService.ObterPorIdAsync(id);
            if (cliente == null)
                return NotFound(ApiResponse<object>.Fail("Cliente não encontrado."));
            return Ok(ApiResponse<object>.Ok(cliente));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.ClienteEntity cliente)
        {
            var result = await _clienteService.CadastrarAsync(cliente);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.ClienteEntity cliente)
        {
            cliente.Id = id;
            var result = await _clienteService.AtualizarAsync(cliente);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("buscar/cpf")]
        public async Task<IActionResult> BuscarPorCpf([FromQuery] string cpf)
        {
            var cliente = await _clienteService.ObterPorCpfAsync(cpf);
            if (cliente == null)
                return NotFound(ApiResponse<object>.Fail("Cliente não encontrado."));
            return Ok(ApiResponse<object>.Ok(cliente));
        }

        [HttpGet("buscar/nome")]
        public async Task<IActionResult> BuscarPorNome([FromQuery] string nome)
        {
            var clientes = await _clienteService.PesquisarPorNomeAsync(nome);
            return Ok(ApiResponse<object>.Ok(clientes));
        }

        [HttpGet("{id}/propriedades")]
        public async Task<IActionResult> ObterPropriedades(Guid id)
        {
            var props = await _clienteService.ObterPropriedadesDoClienteAsync(id);
            return Ok(ApiResponse<object>.Ok(props));
        }

        [HttpGet("propriedades/{propriedadeId}/talhoes")]
        public async Task<IActionResult> ObterTalhoesDaPropriedade(Guid propriedadeId)
        {
            var talhoes = await _clienteService.ObterTalhoesDaPropriedadeAsync(propriedadeId);
            return Ok(ApiResponse<object>.Ok(talhoes));
        }
    }
}
