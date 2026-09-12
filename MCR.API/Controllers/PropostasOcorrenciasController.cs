using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class PropostasOcorrenciasController : Controller
    {
        private readonly IPropostasOcorrenciaService _service;

        public PropostasOcorrenciasController(IPropostasOcorrenciaService service)
        {
            _service = service;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarOcorrencia(Guid propostaId, string descricao, IFormFile anexo = null)
        {
            var usuarioId = GetUserId();
            if (!usuarioId.HasValue)
                return Json(new { success = false, message = "Usuário não autenticado" });

            byte[] anexoBytes = null;
            string nomeAnexo = null;
            if (anexo != null && anexo.Length > 0)
            {
                using var ms = new MemoryStream();
                await anexo.CopyToAsync(ms);
                anexoBytes = ms.ToArray();
                nomeAnexo = anexo.FileName;
            }

            var resultado = await _service.SalvarOcorrenciaAsync(propostaId, descricao, usuarioId.Value, anexoBytes, nomeAnexo);
            return Json(new { success = resultado, message = "Notificação registrada com sucesso!" });
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAnexoOcorrencia(Guid ocorrenciaId)
        {
            try
            {
                var (conteudo, nome) = await _service.DownloadAnexoAsync(ocorrenciaId);
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
