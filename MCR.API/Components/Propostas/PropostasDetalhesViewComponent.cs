using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasDetalhesViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;

        public PropostasDetalhesViewComponent(IPropostasService propostasService)
        {
            _propostasService = propostasService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            var response = await _propostasService.ObterPropostaDetalhes(propostaId);
            if (response.Sucesso)
            {
                ViewBag.PropostaStatus = response.Proposta!.Status;
                return View(response.Proposta);
            }
            else
            {
                ViewBag.Mensagem = response.Mensagem;
                return View(null);
            }
        }
    }
}
