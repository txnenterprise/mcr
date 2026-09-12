using Microsoft.AspNetCore.Mvc;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasBeneficiariosViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly IPropostasBeneficiariosService _beneficiariosService;

        public PropostasBeneficiariosViewComponent(IPropostasService propostasService, IPropostasBeneficiariosService beneficiariosService)
        {
            _propostasService = propostasService;
            _beneficiariosService = beneficiariosService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var registros = await _beneficiariosService.ObterPorPropostaAsync(propostaId);

            var dto = new PropostasBeneficiariosDTO
            {
                PessoasBeneficiarios = registros.Select(r => new PropostaBeneficiarioItemDTO
                {
                    Id = r.Id,
                    ClienteId = r.ClienteId,
                    Nome = r.Nome,
                    Documento = r.Documento,
                    Percentual = r.Percentual,
                    Banco = r.Banco,
                    Agencia = r.Agencia,
                    Conta = r.Conta,
                    ChavePIX = r.ChavePIX
                }).ToList()
            };

            return View(dto);
        }
    }
}
