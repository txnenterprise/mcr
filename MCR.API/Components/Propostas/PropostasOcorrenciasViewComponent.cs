using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasOcorrenciasViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasOcorrenciaService _ocorrenciaService;

        public PropostasOcorrenciasViewComponent(IPropostasService propostasService, IPropostasOcorrenciaService ocorrenciaService)
        {
            _propostasService = propostasService;
            _ocorrenciaService = ocorrenciaService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var registros = await _ocorrenciaService.ObterPorPropostaAsync(propostaId);

            var dto = registros.Select(r => new PropostasOcorrenciaDTO
            {
                Id = r.Id,
                Descricao = r.Descricao,
                DataCriacao = r.DataCriacao,
                NomeArquivo = r.NomeArquivo,
                CaminhoAnexo = r.CaminhoAnexo,
                NomeUsuario = r.Usuario?.Name
            }).ToList();

            return View(dto);
        }
    }
}
