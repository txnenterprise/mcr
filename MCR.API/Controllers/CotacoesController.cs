using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/cotacoes")]
    [Authorize]
    public class CotacoesController : ControllerBase
    {
        private readonly ICotacaoService _cotacaoService;

        public CotacoesController(ICotacaoService cotacaoService)
        {
            _cotacaoService = cotacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string corretor = null,
            [FromQuery] string seguradora = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] bool? efetivada = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var cotacoes = await _cotacaoService.ObterTodosAsync(corretor, seguradora, dataInicio, dataFim, efetivada, page, pageSize);
            var total = await _cotacaoService.ObterTotalItensAsync();
            var items = cotacoes.Select(c => (object)new
            {
                c.Id,
                c.CodigoCotacao,
                c.DataHoraCotacao,
                c.Seguradora,
                c.Cancelado,
                c.Efetivada,
                c.Premio,
                c.Corretor
            }).ToList();
            var result = new PaginationResponse<object>(items, total, page, pageSize);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var cotacao = await _cotacaoService.ObterPorIdAsync(id);
            if (cotacao == null)
                return NotFound(ApiResponse<object>.Fail("Cotação não encontrada."));
            return Ok(ApiResponse<object>.Ok(cotacao));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.CotacaoEntity cotacao)
        {
            var result = await _cotacaoService.CadastrarAsync(cotacao);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.CotacaoEntity cotacao)
        {
            cotacao.Id = id;
            var result = await _cotacaoService.AtualizarAsync(cotacao);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost("{id}/efetivar")]
        public async Task<IActionResult> Efetivar(Guid id)
        {
            var result = await _cotacaoService.EfetivarAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível efetivar a cotação."));
            return Ok(ApiResponse<object>.Ok(null, "Cotação efetivada com sucesso."));
        }

        [HttpPost("{id}/desfazer-efetivacao")]
        public async Task<IActionResult> DesfazerEfetivacao(Guid id)
        {
            var result = await _cotacaoService.DesfazerEfetivacaoAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível desfazer a efetivação."));
            return Ok(ApiResponse<object>.Ok(null, "Efetivação desfeita com sucesso."));
        }

        [HttpGet("{id}/retornos")]
        public async Task<IActionResult> ObterRetornos(Guid id)
        {
            var retornos = await _cotacaoService.ObterRetornosPorCotacaoIdAsync(id);
            return Ok(ApiResponse<object>.Ok(retornos));
        }

        [HttpGet("{id}/relatorio-pdf")]
        public async Task<IActionResult> GerarRelatorioPdf(Guid id)
        {
            var pdf = await _cotacaoService.GerarRelatorioPdfAsync(id);
            if (pdf == null)
                return NotFound(ApiResponse<object>.Fail("Relatório não disponível."));
            return File(pdf, "application/pdf", $"cotacao_{id}.pdf");
        }
    }
}
