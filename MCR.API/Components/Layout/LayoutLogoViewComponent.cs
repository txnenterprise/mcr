using Microsoft.AspNetCore.Mvc;

namespace MCR.API.Components.Layout
{
    public class LayoutLogoViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Guid? cotacaoId = null)
        {
            ViewData["LogoParceiro"] = "";
            ViewData["CorPredominante"] = "#007bff";
            ViewData["NomeParceiro"] = null;

            return View();
        }
    }
}
