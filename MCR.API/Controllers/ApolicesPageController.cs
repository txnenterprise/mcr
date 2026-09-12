using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    [Route("Apolices")]
    public class ApolicesPageController : Controller
    {
        private readonly IPropostasService _propostasService;

        public ApolicesPageController(IPropostasService propostasService)
        {
            _propostasService = propostasService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var request = new ObterPropostasPaginadoRequestDTO
            {
                Page = page,
                PageSize = pageSize,
                StatusFilter = new List<string> { "Apólice emitida" }
            };
            var retorno = (await _propostasService.ObterTodosPaginadoAsync(request)).ToList();

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

            return View("~/Views/Apolices/Index.cshtml", model);
        }

        [Route("Cadastrar/{id?}")]
        public async Task<IActionResult> Cadastrar(Guid? id)
        {
            if (!id.HasValue)
                return View("~/Views/Apolices/Cadastrar.cshtml");

            var proposta = await _propostasService.ObterPorIdAsync(id.Value);
            if (proposta == null)
                return NotFound();

            var model = new PropostasCadastrarDTO
            {
                PropostaId = proposta.Id,
                ClienteId = proposta.ClienteId.ToString(),
                Status = proposta.Status
            };
            ViewBag.PropostaStatus = proposta.Status;

            return View("~/Views/Propostas/Cadastrar.cshtml", model);
        }
    }
}
