using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/cotacoes-agricolas")]
    [Authorize]
    public class CotacoesAgricolaController : ControllerBase
    {
        private readonly ICotacoesAgricolaService _cotacoesAgricolaService;

        public CotacoesAgricolaController(ICotacoesAgricolaService cotacoesAgricolaService)
        {
            _cotacoesAgricolaService = cotacoesAgricolaService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string codigoCotacao = null,
            [FromQuery] Guid? safraId = null,
            [FromQuery] Guid? culturaId = null,
            [FromQuery] string municipio = null,
            [FromQuery] DateTime? dataCotacao = null,
            [FromQuery] string cpfCliente = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var cotacoes = await _cotacoesAgricolaService.ObterTodosPaginadoAsync(
                codigoCotacao, safraId, culturaId, municipio, dataCotacao, cpfCliente, page, pageSize);
            return Ok(ApiResponse<object>.Ok(cotacoes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var cotacao = await _cotacoesAgricolaService.ObterPorIdAsync(id);
            if (cotacao == null)
                return NotFound(ApiResponse<object>.Fail("Cotação agrícola não encontrada."));
            return Ok(ApiResponse<object>.Ok(cotacao));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.CotacoesAgricolaEntity cotacao)
        {
            var result = await _cotacoesAgricolaService.CadastrarAsync(cotacao);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.CotacoesAgricolaEntity cotacao)
        {
            cotacao.Id = id;
            var result = await _cotacoesAgricolaService.AtualizarAsync(cotacao);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var result = await _cotacoesAgricolaService.ExcluirAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível excluir a cotação."));
            return Ok(ApiResponse<object>.Ok(null, "Cotação excluída com sucesso."));
        }

        [HttpPost("{id}/insucesso")]
        public async Task<IActionResult> RegistrarInsucesso(Guid id)
        {
            var result = await _cotacoesAgricolaService.RegistrarInsucessoAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível registrar o insucesso."));
            return Ok(ApiResponse<object>.Ok(null, "Insucesso registrado com sucesso."));
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GerarPdf(Guid id)
        {
            var pdf = await _cotacoesAgricolaService.GerarPdfAsync(id);
            if (pdf == null)
                return NotFound(ApiResponse<object>.Fail("PDF não disponível."));
            return File(pdf, "application/pdf", $"cotacao_agricola_{id}.pdf");
        }

        [HttpGet("tipos-solo")]
        public async Task<IActionResult> ObterTiposSolo(
            [FromQuery] Guid culturaId,
            [FromQuery] Guid safraId)
        {
            var tipos = await _cotacoesAgricolaService.ObterTiposSoloPorCulturaSafraAsync(culturaId, safraId);
            return Ok(ApiResponse<object>.Ok(tipos));
        }

        [HttpGet("classificacoes-solo")]
        public async Task<IActionResult> ObterClassificacoesSolo(
            [FromQuery] Guid culturaId,
            [FromQuery] Guid safraId)
        {
            var classificacoes = await _cotacoesAgricolaService.ObterClassificacoesSoloPorCulturaSafraAsync(culturaId, safraId);
            return Ok(ApiResponse<object>.Ok(classificacoes));
        }
    }
}
