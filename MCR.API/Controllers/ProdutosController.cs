using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Authorize]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutosService _produtosService;
        private readonly IProdutosTaxasService _taxasService;
        private readonly IProdutosCanalService _canalService;

        public ProdutosController(
            IProdutosService produtosService,
            IProdutosTaxasService taxasService,
            IProdutosCanalService canalService)
        {
            _produtosService = produtosService;
            _taxasService = taxasService;
            _canalService = canalService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos(
            [FromQuery] string descricao = null, [FromQuery] Guid? seguradoraId = null,
            [FromQuery] Guid? safraId = null, [FromQuery] Guid? culturaId = null,
            [FromQuery] bool? ativo = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var produtos = await _produtosService.ObterTodosPaginadoAsync(descricao, seguradoraId, safraId, culturaId, ativo, page, pageSize);
            return Ok(ApiResponse<object>.Ok(produtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var produto = await _produtosService.ObterPorIdAsync(id);
            if (produto == null)
                return NotFound(ApiResponse<object>.Fail("Produto não encontrado."));
            return Ok(ApiResponse<object>.Ok(produto));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Entities.ProdutosEntity produto)
        {
            var result = await _produtosService.CadastrarAsync(produto);
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] Entities.ProdutosEntity produto)
        {
            produto.Id = id;
            var result = await _produtosService.AtualizarAsync(produto);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var result = await _produtosService.ExcluirAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível excluir o produto."));
            return Ok(ApiResponse<object>.Ok(null, "Produto excluído com sucesso."));
        }

        [HttpPost("{id}/duplicar")]
        public async Task<IActionResult> Duplicar(Guid id)
        {
            var result = await _produtosService.DuplicarAsync(id);
            if (result == null)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível duplicar o produto."));
            return CreatedAtAction(nameof(ObterPorId), new { id = result.Id }, ApiResponse<object>.Ok(result));
        }

        [HttpPost("{id}/toggle-ativo")]
        public async Task<IActionResult> ToggleAtivo(Guid id)
        {
            var result = await _produtosService.ToggleAtivoAsync(id);
            return Ok(ApiResponse<object>.Ok(new { ativo = result }, "Status alterado com sucesso."));
        }

        // ---- Taxas ----
        [HttpGet("{id}/taxas")]
        public async Task<IActionResult> ListarTaxas(Guid id)
        {
            var taxas = await _taxasService.ObterPorProdutoIdAsync(id);
            return Ok(ApiResponse<object>.Ok(taxas));
        }

        [HttpPost("{id}/taxas")]
        public async Task<IActionResult> AdicionarTaxa(Guid id, [FromBody] Entities.ProdutosTaxasEntity taxa)
        {
            taxa.ProdutoId = id;
            var result = await _taxasService.AdicionarAsync(taxa);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao adicionar taxa."));
            return Ok(ApiResponse<object>.Ok(null, "Taxa adicionada com sucesso."));
        }

        [HttpDelete("{id}/taxas/{taxaId}")]
        public async Task<IActionResult> ExcluirTaxa(Guid id, Guid taxaId)
        {
            var result = await _taxasService.ExcluirAsync(taxaId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao excluir taxa."));
            return Ok(ApiResponse<object>.Ok(null, "Taxa excluída com sucesso."));
        }

        [HttpPost("{id}/taxas/importar")]
        public async Task<IActionResult> ImportarTaxas(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("Arquivo não enviado."));

            using var stream = file.OpenReadStream();
            var result = await _taxasService.ImportarCsvAsync(id, stream);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao importar taxas."));
            return Ok(ApiResponse<object>.Ok(null, "Taxas importadas com sucesso."));
        }

        [HttpDelete("{id}/taxas")]
        public async Task<IActionResult> ExcluirTodasTaxas(Guid id)
        {
            var result = await _taxasService.ExcluirTodasPorProdutoAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao excluir taxas."));
            return Ok(ApiResponse<object>.Ok(null, "Taxas excluídas com sucesso."));
        }

        // ---- Canais ----
        [HttpPost("{id}/canais")]
        public async Task<IActionResult> AdicionarCanal(Guid id, [FromBody] Guid pontoAtendimentoId)
        {
            var result = await _canalService.AdicionarAsync(id, pontoAtendimentoId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao adicionar canal."));
            return Ok(ApiResponse<object>.Ok(null, "Canal adicionado com sucesso."));
        }

        [HttpDelete("{id}/canais/{canalId}")]
        public async Task<IActionResult> ExcluirCanal(Guid id, Guid canalId)
        {
            var result = await _canalService.ExcluirAsync(canalId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao excluir canal."));
            return Ok(ApiResponse<object>.Ok(null, "Canal excluído com sucesso."));
        }
    }
}
