using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    [Authorize]
    public class UsuarioCanalController : Controller
    {
        private readonly DbContextMCR _context;
        private readonly UserManager<UsuarioEntity> _userManager;
        private readonly ICanalService _canalService;
        private readonly IPontoAtendimentoService _pontoAtendimentoService;

        public UsuarioCanalController(
            DbContextMCR context,
            UserManager<UsuarioEntity> userManager,
            ICanalService canalService,
            IPontoAtendimentoService pontoAtendimentoService)
        {
            _context = context;
            _userManager = userManager;
            _canalService = canalService;
            _pontoAtendimentoService = pontoAtendimentoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string pesquisaNomeUsuario = null, int page = 1, int pageSize = 10)
        {
            var query = _context.UsuariosCanal
                .Include(uc => uc.Usuario)
                .Include(uc => uc.UsuarioLiderado)
                .Include(uc => uc.Canal)
                .Where(uc => uc.Ativo && !uc.Excluido)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pesquisaNomeUsuario))
                query = query.Where(uc => uc.Usuario!.Name!.Contains(pesquisaNomeUsuario) ||
                                         uc.UsuarioLiderado!.Name!.Contains(pesquisaNomeUsuario));

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(uc => uc.Usuario!.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.PesquisaNomeUsuario = pesquisaNomeUsuario;
            ViewBag.PaginaAtual = page;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            ViewBag.Usuarios = new SelectList(
                (await _userManager.GetUsersInRoleAsync("Corretor"))
                    .Select(u => new { id = u.Id, text = u.Name }).OrderBy(x => x.text).ToList(),
                "id", "text");
            ViewBag.Canais = new SelectList(
                (await _canalService.ObterTodosPaginadoAsync())
                    .Select(c => new { id = c.Id, text = c.NomeFantasia }).OrderBy(x => x.text).ToList(),
                "id", "text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(UsuarioCanalEntity model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                model.UsuarioId = user!.Id;
                model.Ativo = true;
                model.Excluido = false;
                _context.UsuariosCanal.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao salvar: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var vinculo = await _context.UsuariosCanal
                .Include(uc => uc.Usuario)
                .Include(uc => uc.UsuarioLiderado)
                .Include(uc => uc.Canal)
                .FirstOrDefaultAsync(uc => uc.Id == id);

            if (vinculo == null) return NotFound();

            ViewBag.Usuarios = new SelectList(
                (await _userManager.GetUsersInRoleAsync("Corretor"))
                    .Select(u => new { id = u.Id, text = u.Name }).OrderBy(x => x.text).ToList(),
                "id", "text", vinculo.LideradoId);
            ViewBag.Canais = new SelectList(
                (await _canalService.ObterTodosPaginadoAsync())
                    .Select(c => new { id = c.Id, text = c.NomeFantasia }).OrderBy(x => x.text).ToList(),
                "id", "text", vinculo.CanalId);
            return View(vinculo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(UsuarioCanalEntity model)
        {
            try
            {
                var vinculo = await _context.UsuariosCanal.FindAsync(model.Id);
                if (vinculo == null) return NotFound();

                vinculo.CanalId = model.CanalId;
                vinculo.LideradoId = model.LideradoId;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao atualizar: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(Guid id)
        {
            var vinculo = await _context.UsuariosCanal.FindAsync(id);
            if (vinculo != null)
            {
                vinculo.Excluido = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
