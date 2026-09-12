using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;

namespace MCR.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICorretoraService _corretoraService;
        private readonly ICanalService _canalService;
        private readonly IPontoAtendimentoService _pontoAtendimentoService;

        public HomeController(
            ICorretoraService corretoraService,
            ICanalService canalService,
            IPontoAtendimentoService pontoAtendimentoService)
        {
            _corretoraService = corretoraService;
            _canalService = canalService;
            _pontoAtendimentoService = pontoAtendimentoService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DebugTest()
        {
            var result = new Dictionary<string, object>();
            try
            {
                var corretoras = await _corretoraService.ObterTodosAsync();
                result["corretoras_count"] = corretoras?.Count() ?? 0;
                result["corretoras"] = corretoras?.Select(c => new { c.Id, c.NomeFantasia, c.RazaoSocial }).ToList();
            }
            catch (Exception ex)
            {
                result["corretoras_error"] = ex.Message;
            }
            try
            {
                var canais = await _canalService.ObterTodosPaginadoAsync(null, null, 1, 999);
                result["canais_count"] = canais?.Count() ?? 0;
                result["canais"] = canais?.Select(c => new { c.Id, c.RazaoSocial, c.NomeFantasia }).ToList();
            }
            catch (Exception ex)
            {
                result["canais_error"] = ex.Message;
            }
            try
            {
                var canaisAll = await _canalService.ObterTodosPaginadoAsync(null, null, 1, 99999);
                result["canais_all_count"] = canaisAll?.Count() ?? 0;
            }
            catch (Exception ex)
            {
                result["canais_all_error"] = ex.Message;
            }
            try
            {
                var pas = await _pontoAtendimentoService.ObterTodosPaginadoAsync(null, null, 1, 999);
                result["pa_count"] = pas?.Count() ?? 0;
            }
            catch (Exception ex)
            {
                result["pa_error"] = ex.Message;
            }
            return Json(result);
        }
    }
}
