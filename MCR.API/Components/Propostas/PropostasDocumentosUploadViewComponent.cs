using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;

namespace MCR.API.Components.Propostas
{
    public class PropostasDocumentosUploadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Guid propostaId)
        {
            return View(new PropostasDocumentosUploadDTO(propostaId));
        }
    }
}
