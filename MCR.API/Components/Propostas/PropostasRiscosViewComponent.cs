using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasRiscosViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly DbContextMCR _context;

        public PropostasRiscosViewComponent(IPropostasService propostasService, DbContextMCR context)
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
                    .AsNoTracking()
                    .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"PropostasRiscos: {registros.Count} riscos, {registros.Sum(r => r.PropostasClientePropriedadesTalhoes.Count)} talhoes");

            var dto = new PropostasRiscosDTO
            {
                Riscos = registros.Select(r => new PropostasRiscoItemDTO
                {
                    PropostaRiscoId = r.Id,
                    PropriedadeId = r.PropriedadeId,
                    NomePropriedade = r.Nome,
                    Municipio = r.Cidade,
                    UF = r.Estado,
                    AreaTotal = r.SomaAreaTotalTalhao,
                    Talhoes = r.PropostasClientePropriedadesTalhoes.Select(t => new PropostasRiscoTalhaoDTO
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
                }).ToList()
            };

            return View(dto);
        }
    }
}
