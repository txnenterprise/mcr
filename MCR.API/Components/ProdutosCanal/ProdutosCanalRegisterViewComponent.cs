using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Produto.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.ProdutosCanal
{
    public class ProdutosCanalRegisterViewComponent : BaseViewComponent
    {
        private readonly DbContextMCR _context;

        public ProdutosCanalRegisterViewComponent(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? produtoCanalId, Guid produtoId)
        {
            var canais = await _context.Canais
                .Where(c => c.Ativo && !c.Excluido)
                .OrderBy(c => c.NomeFantasia)
                .Select(c => new { Id = c.Id, Text = c.NomeFantasia })
                .ToListAsync();

            var pontosAtendimento = await _context.PontosAtendimento
                .Where(p => p.Ativo && !p.Excluido)
                .OrderBy(p => p.NomeFantasia)
                .Select(p => new { Id = p.Id, Text = p.NomeFantasia })
                .ToListAsync();

            var model = new ProdutosCanalRegisterDTO
            {
                ProdutoId = produtoId,
                Canais = canais.Select(c => new CanalDTO { Id = c.Id, Text = c.Text }).ToList(),
                PontoAtendimento = pontosAtendimento.Select(p => new PontoAtendimentoDTO { Id = p.Id, Text = p.Text }).ToList()
            };

            return View(model);
        }
    }
}
