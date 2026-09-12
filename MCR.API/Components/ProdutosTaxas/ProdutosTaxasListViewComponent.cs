using Microsoft.AspNetCore.Mvc;
using MCR.API.Produto.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.ProdutosTaxas
{
    public class ProdutosTaxasListViewComponent : BaseViewComponent
    {
        private readonly IProdutosTaxasService _service;

        public ProdutosTaxasListViewComponent(IProdutosTaxasService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid produtoId, int page = 1, int pageSize = 10000)
        {
            var entities = await _service.ObterPorProdutoIdAsync(produtoId);
            var dto = entities.Select(e => new ProdutosTaxasDTO
            {
                ProdutoTaxaId = e.Id,
                ProdutoId = e.ProdutoId,
                UF = e.UF,
                Municipio = e.Municipio,
                ProdutividadeEsperada = e.ProdutividadeEsperada,
                TaxaNc65 = e.TaxaNc65,
                TaxaNc70 = e.TaxaNc70,
                TaxaNc75 = e.TaxaNc75,
                Cpf = e.Cpf,
                Ativo = e.Ativo
            }).ToList();
            return View(dto);
        }
    }
}
