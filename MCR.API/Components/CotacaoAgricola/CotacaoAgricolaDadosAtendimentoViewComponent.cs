using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCR.API.CotacoesAgricola.Domain.DTO;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Components.CotacaoAgricola
{
    public class CotacaoAgricolaDadosAtendimentoViewComponent : BaseViewComponent
    {
        private readonly ICorretoraService _corretoraService;
        private readonly ICanalService _canalService;
        private readonly IPontoAtendimentoService _pontoAtendimentoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DbContextMCR _context;

        public CotacaoAgricolaDadosAtendimentoViewComponent(
            ICorretoraService corretoraService,
            ICanalService canalService,
            IPontoAtendimentoService pontoAtendimentoService,
            IUsuarioService usuarioService,
            IHttpContextAccessor httpContextAccessor,
            DbContextMCR context)
        {
            _corretoraService = corretoraService;
            _canalService = canalService;
            _pontoAtendimentoService = pontoAtendimentoService;
            _usuarioService = usuarioService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(CotacoesAgricolaCadastrarDTO model)
        {
            if (model == null || !model.UsuarioId.HasValue)
            {
                var userIdClaim = UserClaimsPrincipal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var usuario = await _usuarioService.ObterPorIdAsync(userId);
                    if (usuario != null)
                    {
                        model.UsuarioNome = usuario.Name;
                        model.UsuarioEmail = usuario.Email;
                        model.UsuarioTelefone = usuario.PhoneNumber;
                    }
                }
            }

            var corretoras = (await _corretoraService.ObterTodosAsync()).ToList();
            var todosCanais = await _context.Canais.Where(c => c.Ativo && !c.Excluido).ToListAsync();
            var todosPontos = await _context.PontosAtendimento.Where(p => p.Ativo && !p.Excluido).ToListAsync();

            var isCorretor = _httpContextAccessor.IsCorretor();
            var isConsultor = _httpContextAccessor.IsConsultor();

            if (isCorretor || isConsultor)
            {
                var userIdClaim = UserClaimsPrincipal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var estruturas = await _context.UsuarioEstruturaNegocio
                        .Where(x => x.UsuarioId == userId && x.Ativo && !x.Excluido)
                        .ToListAsync();

                    var corretoraIds = estruturas.Where(x => x.CorretoraId.HasValue).Select(x => x.CorretoraId!.Value).Distinct().ToList();
                    var canalIds = estruturas.Where(x => x.CanalId.HasValue).Select(x => x.CanalId!.Value).Distinct().ToList();
                    var paIds = estruturas.Where(x => x.PontoAtendimentoId.HasValue).Select(x => x.PontoAtendimentoId!.Value).Distinct().ToList();

                    corretoras = corretoras.Where(x => corretoraIds.Contains(x.Id)).ToList();
                    todosCanais = todosCanais.Where(x => canalIds.Contains(x.Id)).ToList();
                    todosPontos = todosPontos.Where(x => paIds.Contains(x.Id)).ToList();
                }
            }

            // Auto-preencher ANTES de criar os SelectLists
            var isAdmin = _httpContextAccessor.IsAdministrador();
            if (!isAdmin)
            {
                if (!model.CorretoraId.HasValue && corretoras.Any())
                    model.CorretoraId = corretoras.First().Id;
                if (!model.CanalId.HasValue && todosCanais.Any())
                    model.CanalId = todosCanais.First().Id;
                if (!model.PontoAtendimentoId.HasValue && todosPontos.Any())
                    model.PontoAtendimentoId = todosPontos.First().Id;
            }

            // Agora sim, criar os SelectLists com o selected value correto
            ViewBag.Corretora = new SelectList(corretoras.Select(x => new { id = x.Id, text = x.NomeFantasia }).OrderBy(x => x.text).ToList(), "id", "text", model.CorretoraId);
            ViewBag.CorretoraCount = corretoras.Count();

            ViewBag.Canal = new SelectList(todosCanais.Select(x => new { id = x.Id, text = x.RazaoSocial }).OrderBy(x => x.text).ToList(), "id", "text", model.CanalId);
            ViewBag.CanalCount = todosCanais.Count();

            ViewBag.PontoAtendimento = new SelectList(todosPontos.Select(x => new { id = x.Id, text = x.RazaoSocial }).OrderBy(x => x.text).ToList(), "id", "text", model.PontoAtendimentoId);
            ViewBag.PACount = todosPontos.Count();

            // Passar role do usuário para a view decidir disabled
            ViewBag.UserRole = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return View(model);
        }
    }
}
