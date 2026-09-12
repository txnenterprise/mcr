using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/estruturas-negocio")]
    [Authorize]
    public class EstruturasNegocioController : ControllerBase
    {
        [HttpGet]
        public IActionResult Obter()
        {
            return Ok(ApiResponse<object>.Ok(null, "Endpoint pendente de implementação do serviço."));
        }
    }

    [ApiController]
    [Route("api/estruturas-risco")]
    [Authorize]
    public class EstruturasRiscoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Obter()
        {
            return Ok(ApiResponse<object>.Ok(null, "Endpoint pendente de implementação do serviço."));
        }
    }
}
