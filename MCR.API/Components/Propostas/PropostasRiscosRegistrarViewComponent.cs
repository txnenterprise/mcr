using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasRiscosRegistrarViewComponent : ViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly DbContextMCR _context;

        public PropostasRiscosRegistrarViewComponent(IPropostasService propostasService, DbContextMCR context)
        {
            _propostasService = propostasService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId, string clienteId, string cidade)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            if (string.IsNullOrEmpty(cidade))
            {
                var proposta = await _context.Propostas.FindAsync(propostaId);
                cidade = proposta?.Municipio;
            }

            ViewBag.MunicipioProposta = cidade;

            var dto = new PropostasRiscosCadastrarDTO
            {
                PropostaId = propostaId,
                ClienteId = Guid.TryParse(clienteId, out var cId) ? cId : null
            };

            if (dto.ClienteId.HasValue)
            {
                var propriedades = await _context.VinculosPropriedadesClientes
                    .Where(v => v.ClienteId == dto.ClienteId.Value)
                    .Include(v => v.Propriedade)
                    .ToListAsync();

                dto.Riscos = propriedades.Select(v => new PropostaRiscoCadastrarItemDTO
                {
                    PropriedadeId = v.PropriedadeId,
                    NomePropriedade = v.Propriedade?.Nome,
                    Municipio = v.Propriedade?.Cidade,
                    UF = v.Propriedade?.Estado,
                    AreaTotal = v.Propriedade?.SomaAreaTotalTalhao
                }).ToList();
            }

            return View(dto);
        }
    }
}
