using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasStatusController : Controller
    {
        private readonly IPropostasStatusService _service;

        public PropostasStatusController(IPropostasStatusService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarStatus([FromForm] PropostasStatusDTO model)
        {
            var usuarioId = GetUserId();
            var ehNavegacaoBrowser = Request.Headers.Accept.ToString().Contains("text/html", StringComparison.OrdinalIgnoreCase);

            if (!usuarioId.HasValue)
            {
                if (ehNavegacaoBrowser)
                    return Redirect("/Propostas");
                return Json(new { success = false, message = "Usuário não autenticado" });
            }

            var (sucesso, mensagem) = await _service.SalvarStatusProposta(model, usuarioId.Value);

            if (ehNavegacaoBrowser)
                return Redirect(ListaPorStatus(model.Status));

            return Json(new { success = sucesso, message = mensagem, status = model.Status });
        }

        private static string ListaPorStatus(string status)
        {
            var s = (status ?? string.Empty).ToLowerInvariant();
            if (s.Contains("apólice") || s.Contains("apolice")) return "/Apolices";
            if (s.Contains("sinistro") || s.Contains("comunicar_sinistro") || s.Contains("deferido") || s.Contains("indeferido")) return "/Sinistro";
            if (s.Contains("endosso")) return "/Endosso";
            return "/Propostas";
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadPropostaAceita(Guid statusId)
        {
            return await DownloadAnexo(statusId);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DownloadApoliceEmitida(Guid statusId)
        {
            return await DownloadAnexo(statusId);
        }

        private async Task<IActionResult> DownloadAnexo(Guid statusId)
        {
            try
            {
                var (conteudo, nome) = await _service.DownloadAnexoStatusAsync(statusId);
                if (conteudo == null || conteudo.Length == 0)
                    return NotFound("Anexo não encontrado.");
                return File(conteudo, "application/octet-stream", nome ?? "anexo.bin");
            }
            catch
            {
                return StatusCode(500, "Erro interno do servidor ao processar o download.");
            }
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (claim != null && Guid.TryParse(claim.Value, out var id))
                return id;
            return null;
        }
    }
}
