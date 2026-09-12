using Microsoft.AspNetCore.Mvc;
using MCR.API.Produto.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.ProdutosTaxas
{
    public class ProdutosTaxasRegisterViewComponent : BaseViewComponent
    {
        private readonly IProdutosTaxasService _service;

        public ProdutosTaxasRegisterViewComponent(IProdutosTaxasService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? taxaId, Guid produtoId)
        {
            if (taxaId.HasValue)
            {
                var entity = await _service.ObterPorIdAsync(taxaId.Value);
                if (entity != null)
                {
                    var dto = new ProdutosTaxasDTO
                    {
                        ProdutoTaxaId = entity.Id,
                        ProdutoId = entity.ProdutoId,
                        UF = entity.UF,
                        Municipio = entity.Municipio,
                        ProdutividadeEsperada = entity.ProdutividadeEsperada,
                        TaxaNc65 = entity.TaxaNc65,
                        TaxaNc70 = entity.TaxaNc70,
                        TaxaNc75 = entity.TaxaNc75,
                        Cpf = entity.Cpf,
                        Ativo = entity.Ativo
                    };
                    return View(dto);
                }
            }
            return View(new ProdutosTaxasDTO { ProdutoId = produtoId });
        }
    }
}
