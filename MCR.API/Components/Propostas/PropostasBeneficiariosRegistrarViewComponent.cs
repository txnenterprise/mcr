using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;
using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Components.Propostas
{
    public class PropostasBeneficiariosRegistrarViewComponent : BaseViewComponent
    {
        private readonly IPropostasBeneficiariosService _propostasBeneficiariosService;
        private readonly IPropostasService _propostasService;

        public PropostasBeneficiariosRegistrarViewComponent(
            IPropostasBeneficiariosService propostasBeneficiariosService,
            IPropostasService propostasService)
        {
            _propostasBeneficiariosService = propostasBeneficiariosService;
            _propostasService = propostasService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);
            var dto = await _propostasBeneficiariosService.ObterBeneficiariosPorPropostaAsync(propostaId);
            return View(dto);
        }
    }
}
