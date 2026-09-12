using Microsoft.AspNetCore.Mvc;
using MCR.API.CotacoesAgricola.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.CotacaoAgricola
{
    public class CotacaoAgricolaDadosPropostasViewComponent : BaseViewComponent
    {
        private readonly ICotacoesAgricolaService _cotacoesAgricolaService;

        public CotacaoAgricolaDadosPropostasViewComponent(ICotacoesAgricolaService cotacoesAgricolaService)
        {
            _cotacoesAgricolaService = cotacoesAgricolaService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid cotacaoId, string order = "segurada")
        {
            return View(await _cotacoesAgricolaService.ObterDadosPropostaCotacao(cotacaoId, order));
        }
    }
}
