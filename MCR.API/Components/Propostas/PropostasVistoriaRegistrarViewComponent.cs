using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasVistoriaRegistrarViewComponent : ViewComponent
    {
        private readonly DbContextMCR _context;
        private readonly IPropostasService _propostasService;

        public PropostasVistoriaRegistrarViewComponent(DbContextMCR context, IPropostasService propostasService)
        {
            _context = context;
            _propostasService = propostasService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId, string clienteId)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var dto = new PropostasVistoriaCadastrarDTO(propostaId, Guid.TryParse(clienteId, out var cId) ? cId : null);

            var segurados = await _context.PropostasSegurados
                .Where(s => s.PropostaId == propostaId)
                .ToListAsync();

            foreach (var seg in segurados)
            {
                dto.PessoasDisponiveis.Add(new PropostasVistoriaListaDTO
                {
                    PropostasSeguradoId = seg.Id,
                    VinculoFamiliarId = seg.VinculoFamiliarId,
                    Nome = seg.Nome,
                    CPF = seg.CPF,
                    Telefone = seg.Telefone,
                    RelacaoParental = seg.RelacaoParental
                });
            }

            return View(dto);
        }
    }
}
