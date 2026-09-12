using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasSeguradosViewComponent : BaseViewComponent
    {
        private readonly IPropostaSeguradoService propostaSeguradoService;
        private readonly IPropostasService propostasService;
        public PropostasSeguradosViewComponent(IPropostaSeguradoService propostaSeguradoService, IPropostasService propostasService)
        {
            this.propostaSeguradoService = propostaSeguradoService;
            this.propostasService = propostasService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await propostasService.ObterStatusProposta(propostaId);
            return View(await propostaSeguradoService.ObterPropostaSeguradosPorId(propostaId));
        }
    }
}
