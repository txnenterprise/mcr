using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasTalhoesViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly DbContextMCR _context;

        public PropostasTalhoesViewComponent(IPropostasService propostasService, DbContextMCR context)
        {
            _propostasService = propostasService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var registros = await _context.PropostasClientePropriedades
                .Where(p => p.PropostaId == propostaId)
                .Include(p => p.PropostasClientePropriedadesTalhoes)
                    .ThenInclude(t => t.GrupoVariedade)
                .Include(p => p.PropostasClientePropriedadesTalhoes)
                    .ThenInclude(t => t.Variedade)
                .ToListAsync();

            var dto = new PropostasTalhoesDTO
            {
                Talhoes = registros.SelectMany(r => r.PropostasClientePropriedadesTalhoes).Select(t => new PropostasRiscoTalhaoDTO
                {
                    PropostaTalhaoId = t.Id,
                    TalhaoId = t.TalhaoId,
                    NomeTalhao = t.Nome,
                    AreaTalhao = t.Area,
                    TipoSolo = t.TipoSolo,
                    ClassificacaoSolo = t.ClassificacaoSolo,
                    DataPlantio = t.DataPlantio,
                    GrupoVariedadeNome = t.GrupoVariedade?.GrupoMaturacao,
                    VariedadeNome = t.Variedade?.Nome
                }).ToList()
            };

            return View(dto);
        }
    }
}
