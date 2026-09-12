using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasObservacoesViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasObservacoesService _observacoesService;

        public PropostasObservacoesViewComponent(IPropostasService propostasService, IPropostasObservacoesService observacoesService)
        {
            _propostasService = propostasService;
            _observacoesService = observacoesService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var registros = await _observacoesService.ObterPorPropostaAsync(propostaId);

            var dto = new PropostasObservacoesDTO
            {
                PropostaId = propostaId,
                Observacoes = registros.Select(r => new PropostaObservacaoItemDTO
                {
                    Id = r.Id,
                    Observacao = r.Observacao,
                    DataCriacao = r.DataCriacao,
                    NomeUsuario = r.Usuario?.Name
                }).ToList()
            };

            return View(dto);
        }
    }
}
