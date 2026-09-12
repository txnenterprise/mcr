using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCR.API.Models;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class ProdutosPageController : Controller
    {
        private readonly IProdutosService _produtosService;
        private readonly ISafraService _safraService;
        private readonly ICulturaService _culturaService;
        private readonly ISeguradoraService _seguradoraService;
        private readonly DbContextMCR _context;

        public ProdutosPageController(
            IProdutosService produtosService,
            ISafraService safraService,
            ICulturaService culturaService,
            ISeguradoraService seguradoraService,
            DbContextMCR context)
        {
            _produtosService = produtosService;
            _safraService = safraService;
            _culturaService = culturaService;
            _seguradoraService = seguradoraService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string pesquisaDescricao = null,
            string pesquisaSafra = null,
            string pesquisaCultura = null,
            string pesquisaSeguradora = null,
            string pesquisaAtivoInativo = "Todos",
            int page = 1,
            int pageSize = 10)
        {
            Guid? safraId = null;
            Guid? culturaId = null;
            Guid? seguradoraId = null;

            if (!string.IsNullOrEmpty(pesquisaSafra) && Guid.TryParse(pesquisaSafra, out var sId))
                safraId = sId;
            if (!string.IsNullOrEmpty(pesquisaCultura) && Guid.TryParse(pesquisaCultura, out var cId))
                culturaId = cId;
            if (!string.IsNullOrEmpty(pesquisaSeguradora) && Guid.TryParse(pesquisaSeguradora, out var segId))
                seguradoraId = segId;

            bool? ativo = pesquisaAtivoInativo switch
            {
                "Ativo" => true,
                "Inativo" => false,
                _ => null
            };

            var produtos = (await _produtosService.ObterTodosPaginadoAsync(
                pesquisaDescricao, seguradoraId, safraId, culturaId, ativo, page, pageSize)).ToList();

            var query = _context.Produtos.Where(p => !p.Excluido).AsQueryable();

            if (!string.IsNullOrEmpty(pesquisaDescricao))
                query = query.Where(p => p.NomeProduto.Contains(pesquisaDescricao));
            if (safraId.HasValue)
                query = query.Where(p => p.SafraId == safraId.Value);
            if (culturaId.HasValue)
                query = query.Where(p => p.CulturaId == culturaId.Value);
            if (seguradoraId.HasValue)
                query = query.Where(p => p.SeguradoraId == seguradoraId.Value);
            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var model = new ProdutosViewModel
            {
                ListaProdutos = produtos.Select(p => new ProdutosListaItem
                {
                    Id = p.Id,
                    NomeProduto = p.NomeProduto,
                    Modalidade = p.Modalidade,
                    Ativo = p.Ativo,
                    SafraAno = p.Safra?.AnoReferencia ?? "",
                    CulturaNome = p.Cultura?.Nome ?? "",
                    SeguradoraNome = p.Seguradora?.NomeFantasia ?? ""
                }).ToList(),
                PesquisaSafra = pesquisaSafra,
                PesquisaCultura = pesquisaCultura,
                PesquisaSeguradora = pesquisaSeguradora,
                PesquisaAtivoInativo = pesquisaAtivoInativo,
                PaginaAtual = page,
                TotalPaginas = totalPages
            };

            var safras = await _safraService.ObterTodosPaginadoAsync(null, null, null, 1, 100);
            ViewData["Safras"] = safras.Select(s => new SelectListItem
            {
                Text = $"{s.Descricao} - {s.AnoReferencia}",
                Value = s.Id.ToString()
            }).ToList();

            var culturas = await _culturaService.ObterTodosPaginadoAsync(null, null, null, null, 1, 100);
            ViewData["Culturas"] = culturas.Select(c => new SelectListItem
            {
                Text = c.Nome,
                Value = c.Id.ToString()
            }).ToList();

            var seguradoras = await _seguradoraService.ObterTodosPaginadoAsync(null, null, 1, 100);
            ViewData["Seguradoras"] = seguradoras.Select(s => new SelectListItem
            {
                Text = s.NomeFantasia,
                Value = s.Id.ToString()
            }).ToList();

            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            var model = new ProdutosViewModel();
            await CarregarDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(ProdutosViewModel model)
        {
            try
            {
                model.Produto.Ativo = true;
                model.Produto.Excluido = false;
                model.Produto.DataCriacao = DateTime.UtcNow;
                var result = await _produtosService.CadastrarAsync(model.Produto);
                return Json(new { success = result != null, message = result != null ? "Produto cadastrado!" : "Erro ao cadastrar produto." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao cadastrar produto: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var produto = await _produtosService.ObterPorIdAsync(id);
            if (produto != null)
            {
                produto.SubvencoesEstaduaisIds = produto.ProdutosSubvencoesEstaduais?
                    .Where(x => x.SubvencaoEstadual != null)
                    .Select(x => x.SubvencaoEstadual!.Id)
                    .ToList() ?? new List<Guid>();
            }
            var model = new ProdutosViewModel
            {
                Produto = produto ?? new MCR.API.Entities.ProdutosEntity()
            };
            await CarregarDropdowns(model);
            return View("Gerenciar", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Gerenciar(ProdutosViewModel model)
        {
            try
            {
                var produto = model.Produto;
                produto.ProdutosSubvencoesEstaduais = produto.SubvencoesEstaduaisIds?
                    .Select(id => new MCR.API.Entities.ProdutoSubvencaoEstadualEntity
                    {
                        ProdutoId = produto.Id,
                        SubvencaoEstadualId = id
                    }).ToList() ?? new List<MCR.API.Entities.ProdutoSubvencaoEstadualEntity>();

                var result = await _produtosService.AtualizarAsync(produto);
                return Json(new { success = result != null, message = result != null ? "Produto atualizado com sucesso!" : "Erro ao atualizar produto." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao atualizar produto: " + ex.Message });
            }
        }

        private async Task CarregarDropdowns(ProdutosViewModel model)
        {
            var safras = await _safraService.ObterTodosPaginadoAsync(null, null, null, 1, 999);
            model.SafraOptions = safras.Select(s => new SelectListItem
            {
                Text = $"{s.Descricao} - {s.AnoReferencia}",
                Value = s.Id.ToString()
            }).ToList();

            var culturas = await _culturaService.ObterTodosPaginadoAsync(null, null, null, null, 1, 999);
            model.CulturaOptions = culturas.Select(c => new SelectListItem
            {
                Text = c.Nome,
                Value = c.Id.ToString()
            }).ToList();

            var seguradoras = await _seguradoraService.ObterTodosPaginadoAsync(null, true, 1, 999);
            model.SeguradoraOptions = seguradoras.Select(s => new SelectListItem
            {
                Text = s.NomeFantasia,
                Value = s.Id.ToString()
            }).ToList();

            var subvFederal = await _context.SubvencoesFederais.Where(x => x.Ativo && !x.Excluido).ToListAsync();
            ViewBag.SubvencaoFederalList = subvFederal.Select(s => new SelectListItem
            {
                Text = $"{s.Cultura} - {s.AnoCivil} ({s.Porentagem}%)",
                Value = s.Id.ToString()
            }).ToList();

            var subvEstadual = await _context.SubvencoesEstaduais.Where(x => x.Ativo && !x.Excluido).ToListAsync();
            ViewBag.SubvencaoEstadualList = subvEstadual.Select(s => new SelectListItem
            {
                Text = $"{s.Cultura} - {s.Estado} - {s.AnoCivil} ({s.Porentagem}%)",
                Value = s.Id.ToString()
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Duplicar(Guid id)
        {
            try
            {
                var result = await _produtosService.DuplicarAsync(id);
                if (result == null)
                    return Json(new { success = false, message = "Não foi possível duplicar o produto." });
                return Json(new { success = true, message = "Produto duplicado.", produtoId = result.Id.ToString() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao duplicar: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAtivo(Guid id)
        {
            try
            {
                var result = await _produtosService.ToggleAtivoAsync(id);
                return Json(new { success = true, message = result ? "Produto ativado." : "Produto inativado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao alterar status: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(Guid id)
        {
            try
            {
                var result = await _produtosService.ExcluirAsync(id);
                if (!result)
                    return Json(new { success = false, message = "Não foi possível excluir o produto." });
                return Json(new { success = true, message = "Produto excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao excluir: " + ex.Message });
            }
        }
    }
}
