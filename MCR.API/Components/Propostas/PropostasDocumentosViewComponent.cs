using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasDocumentosViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasDocumentosService _documentosService;

        public PropostasDocumentosViewComponent(IPropostasService propostasService, IPropostasDocumentosService documentosService)
        {
            _propostasService = propostasService;
            _documentosService = documentosService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var registros = await _documentosService.ListarPorPropostaAsync(propostaId);

            var dto = registros.Select(r => new PropostasDocumentosListaDTO
            {
                Id = r.Id,
                PropostaId = r.PropostaId,
                TipoDocumento = r.TipoDocumento,
                StatusProposta = r.StatusProposta,
                DataUpload = r.DataUpload,
                NomeArquivo = r.NomeArquivo,
                NomeUsuario = r.Usuario?.Name,
                UrlDocumento = r.UrlDocumento
            }).ToList();

            return View(dto);
        }
    }
}
