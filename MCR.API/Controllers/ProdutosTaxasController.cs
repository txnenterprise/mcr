using Microsoft.AspNetCore.Mvc;
using MCR.API.Produto.Domain.DTO;
using MCR.API.Entities;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class ProdutosTaxasController : Controller
    {
        private readonly IProdutosTaxasService _service;

        public ProdutosTaxasController(IProdutosTaxasService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(ProdutosTaxasDTO model)
        {
            try
            {
                var entity = new ProdutosTaxasEntity
                {
                    ProdutoId = model.ProdutoId,
                    UF = model.UF ?? "",
                    Municipio = model.Municipio ?? "",
                    ProdutividadeEsperada = model.ProdutividadeEsperada ?? 0,
                    TaxaNc65 = model.TaxaNc65 ?? 0,
                    TaxaNc70 = model.TaxaNc70 ?? 0,
                    TaxaNc75 = model.TaxaNc75 ?? 0,
                    Cpf = model.Cpf ?? "0",
                    Ativo = true,
                    Excluido = false
                };

                bool result;
                if (model.ProdutoTaxaId != Guid.Empty)
                {
                    entity.Id = model.ProdutoTaxaId;
                    result = await _service.AtualizarAsync(entity);
                    return Json(new { success = result, message = result ? "Taxa atualizada com sucesso!" : "Erro ao atualizar taxa." });
                }
                else
                {
                    result = await _service.AdicionarAsync(entity);
                    return Json(new { success = result, message = result ? "Taxa adicionada com sucesso!" : "Erro ao adicionar taxa." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao salvar taxa: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(Guid taxaId)
        {
            try
            {
                var result = await _service.ExcluirAsync(taxaId);
                return Json(new { success = result, message = result ? "Taxa excluída com sucesso!" : "Erro ao excluir taxa." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao excluir: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterById(Guid taxaId)
        {
            var entity = await _service.ObterPorIdAsync(taxaId);
            if (entity == null)
                return Json(new { success = false, message = "Taxa não encontrada." });

            return Json(new
            {
                produtoTaxaId = entity.Id,
                produtoId = entity.ProdutoId,
                uf = entity.UF,
                municipio = entity.Municipio,
                produtividadeEsperada = entity.ProdutividadeEsperada,
                taxaNc65 = entity.TaxaNc65,
                taxaNc70 = entity.TaxaNc70,
                taxaNc75 = entity.TaxaNc75,
                cpf = entity.Cpf,
                ativo = entity.Ativo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarTaxas(IFormFile file, Guid produtoId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "Nenhum arquivo enviado." });

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return Json(new { success = false, message = "Apenas arquivos CSV são aceitos." });

                using var stream = file.OpenReadStream();
                var result = await _service.ImportarCsvAsync(produtoId, stream);
                return Json(new { success = result, message = result ? "Taxas importadas com sucesso!" : "Erro ao importar taxas." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao importar: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProdutosTaxasList(Guid produtoId)
        {
            return ViewComponent("ProdutosTaxasList", new { produtoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTodas(Guid produtoId)
        {
            try
            {
                var result = await _service.ExcluirTodasPorProdutoAsync(produtoId);
                return Json(new { success = result, message = result ? "Taxas excluídas com sucesso!" : "Erro ao excluir taxas." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao excluir: " + ex.Message });
            }
        }
    }
}
