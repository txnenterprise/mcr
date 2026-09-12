using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasFormaPagamentosViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasFormaPagamentosService _formaPagamentosService;

        public PropostasFormaPagamentosViewComponent(IPropostasService propostasService, IPropostasFormaPagamentosService formaPagamentosService)
        {
            _propostasService = propostasService;
            _formaPagamentosService = formaPagamentosService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var registros = await _formaPagamentosService.ObterPorPropostaIdAsync(propostaId);

            var dto = registros.Select(r => new PropostasFormaPagamentosDTO
            {
                Id = r.Id,
                ProdutoName = r.Produto?.NomeProduto,
                FormaDePagamento = r.FormaDePagamento,
                Parcelamento = r.Parcelamento
            }).ToList();

            return View(dto);
        }
    }
}
