using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasTimelineViewComponent : BaseViewComponent
    {
        private readonly IPropostasService propostasService;
        private readonly DbContextMCR _context;

        public PropostasTimelineViewComponent(IPropostasService propostasService, DbContextMCR context)
        {
            this.propostasService = propostasService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId)
        {
            ViewBag.PropostaStatus = await propostasService.ObterStatusProposta(propostaId);
            ViewBag.PropostaId = propostaId;

            var registros = await _context.PropostasStatus
                .Where(s => s.PropostaId == propostaId)
                .OrderByDescending(s => s.DataStatus)
                .Include(s => s.Usuario)
                .ToListAsync();

            var timeline = registros.Select(r => new PropostasTimelineDTO
            {
                Id = r.Id,
                PropostaId = r.PropostaId,
                Data = r.DataStatus,
                Tipo = "Status",
                Titulo = r.Status ?? "",
                Descricao = r.Status ?? "",
                Status = r.Status,
                StatusAnterior = r.StatusAnterior,
                NomeUsuario = r.Usuario?.UserName,
                UsuarioPerfil = r.UsuarioPerfil
            }).ToList();

            return View(timeline);
        }
    }
}
