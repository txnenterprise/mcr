using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Usuarios
{
    public class UsuarioAlterarViewComponent : BaseViewComponent
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ICorretoraService _corretoraService;
        private readonly ICanalService _canalService;
        private readonly IPontoAtendimentoService _pontoAtendimentoService;
        private readonly DbContextMCR _context;

        public UsuarioAlterarViewComponent(
            IUsuarioService usuarioService,
            ICorretoraService corretoraService,
            ICanalService canalService,
            IPontoAtendimentoService pontoAtendimentoService,
            DbContextMCR context)
        {
            _usuarioService = usuarioService;
            _corretoraService = corretoraService;
            _canalService = canalService;
            _pontoAtendimentoService = pontoAtendimentoService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid userId)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(userId);
            if (usuario == null)
                return Content("Usuário não encontrado.");

            var dto = new MCR.API.Usuario.Domain.DTO.UsuarioAlterarDTO
            {
                Id = usuario.Id.ToString(),
                Nome = usuario.Name,
                Email = usuario.Email,
                Document = usuario.Document,
                PhoneNumber = usuario.PhoneNumber,
                PerfilAcesso = usuario.Role
            };

            // Carregar EstruturaNegocio existente
            dto.EstruturaNegocio = await _context.UsuarioEstruturaNegocio
                .Where(x => x.UsuarioId == userId && x.Ativo && !x.Excluido)
                .Include(x => x.Corretora)
                .Include(x => x.Canal)
                .Include(x => x.PontoAtendimento)
                .Select(x => new MCR.API.Entities.UsuarioEstruturaNegocioDTO
                {
                    CorretoraId = x.CorretoraId,
                    CanalId = x.CanalId,
                    PontoAtendimentoId = x.PontoAtendimentoId,
                    CorretoraRazao = x.Corretora != null ? x.Corretora.NomeFantasia : "*",
                    CanalRazao = x.Canal != null ? x.Canal.RazaoSocial : "*",
                    PontoAtendimentoRazao = x.PontoAtendimento != null ? x.PontoAtendimento.NomeFantasia : "*"
                }).ToListAsync();

            var corretoras = (await _corretoraService.ObterTodosAsync()).ToList();
            ViewBag.Corretora = new SelectList(
                corretoras.Select(x => new { id = x.Id, text = x.NomeFantasia }).OrderBy(x => x.text).ToList(),
                "id", "text");

            var canais = (await _canalService.ObterTodosPaginadoAsync()).ToList();
            ViewBag.Canal = new SelectList(
                canais.Select(x => new { id = x.Id, text = x.RazaoSocial }).OrderBy(x => x.text).ToList(),
                "id", "text");

            var pontosAtendimento = (await _pontoAtendimentoService.ObterTodosPaginadoAsync()).ToList();
            ViewBag.PontoAtendimento = new SelectList(
                pontosAtendimento.Select(x => new { id = x.Id, text = x.NomeFantasia }).OrderBy(x => x.text).ToList(),
                "id", "text");

            var perfis = new List<string> { "Administrador", "Corretor", "Gestor do Canal", "Consultor", "Assistente" };
            ViewBag.ListaPerfis = new SelectList(perfis.Select(p => new { id = p, text = p }).ToList(), "id", "text");

            return View(dto);
        }
    }
}
