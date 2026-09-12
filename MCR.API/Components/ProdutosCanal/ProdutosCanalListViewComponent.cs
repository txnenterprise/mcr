using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCR.API.Produto.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.ProdutosCanal
{
    public class ProdutosCanalListViewComponent : BaseViewComponent
    {
        private readonly DbContextMCR _context;

        public ProdutosCanalListViewComponent(DbContextMCR context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid produtoId)
        {
            var vinculos = await _context.ProdutosCanalPontosAtendimento
                .Include(x => x.Canal)
                .Include(x => x.PontoAtendimento)
                .Where(x => x.ProdutoId == produtoId)
                .ToListAsync();

            var dto = vinculos.Select(x => new ProdutosCanalListaDTO
            {
                Id = x.Id,
                ProdutoId = x.ProdutoId,
                Canal = x.Canal?.RazaoSocial ?? x.Canal?.NomeFantasia ?? "",
                PontoAtendimento = x.PontoAtendimento?.NomeFantasia ?? ""
            }).ToList();

            return View(dto);
        }
    }
}
