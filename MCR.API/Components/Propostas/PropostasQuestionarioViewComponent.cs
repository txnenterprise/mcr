using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasQuestionarioViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasQuestionarioService _questionarioService;

        public PropostasQuestionarioViewComponent(IPropostasService propostasService, IPropostasQuestionarioService questionarioService)
        {
            _propostasService = propostasService;
            _questionarioService = questionarioService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var culturas = await _questionarioService.ListarCulturasAsync();
            ViewBag.Culturas = new SelectList(culturas.Select(x => new { id = x.Id, text = x.Text }).OrderBy(x => x.text).ToList(), "id", "text");

            var resultado = await _questionarioService.ObterQuestionarioAsync(propostaId);

            if (resultado is PropostasQuestionarioDTO dto)
            {
                return View(dto);
            }

            return View(new PropostasQuestionarioDTO { PropostaId = propostaId });
        }
    }
}
