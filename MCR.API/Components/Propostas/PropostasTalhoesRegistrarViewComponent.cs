using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Propostas
{
    public class PropostasTalhoesRegistrarViewComponent : BaseViewComponent
    {
        private readonly IPropostasService _propostasService;
        private readonly DbContextMCR _context;

        public PropostasTalhoesRegistrarViewComponent(IPropostasService propostasService, DbContextMCR context)
        {
            _propostasService = propostasService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid propostaId, string? propriedadeId = null)
        {
            ViewBag.PropostaStatus = await _propostasService.ObterStatusProposta(propostaId);

            var gruposMaturacao = await _context.CulturaMaturacao
                .Where(g => g.Ativo && !g.Excluido)
                .OrderBy(g => g.GrupoMaturacao)
                .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.GrupoMaturacao
                }).ToListAsync();
            ViewBag.GrupoMaturacao = gruposMaturacao;

            var riscos = await _context.PropostasClientePropriedades
                .Where(p => p.PropostaId == propostaId)
                .Include(p => p.Propriedade)
                .ToListAsync();

            var propriedadeIds = riscos.Select(r => r.PropriedadeId).ToList();
            var todosTalhoes = await _context.Talhoes
                .Where(t => propriedadeIds.Contains(t.PropriedadeId))
                .ToListAsync();

            if (!string.IsNullOrEmpty(propriedadeId) && Guid.TryParse(propriedadeId, out var pId))
                riscos = riscos.Where(p => p.Id == pId || p.PropriedadeId == pId).ToList();

            var dto = new PropostasTalhoesCadastrarDTO
            {
                PropostaId = propostaId,
                Propriedades = riscos.Select(r => new PropostasTalhoesPropriedadeDTO
                {
                    PropriedadeId = r.PropriedadeId,
                    PropostaPropriedadeId = r.Id,
                    Nome = r.Nome,
                    Municipio = $"{r.Cidade}/{r.Estado}",
                    AreaTotal = r.SomaAreaTotalTalhao,
                    Talhoes = todosTalhoes.Where(t => t.PropriedadeId == r.PropriedadeId).Select(t => new PropostasTalhoesItemDTO
                    {
                        TalhaoId = t.Id,
                        PropriedadeId = t.PropriedadeId,
                        NomeTalhao = t.Nome,
                        AreaTalhao = t.Area,
                        TipoSolo = t.TipoSolo,
                        ClassificacaoSolo = t.ClassificacaoSolo
                    }).ToList()
                }).ToList()
            };

            return View(dto);
        }
    }
}
