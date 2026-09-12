using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class EstruturaNegocioController : Controller
    {
        private readonly IEstruturaNegocioService _service;

        public EstruturaNegocioController(IEstruturaNegocioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string pesquisaRazaoSocial,
            string pesquisaAtivoInativo,
            string pesquisaRazaoSocialPA,
            string pesquisaAtivoInativoPA,
            int pagina = 1,
            int pageSize = 10)
        {
            var retorno = await _service.PesquisarCanaisPontosAtendimento(
                pesquisaRazaoSocial, pesquisaAtivoInativo,
                pesquisaRazaoSocialPA, pesquisaAtivoInativoPA,
                pagina, pageSize);

            var model = new Models.ViewModels.CanalModel
            {
                PesquisaRazaoSocial = pesquisaRazaoSocial,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PesquisaRazaoSocialPA = pesquisaRazaoSocialPA,
                PesquisaAtivoInativoPA = pesquisaAtivoInativoPA,
                PaginaAtual = pagina,
                TotalPaginas = retorno != null && retorno.Count > 0 && retorno[0] != null ? retorno[0].TotalPages : 0,
                ListaCanais = retorno?.ToList() ?? new()
            };

            return View(model);
        }
    }
}
