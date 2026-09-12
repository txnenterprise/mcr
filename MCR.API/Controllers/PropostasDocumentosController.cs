using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class PropostasDocumentosController : Controller
    {
        private readonly IPropostasDocumentosService _service;
        private readonly IPropostasService _propostasService;

        public PropostasDocumentosController(IPropostasDocumentosService service, IPropostasService propostasService)
        {
            _service = service;
            _propostasService = propostasService;
        }

        [HttpPost("SalvarDocumento")]
        public async Task<IActionResult> SalvarDocumento(Guid propostaId, string nome, IFormFile file)
        {
            return await UploadFileAction(propostaId, nome, file);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            return await UploadFileAction(Guid.Empty, file?.FileName ?? "arquivo", file);
        }

        private async Task<IActionResult> UploadFileAction(Guid propostaId, string nome, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "Nenhum arquivo enviado." });

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var usuarioIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var usuarioId = Guid.TryParse(usuarioIdStr, out var uid) ? uid : Guid.Empty;

                var result = await _service.UploadDocumentoAsync(propostaId, nome, file.ContentType, ms.ToArray(), usuarioId, await ObterStatusProposta(propostaId));
                return Json(new { success = result != null, message = result != null ? "Documento salvo com sucesso!" : "Erro ao salvar documento." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Listar(Guid propostaId)
        {
            try
            {
                var docs = await _service.ListarPorPropostaAsync(propostaId);
                return Json(new { success = true, data = docs });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(Guid id)
        {
            try
            {
                var result = await _service.ExcluirDocumentoAsync(id);
                return Json(new { success = result, message = result ? "Documento excluído!" : "Erro ao excluir." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var (conteudo, nome, contentType) = await _service.DownloadDocumentoAsync(id);
                if (conteudo == null)
                    return Json(new { success = false, message = "Documento não encontrado." });

                return File(conteudo, contentType, nome);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private async Task<string?> ObterStatusProposta(Guid propostaId)
        {
            var proposta = await _propostasService.ObterPorIdAsync(propostaId);
            return proposta?.Status;
        }
    }
}
