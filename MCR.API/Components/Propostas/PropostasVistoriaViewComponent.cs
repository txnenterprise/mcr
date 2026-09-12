using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasVistoriaViewComponent : BaseViewComponent
    {
        private readonly IPropostasService propostasService;
        private readonly DbContextMCR _context;

        public PropostasVistoriaViewComponent(IPropostasService propostasService, DbContextMCR context)
        {
            this.propostasService = propostasService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId, string clienteId)
        {
            ViewBag.PropostaStatus = await propostasService.ObterStatusProposta(propostaId);

            var registros = await _context.PropostasVistoria
                .Where(v => v.PropostaId == propostaId)
                .ToListAsync();

            var dto = new PropostasVistoriaDTO
            {
                PessoasVistoria = registros.Select(r => new PropostasVistoriaPessoaDTO
                {
                    Id = r.Id,
                    Nome = r.Nome,
                    CPF = r.CPF,
                    Telefone = r.Telefone
                }).ToList()
            };

            return View(dto);
        }
    }
}
