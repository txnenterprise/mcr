using Microsoft.AspNetCore.Mvc;

namespace MCR.API.Components.Propostas
{
    public class PropostasStatusViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(Guid propostaId)
        {
            return Content("");
        }
    }
}
