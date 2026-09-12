using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/propostas")]
    [Authorize]
    public class PropostasController : ControllerBase
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasBeneficiariosService _beneficiariosService;
        private readonly IPropostasDocumentosService _documentosService;
        private readonly IPropostasObservacoesService _observacoesService;
        private readonly IPropostasOcorrenciaService _ocorrenciaService;
        private readonly IPropostasStatusService _statusService;
        private readonly IPropostaRiscoService _riscoService;
        private readonly IPropostaSeguradoService _seguradoService;
        private readonly IPropostasQuestionarioService _questionarioService;
        private readonly IPropostasFormaPagamentosService _formaPagamentosService;

        public PropostasController(
            IPropostasService propostasService,
            IPropostasBeneficiariosService beneficiariosService,
            IPropostasDocumentosService documentosService,
            IPropostasObservacoesService observacoesService,
            IPropostasOcorrenciaService ocorrenciaService,
            IPropostasStatusService statusService,
            IPropostaRiscoService riscoService,
            IPropostaSeguradoService seguradoService,
            IPropostasQuestionarioService questionarioService,
            IPropostasFormaPagamentosService formaPagamentosService)
        {
            _propostasService = propostasService;
            _beneficiariosService = beneficiariosService;
            _documentosService = documentosService;
            _observacoesService = observacoesService;
            _ocorrenciaService = ocorrenciaService;
            _statusService = statusService;
            _riscoService = riscoService;
            _seguradoService = seguradoService;
            _questionarioService = questionarioService;
            _formaPagamentosService = formaPagamentosService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos([FromQuery] ObterPropostasPaginadoRequestDTO request)
        {
            var propostas = await _propostasService.ObterTodosPaginadoAsync(request);
            return Ok(ApiResponse<object>.Ok(propostas));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var proposta = await _propostasService.ObterPorIdAsync(id);
            if (proposta == null)
                return NotFound(ApiResponse<object>.Fail("Proposta não encontrada."));
            return Ok(ApiResponse<object>.Ok(proposta));
        }

        [HttpPost("gerar/{cotacaoAgricolaId}")]
        public async Task<IActionResult> GerarProposta(Guid cotacaoAgricolaId)
        {
            var result = await _propostasService.GerarPropostaAsync(cotacaoAgricolaId);
            if (!result.Sucesso)
                return BadRequest(ApiResponse<object>.Fail(result.Mensagem));
            return Ok(ApiResponse<object>.Ok(new { propostaId = result.PropostaId }, result.Mensagem));
        }

        [HttpPost("{id}/transmitir")]
        public async Task<IActionResult> EnviarTransmissao(Guid id)
        {
            var result = await _propostasService.EnviarTransmissaoAsync(id);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Não foi possível transmitir a proposta."));
            return Ok(ApiResponse<object>.Ok(null, "Proposta transmitida com sucesso."));
        }

        [HttpGet("{id}/status-validacao")]
        public async Task<IActionResult> ObterStatusValidacao(Guid id)
        {
            var validacao = await _propostasService.ObterStatusValidacaoAsync(id);
            return Ok(ApiResponse<object>.Ok(validacao));
        }

        [HttpGet("{id}/kml")]
        public async Task<IActionResult> BaixarKmlTalhao([FromQuery] Guid talhaoId)
        {
            var kml = await _propostasService.BaixarKmlTalhaoAsync(talhaoId);
            if (kml == null)
                return NotFound(ApiResponse<object>.Fail("KML não disponível."));
            return File(kml, "application/vnd.google-earth.kml+xml", $"talhao_{talhaoId}.kml");
        }

        // ---- Beneficiarios ----
        [HttpGet("{id}/beneficiarios")]
        public async Task<IActionResult> ObterBeneficiarios(Guid id)
        {
            var beneficiarios = await _beneficiariosService.ObterPorPropostaAsync(id);
            return Ok(ApiResponse<object>.Ok(beneficiarios));
        }

        [HttpPost("{id}/beneficiarios")]
        public async Task<IActionResult> SalvarBeneficiarios(Guid id, [FromBody] List<Guid> beneficiarioIds)
        {
            var dto = new PropostasBeneficiariosDTO
            {
                PropostaId = id,
                PessoasBeneficiarios = beneficiarioIds.Select(b => new PropostaBeneficiarioItemDTO
                {
                    ClienteId = b,
                    Selecionado = true
                }).ToList()
            };
            var result = await _beneficiariosService.SalvarBeneficiariosAsync(dto);
            if (!result.Sucesso)
                return BadRequest(ApiResponse<object>.Fail(result.Mensagem));
            return Ok(ApiResponse<object>.Ok(null, result.Mensagem));
        }

        [HttpDelete("beneficiarios/{vinculoId}")]
        public async Task<IActionResult> RemoverBeneficiario(Guid vinculoId)
        {
            var result = await _beneficiariosService.RemoverBeneficiarioAsync(vinculoId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao remover beneficiário."));
            return Ok(ApiResponse<object>.Ok(null, "Beneficiário removido com sucesso."));
        }

        // ---- Documentos ----
        [HttpGet("{id}/documentos")]
        public async Task<IActionResult> ListarDocumentos(Guid id)
        {
            var docs = await _documentosService.ListarPorPropostaAsync(id);
            return Ok(ApiResponse<object>.Ok(docs));
        }

        [HttpPost("{id}/documentos")]
        public async Task<IActionResult> UploadDocumento(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("Arquivo não enviado."));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var doc = await _documentosService.UploadDocumentoAsync(
                id, file.FileName, file.ContentType, ms.ToArray(), Guid.Parse(userId));
            return Ok(ApiResponse<object>.Ok(doc, "Documento enviado com sucesso."));
        }

        [HttpDelete("documentos/{documentoId}")]
        public async Task<IActionResult> ExcluirDocumento(Guid documentoId)
        {
            var result = await _documentosService.ExcluirDocumentoAsync(documentoId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao excluir documento."));
            return Ok(ApiResponse<object>.Ok(null, "Documento excluído com sucesso."));
        }

        [HttpGet("documentos/{documentoId}/download")]
        public async Task<IActionResult> DownloadDocumento(Guid documentoId)
        {
            var (conteudo, nome, contentType) = await _documentosService.DownloadDocumentoAsync(documentoId);
            return File(conteudo, contentType, nome);
        }

        // ---- Observacoes ----
        [HttpGet("{id}/observacoes")]
        public async Task<IActionResult> ObterObservacoes(Guid id)
        {
            var obs = await _observacoesService.ObterPorPropostaAsync(id);
            return Ok(ApiResponse<object>.Ok(obs));
        }

        [HttpPost("{id}/observacoes")]
        public async Task<IActionResult> SalvarObservacao(Guid id, [FromBody] string observacao)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _observacoesService.SalvarObservacaoAsync(id, observacao, Guid.Parse(userId));
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao salvar observação."));
            return Ok(ApiResponse<object>.Ok(null, "Observação salva com sucesso."));
        }

        // ---- Ocorrencias ----
        [HttpGet("{id}/ocorrencias")]
        public async Task<IActionResult> ObterOcorrencias(Guid id)
        {
            var ocorrencias = await _ocorrenciaService.ObterPorPropostaAsync(id);
            return Ok(ApiResponse<object>.Ok(ocorrencias));
        }

        [HttpPost("{id}/ocorrencias")]
        public async Task<IActionResult> SalvarOcorrencia(Guid id, [FromForm] string descricao, IFormFile anexo = null)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            byte[] anexoBytes = null;
            string nomeAnexo = null;
            if (anexo != null)
            {
                using var ms = new MemoryStream();
                await anexo.CopyToAsync(ms);
                anexoBytes = ms.ToArray();
                nomeAnexo = anexo.FileName;
            }

            var result = await _ocorrenciaService.SalvarOcorrenciaAsync(id, descricao, Guid.Parse(userId), anexoBytes, nomeAnexo);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao salvar ocorrência."));
            return Ok(ApiResponse<object>.Ok(null, "Ocorrência salva com sucesso."));
        }

        [HttpGet("ocorrencias/{ocorrenciaId}/download")]
        public async Task<IActionResult> DownloadAnexoOcorrencia(Guid ocorrenciaId)
        {
            var (conteudo, nome) = await _ocorrenciaService.DownloadAnexoAsync(ocorrenciaId);
            return File(conteudo, "application/octet-stream", nome);
        }

        // ---- Status ----
        [HttpGet("{id}/status")]
        public async Task<IActionResult> ObterStatus(Guid id)
        {
            var status = await _statusService.ObterStatusPorPropostaAsync(id);
            return Ok(ApiResponse<object>.Ok(status));
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> SalvarStatus(Guid id, [FromBody] SalvarStatusRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var result = await _statusService.SalvarStatusAsync(id, request.Status, request.Observacao, Guid.Parse(userId));
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao salvar status."));
            return Ok(ApiResponse<object>.Ok(null, "Status salvo com sucesso."));
        }

        [HttpGet("status/{statusId}/download")]
        public async Task<IActionResult> DownloadAnexoStatus(Guid statusId)
        {
            var (conteudo, nome) = await _statusService.DownloadAnexoStatusAsync(statusId);
            return File(conteudo, "application/octet-stream", nome);
        }

        // ---- Riscos ----
        [HttpPost("{id}/riscos")]
        public async Task<IActionResult> VincularRiscos(Guid id, [FromBody] List<Guid> riscoIds)
        {
            var result = await _riscoService.VincularRiscosAsync(id, riscoIds);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao vincular riscos."));
            return Ok(ApiResponse<object>.Ok(null, "Riscos vinculados com sucesso."));
        }

        [HttpDelete("riscos/{vinculoId}")]
        public async Task<IActionResult> RemoverRisco(Guid vinculoId)
        {
            var result = await _riscoService.RemoverRiscoAsync(vinculoId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao remover risco."));
            return Ok(ApiResponse<object>.Ok(null, "Risco removido com sucesso."));
        }

        // ---- Segurados ----
        [HttpPost("{id}/segurados")]
        public async Task<IActionResult> VincularSegurado(Guid id, [FromBody] Guid clienteId)
        {
            var result = await _seguradoService.VincularSeguradoAsync(id, clienteId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao vincular segurado."));
            return Ok(ApiResponse<object>.Ok(null, "Segurado vinculado com sucesso."));
        }

        [HttpDelete("segurados/{vinculoId}")]
        public async Task<IActionResult> RemoverSegurado(Guid vinculoId)
        {
            var result = await _seguradoService.RemoverSeguradoAsync(vinculoId);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao remover segurado."));
            return Ok(ApiResponse<object>.Ok(null, "Segurado removido com sucesso."));
        }

        // ---- Questionario ----
        [HttpGet("{id}/questionario")]
        public async Task<IActionResult> ObterQuestionario(Guid id)
        {
            var q = await _questionarioService.ObterQuestionarioAsync(id);
            return Ok(ApiResponse<object>.Ok(q));
        }

        [HttpPost("{id}/questionario")]
        public async Task<IActionResult> SalvarQuestionario(Guid id, [FromBody] object dados)
        {
            var result = await _questionarioService.SalvarQuestionarioAsync(id, dados);
            if (!result)
                return BadRequest(ApiResponse<object>.Fail("Erro ao salvar questionário."));
            return Ok(ApiResponse<object>.Ok(null, "Questionário salvo com sucesso."));
        }

        // ---- Formas de Pagamento ----
        [HttpGet("{id}/formas-pagamento")]
        public async Task<IActionResult> ObterFormasPagamento(Guid id)
        {
            var formas = await _formaPagamentosService.ObterPorPropostaIdAsync(id);
            return Ok(ApiResponse<object>.Ok(formas));
        }
    }

    public class SalvarStatusRequest
    {
        public string Status { get; set; }
        public string Observacao { get; set; }
    }
}
