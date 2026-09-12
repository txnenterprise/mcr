using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    [Route("Propostas")]
    public class PropostasPageController : Controller
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasStatusService _propostasStatusService;
        private readonly IPropostaSeguradoService _propostaSeguradoService;
        private readonly ILogger<PropostasPageController> _logger;

        public PropostasPageController(
            IPropostasService propostasService,
            IPropostasStatusService propostasStatusService,
            IPropostaSeguradoService propostaSeguradoService,
            ILogger<PropostasPageController> logger)
        {
            _propostasService = propostasService;
            _propostasStatusService = propostasStatusService;
            _propostaSeguradoService = propostaSeguradoService;
            _logger = logger;
        }

        [HttpGet("test")]
        public IActionResult TestPage()
        {
            return Content("<h1>Test OK</h1><p>Razor funciona</p>", "text/html");
        }

        [HttpGet("test-razor")]
        public IActionResult TestRazor()
        {
            return View("~/Views/Propostas/Index.cshtml", new ObterPropostasPaginadoResponseDTO());
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            _logger.LogWarning("=== PropostasIndex START === page={Page}", page);
            try
            {
                var request = new ObterPropostasPaginadoRequestDTO
                {
                    Page = page,
                    PageSize = pageSize,
                    StatusFilter = new List<string> { "Proposta em negociação", "Proposta com pendência", "Aguardando transmissão", "Proposta transmitida (Em análise)", "Proposta aceita → Devolutiva da Seguradora", "Proposta recusada → Devolutiva da Seguradora", "Proposta cancelada" }
                };
                var retorno = (await _propostasService.ObterTodosPaginadoAsync(request)).ToList();
                _logger.LogWarning("PropostasIndex: {Count} propostas carregadas", retorno.Count);

                var model = new ObterPropostasPaginadoResponseDTO
                {
                    Propostas = retorno.Select(p => new PropostaItemDTO
                    {
                        Id = p.Id.ToString(),
                        CodigoCotacao = p.CodigoCotacao ?? p.CodigoProposta.ToString(),
                        PontoAtendimento = p.PontoAtendimento?.RazaoSocial ?? string.Empty,
                        Seguradora = p.PropostasProdutos.FirstOrDefault()?.Seguradora?.RazaoSocial ?? string.Empty,
                        NomeProponente = p.Cliente?.Nome ?? "-",
                        NomeRisco = p.Cultura?.Nome ?? string.Empty,
                        AreaTotalRisco = p.AreaTotal,
                        MunicipioUF = $"{p.Municipio}/{p.Estado}",
                        Status = p.Status ?? "-"
                    }).ToList(),
                    PaginaAtual = page,
                    TotalPages = (int)Math.Ceiling(retorno.Count / (double)pageSize),
                    TamanhoPagina = pageSize
                };

                _logger.LogWarning("PropostasIndex: returning View with {Count} items", model.Propostas.Count);
                
                try
                {
                    return View("~/Views/Propostas/Index.cshtml", model);
                }
                catch (Exception viewEx)
                {
                    _logger.LogError(viewEx, "PropostasIndex: RAZOR VIEW EXCEPTION");
                    return Content($"RAZOR ERROR: {viewEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== PropostasIndex EXCEPTION ===");
                throw;
            }
        }

        [Route("Cadastrar/{id?}")]
        public async Task<IActionResult> Cadastrar(Guid? id)
        {
            _logger.LogWarning("Cadastrar called with id={Id}, HasValue={HasValue}", id, id.HasValue);
            var model = new PropostasCadastrarDTO();

            if (id.HasValue)
            {
                var proposta = await _propostasService.ObterPorIdAsync(id.Value);
                if (proposta == null)
                    return NotFound();

                _logger.LogWarning("Cadastrar setting PropostaId={PropostaId}", proposta.Id);
                model.PropostaId = proposta.Id;
                model.ClienteId = proposta.ClienteId.ToString();
                model.Status = proposta.Status;
                ViewBag.PropostaStatus = proposta.Status;
            }

            return View("~/Views/Propostas/Cadastrar.cshtml", model);
        }

        [HttpGet("PropostasSegurados")]
        public IActionResult PropostasSegurados(Guid propostaId) =>
            ViewComponent("PropostasSegurados", new { propostaId });

        [HttpGet("PropostasSeguradosRegistrar")]
        public IActionResult PropostasSeguradosRegistrar(Guid propostaId) =>
            ViewComponent("PropostasSeguradosRegistrar", new { propostaId });

        [HttpGet("PropostasSeguradosListar")]
        public IActionResult PropostasSeguradosListar(string clienteId) =>
            ViewComponent("PropostasSeguradosListar", new { clienteId });

        [HttpGet("PropostasRiscos")]
        public IActionResult PropostasRiscos(Guid propostaId, string clienteId) =>
            ViewComponent("PropostasRiscos", new { propostaId, clienteId });

        [HttpGet("PropostasRiscosRegistrar")]
        public IActionResult PropostasRiscosRegistrar(Guid propostaId, string clienteId, string cidade) =>
            ViewComponent("PropostasRiscosRegistrar", new { propostaId, clienteId, cidade });

        [HttpGet("PropostasTalhoes")]
        public IActionResult PropostasTalhoes(Guid propostaId) =>
            ViewComponent("PropostasTalhoes", new { propostaId });

        [HttpGet("PropostasTalhoesRegistrar")]
        public IActionResult PropostasTalhoesRegistrar(Guid propostaId, string propriedadeId) =>
            ViewComponent("PropostasTalhoesRegistrar", new { propostaId, propriedadeId });

        [HttpGet("PropostasVistoria")]
        public IActionResult PropostasVistoria(Guid propostaId, string clienteId) =>
            ViewComponent("PropostasVistoria", new { propostaId, clienteId });

        [HttpGet("PropostasVistoriaRegistrar")]
        public IActionResult PropostasVistoriaRegistrar(Guid propostaId, string clienteId) =>
            ViewComponent("PropostasVistoriaRegistrar", new { propostaId, clienteId });

        [HttpGet("PropostasBeneficiarios")]
        public IActionResult PropostasBeneficiarios(Guid propostaId) =>
            ViewComponent("PropostasBeneficiarios", new { propostaId });

        [HttpGet("PropostasBeneficiariosRegistrar")]
        public IActionResult PropostasBeneficiariosRegistrar(Guid propostaId) =>
            ViewComponent("PropostasBeneficiariosRegistrar", new { propostaId });

        [HttpGet("PropostasObservacoes")]
        public IActionResult PropostasObservacoes(Guid propostaId) =>
            ViewComponent("PropostasObservacoes", new { propostaId });

        [HttpGet("PropostasDocumentos")]
        public IActionResult PropostasDocumentos(Guid propostaId) =>
            ViewComponent("PropostasDocumentos", new { propostaId });

        [HttpGet("PropostasFormaPagamentos")]
        public IActionResult PropostasFormaPagamentos(Guid propostaId) =>
            ViewComponent("PropostasFormaPagamentos", new { propostaId });

        [HttpGet("PropostasQuestionario")]
        public IActionResult PropostasQuestionario(Guid propostaId) =>
            ViewComponent("PropostasQuestionario", new { propostaId });

        [HttpGet("PropostasTimeline")]
        public IActionResult PropostasTimeline(Guid propostaId) =>
            ViewComponent("PropostasTimeline", new { propostaId });

        [HttpGet("PropostasStatus")]
        public IActionResult PropostasStatus(Guid propostaId) =>
            ViewComponent("PropostasStatus", new { propostaId });

        [HttpGet("PropostasOcorrencias")]
        public IActionResult PropostasOcorrencias(Guid propostaId) =>
            ViewComponent("PropostasOcorrencias", new { propostaId });

        [HttpGet("PropostasOcorrenciasRegistrar")]
        public IActionResult PropostasOcorrenciasRegistrar(Guid propostaId) =>
            ViewComponent("PropostasOcorrenciasRegistrar", new { propostaId });

        [HttpGet("PropostasStatusCadastrar")]
        public async Task<IActionResult> PropostasStatusCadastrar(Guid propostaId)
        {
            var model = new PropostasStatusDTO(propostaId);

            var statusFiltrados = _propostasStatusService.ListarStatusPropostaFiltrado(propostaId);
            ViewBag.StatusList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                statusFiltrados.Select(x => new { id = x, text = x }).OrderBy(x => x.text).ToList(), "id", "text");

            var statusLista = await _propostasStatusService.ObterPropostaStatusLista(propostaId);
            var ultimoStatus = statusLista?.OrderByDescending(s => s.DataStatus).FirstOrDefault();
            ViewBag.UltimoStatus = ultimoStatus;

            var ultimoRegistro = statusLista?.OrderByDescending(s => s.DataStatus).FirstOrDefault();
            if (ultimoRegistro != null)
            {
                model.ApoliceEmitidaDTO = ultimoRegistro.ApoliceEmitidaDTO;
                model.SinistroComunicarDTO = ultimoRegistro.SinistroComunicarDTO;
                model.EndossoEmitidoDTO = ultimoRegistro.EndossoEmitidoDTO;
                model.JustificativaDTO = ultimoRegistro.JustificativaDTO;
                model.PendenciaDTO = ultimoRegistro.PendenciaDTO;
                model.EndossoSolicitacaoDTO = ultimoRegistro.EndossoSolicitacaoDTO;
                model.EndossoPendenciaDTO = ultimoRegistro.EndossoPendenciaDTO;
                model.EndossoTransmitidoDTO = ultimoRegistro.EndossoTransmitidoDTO;
                model.SinistroPendenciaDTO = ultimoRegistro.SinistroPendenciaDTO;
                model.SinistroCanceladoDTO = ultimoRegistro.SinistroCanceladoDTO;
                model.SinistroAbertoRegulacaoDTO = ultimoRegistro.SinistroAbertoRegulacaoDTO;
                model.SinistroAguardPagamentoDTO = ultimoRegistro.SinistroAguardPagamentoDTO;
                model.SinistroDeferidoPagoDTO = ultimoRegistro.SinistroDeferidoPagoDTO;
                model.SinistroIndeferidoDTO = ultimoRegistro.SinistroIndeferidoDTO;
            }

            ViewBag.CoberturaList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                new List<string> { "Produção", "Replantio" }.Select(x => new { id = x, text = x }).ToList(), "id", "text");
            ViewBag.SeveridadeDanoList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                new List<string> { "Parcial", "Total" }.Select(x => new { id = x, text = x }).ToList(), "id", "text");

            var propostaDetalhes = await _propostasService.ObterPropostaDetalhes(propostaId);
            var proposta = propostaDetalhes.Sucesso ? propostaDetalhes.Proposta : null;
            if (proposta != null)
            {
                ViewBag.PropostaAreaTotal = proposta.AreaTotal;
                ViewBag.PropostaStatus = proposta.Status;
                model.Status = proposta.Status;

                var produtos = proposta.Produtos;
                if (produtos is { Count: > 0 })
                {
                    var culture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
                    model.AceitaDTO = new MCR.API.Propostas.Domain.DTO.PropostasStatusAceitaDTO
                    {
                        LmiTotal = produtos.Sum(p => p.LMIProducaoTotal + p.LMIReplantioTotal).ToString("C2", culture),
                        PremioTotal = produtos.Sum(p => p.PremioTotal).ToString("C2", culture),
                        SubFederal = produtos.Sum(p => p.SubvencaoFederal).ToString("C2", culture),
                        SubEstadual = produtos.Sum(p => p.SubvencaoEstadual).ToString("C2", culture),
                        ParcelaSegurado = Math.Round(produtos.Sum(p => p.ParcelaSegurado), 2)
                    };

                    // Pre-fill EndossoEmitidoDTO se não veio de status anterior
                    if (model.EndossoEmitidoDTO == null)
                    {
                        model.EndossoEmitidoDTO = new MCR.API.Propostas.Domain.DTO.PropostasStatusEndossoEmitidoDTO
                        {
                            LMITotal = produtos.Sum(p => p.LMIProducaoTotal + p.LMIReplantioTotal).ToString("C2", culture),
                            PremioTotal = produtos.Sum(p => p.PremioTotal).ToString("C2", culture),
                            SubFederal = produtos.Sum(p => p.SubvencaoFederal).ToString("C2", culture),
                            SubEstadual = produtos.Sum(p => p.SubvencaoEstadual).ToString("C2", culture),
                            ParcSegurado = Math.Round(produtos.Sum(p => p.ParcelaSegurado), 2),
                            AreaTotal = proposta.AreaTotal
                        };
                    }
                }
            }

            var segurados = await _propostaSeguradoService.ObterPropostaSeguradosPorId(propostaId);
            ViewBag.SeguradosList = segurados;

            return PartialView("~/Views/Propostas/PropostasStatusCadastrar.cshtml", model);
        }

        [HttpPost("GerarProposta")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GerarProposta(Guid cotacaoAgricolaPropostaId)
        {
            var result = await _propostasService.GerarPropostaAsync(cotacaoAgricolaPropostaId);
            return Json(new { success = result.Sucesso, message = result.Mensagem, propostaId = result.PropostaId });
        }

        [HttpGet("ObterStatusValidacao")]
        [Produces("application/json")]
        public async Task<IActionResult> ObterStatusValidacao(Guid propostaId)
        {
            if (propostaId == Guid.Empty)
                return Content("");

            var validacao = await _propostasService.ObterStatusValidacaoAsync(propostaId);
            return Json(validacao);
        }

        [HttpPost("EnviarTransmissao")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarTransmissao(Guid propostaId)
        {
            try
            {
                if (propostaId == Guid.Empty)
                    return Json(new { success = false, message = "Proposta não informada." });

                var result = await _propostasService.EnviarTransmissaoAsync(propostaId);
                if (result)
                    return Json(new { success = true, message = "Proposta enviada para transmissão com sucesso!" });

                return Json(new { success = false, message = "Não foi possível enviar a proposta para transmissão. Verifique se todos os dados estão preenchidos." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao enviar para transmissão: " + ex.Message });
            }
        }
    }
}
