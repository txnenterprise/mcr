using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasQuestionarioController : Controller
    {
        private readonly IPropostasQuestionarioService _service;

        public PropostasQuestionarioController(IPropostasQuestionarioService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SalvarQuestionario(Guid propostaId, object dados)
        {
            try
            {
                var result = await _service.SalvarQuestionarioAsync(propostaId, dados);
                return Json(new { success = result, message = result ? "Questionário salvo!" : "Erro ao salvar questionário." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }
    }
}
