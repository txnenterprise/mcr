using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class EndossoController : Controller
    {
        private readonly IPropostasService _propostasService;

        public EndossoController(IPropostasService propostasService)
        {
            _propostasService = propostasService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var request = new ObterPropostasPaginadoRequestDTO
            {
                Page = page,
                PageSize = pageSize,
                StatusFilter = new List<string>
                {
                    "Solicitar Endosso", "Endosso com pendência", "Endosso Transmitido", "Endosso Emitido"
                }
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

            return View(model);
        }

        public IActionResult Cadastrar() => View();
    }
}
