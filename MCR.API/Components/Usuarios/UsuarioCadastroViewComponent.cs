using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MCR.API.Entities;
using MCR.API.Services.Interfaces;

namespace MCR.API.Components.Usuarios
{
    public class UsuarioCadastroViewComponent : BaseViewComponent
    {
        private readonly ICorretoraService _corretoraService;
        private readonly ICanalService _canalService;
        private readonly IPontoAtendimentoService _pontoAtendimentoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioCadastroViewComponent(
            ICorretoraService corretoraService,
            ICanalService canalService,
            IPontoAtendimentoService pontoAtendimentoService,
            IHttpContextAccessor httpContextAccessor)
        {
            _corretoraService = corretoraService;
            _canalService = canalService;
            _pontoAtendimentoService = pontoAtendimentoService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new UsuarioCadastroDTO();

            ViewData["ListaPerfis"] = new List<SelectListItem>
            {
                new() { Text = "Administrador", Value = "Administrador" },
                new() { Text = "Corretor", Value = "Corretor" },
                new() { Text = "Gestor do Canal", Value = "Gestor do Canal" },
                new() { Text = "Consultor", Value = "Consultor" },
                new() { Text = "Assistente", Value = "Assistente" }
            };

            var corretoraLista = (await _corretoraService.ObterTodosAsync()).ToList();
            if (corretoraLista.Count == 1)
                model.DefaultCorretoraId = corretoraLista.First().Id;

            ViewBag.Corretora = new SelectList(corretoraLista.Select(x => new { id = x.Id, text = x.NomeFantasia }).OrderBy(x => x.text).ToList(), "id", "text");

            var canaisLista = new List<CanalEntity>();
            if (model.DefaultCorretoraId.HasValue)
            {
                canaisLista = (await _canalService.ObterPorCorretoraAsync(model.DefaultCorretoraId.Value)).ToList();
                if (canaisLista.Count == 1)
                    model.DefaultCanalId = canaisLista.First().Id;
            }
            ViewBag.Canal = new SelectList(canaisLista.Select(x => new { id = x.Id, text = x.RazaoSocial ?? x.NomeFantasia }).OrderBy(x => x.text).ToList(), "id", "text");

            var pontosAtendimentoLista = new List<PontoAtendimentoEntity>();
            if (model.DefaultCanalId.HasValue)
            {
                pontosAtendimentoLista = (await _pontoAtendimentoService.ObterPorCanalAsync(model.DefaultCanalId.Value)).ToList();
                if (pontosAtendimentoLista.Count == 1)
                    model.DefaultPontoAtendimentoId = pontosAtendimentoLista.First().Id;
            }
            ViewBag.PontoAtendimento = new SelectList(pontosAtendimentoLista.Select(x => new { id = x.Id, text = x.RazaoSocial }).OrderBy(x => x.text).ToList(), "id", "text");

            ViewBag.CorretoraCount = corretoraLista.Count;
            ViewBag.CanalCount = canaisLista.Count;
            ViewBag.PACount = pontosAtendimentoLista.Count;

            Console.WriteLine($"[DEBUG UsuarioCadastro] Corretoras: {corretoraLista.Count}, Canais: {canaisLista.Count}, PAs: {pontosAtendimentoLista.Count}");
            if (corretoraLista.Count > 0)
                Console.WriteLine($"[DEBUG UsuarioCadastro] Primeira corretora: {corretoraLista.First().Id} - {corretoraLista.First().NomeFantasia}");
            if (canaisLista.Count > 0)
                Console.WriteLine($"[DEBUG UsuarioCadastro] Primeiro canal: {canaisLista.First().Id} - {canaisLista.First().RazaoSocial}");

            return View(model);
        }
    }
}
