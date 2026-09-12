using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models.ViewModels;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class EstruturaRiscoController : Controller
    {
        private readonly IPropostasService _propostasService;

        public EstruturaRiscoController(IPropostasService propostasService)
        {
            _propostasService = propostasService;
        }

        public async Task<IActionResult> Index(string pesquisaNome, string pesquisaCPF, string pesquisaAtivoInativo, int page = 1, int pageSize = 10)
        {
            var request = new ObterPropostasPaginadoRequestDTO
            {
                Page = page,
                PageSize = pageSize
            };

            var retorno = (await _propostasService.ObterTodosPaginadoAsync(request)).ToList();

            var model = new EstruturaRiscoModel
            {
                PesquisaNome = pesquisaNome,
                PesquisaCPF = pesquisaCPF,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                ListaEstruturaRisco = retorno.Select(p => new EstruturaRiscoItem
                {
                    Cliente = new EstruturaRiscoCliente
                    {
                        Nome = p.Cliente?.Nome,
                        CPF = p.Cliente?.CPF,
                        Telefone = p.Cliente?.Telefone,
                        Cidade = p.Municipio,
                        Estado = p.Estado
                    }
                }).ToList()
            };

            return View(model);
        }
    }
}
