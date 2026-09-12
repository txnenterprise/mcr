using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Entities;
using MCR.API.Models;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MCR.API.Controllers
{
    [Authorize]
    public class MonitoramentoController : Controller
    {
        private readonly IMonitoramentoService _monitoramentoService;
        private readonly INdviProcessingService _ndviProcessingService;
        private readonly IPropriedadeService _propriedadeService;
        private readonly DbContextMCR _context;

        public MonitoramentoController(
            IMonitoramentoService monitoramentoService,
            INdviProcessingService ndviProcessingService,
            IPropriedadeService propriedadeService,
            DbContextMCR context)
        {
            _monitoramentoService = monitoramentoService;
            _ndviProcessingService = ndviProcessingService;
            _propriedadeService = propriedadeService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ObterMonitoramento(Guid propertyId)
        {
            var propriedade = await _propriedadeService.ObterPorIdAsync(propertyId);
            if (propriedade == null || !propriedade.Sucesso)
                return NotFound(new { success = false, message = "Propriedade não encontrada." });

            var config = await _monitoramentoService.ObterConfiguracaoAsync(propertyId);
            var execucoes = (await _monitoramentoService.ObterExecucoesAsync(propertyId)).ToList();
            var alertas = (await _monitoramentoService.ObterAlertasAsync(propertyId)).ToList();
            var talhoes = await _context.Talhoes
                .Where(t => t.PropriedadeId == propertyId && !t.Excluido)
                .ToListAsync();
            var cenas = (await _monitoramentoService.ObterCenasSalvasAsync(propertyId)).ToList();
            var analises = (await _ndviProcessingService.ObterAnalisesAsync(propertyId)).ToList();

            var model = new MonitoramentoModel
            {
                Propriedade = propriedade,
                Talhoes = talhoes,
                Configuracao = config,
                Execucoes = execucoes,
                Alertas = alertas,
                CenasSatelite = cenas,
                Analises = analises,
                UltimaAnalise = analises.FirstOrDefault(),
                Sucesso = true
            };

            return PartialView("~/Views/Propriedade/_Monitoramento.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtivarMonitoramento(Guid propertyId)
        {
            var result = await _monitoramentoService.AtivarMonitoramentoAsync(propertyId);
            return Json(new { success = result.Sucesso, message = result.Mensagem });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesativarMonitoramento(Guid propertyId)
        {
            var result = await _monitoramentoService.DesativarMonitoramentoAsync(propertyId);
            return Json(new { success = result.Sucesso, message = result.Mensagem });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecutarAnalise(Guid propertyId)
        {
            var result = await _monitoramentoService.ExecutarAnaliseAsync(propertyId);
            return Json(new
            {
                success = result.Sucesso,
                message = result.Mensagem,
                data = new
                {
                    status = result.Status,
                    satellite = result.Satellite,
                    cloudCover = result.CloudCover,
                    vegetationIndex = result.AverageVegetationIndex,
                    affectedArea = result.AffectedArea,
                    executionDate = result.ExecutionDate.ToString("dd/MM/yyyy HH:mm")
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarAlertaVisualizado(Guid alertId)
        {
            var result = await _monitoramentoService.MarcarAlertaVisualizadoAsync(alertId);
            return Json(new { success = result.Sucesso, message = result.Mensagem });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarCenasSatelite(Guid propertyId, Guid talhaoId,
            DateTime dataInicio, DateTime dataFim, decimal? nuvemMaxima)
        {
            var propriedade = await _propriedadeService.ObterPorIdAsync(propertyId);
            if (propriedade == null || !propriedade.Sucesso)
                return Json(new { success = false, message = "Propriedade não encontrada." });

            var talhao = await _context.Talhoes.FindAsync(talhaoId);
            if (talhao == null || talhao.PropriedadeId != propertyId)
                return Json(new { success = false, message = "Talhão não encontrado para esta propriedade." });

            if (string.IsNullOrEmpty(talhao.KmlTalhao))
                return Json(new { success = false, message = "Este talhão não possui polígono cadastrado. Cadastre o polígono antes de buscar imagens." });

            var cenas = await _monitoramentoService.BuscarCenasSateliteAsync(
                propertyId, talhaoId, dataInicio, dataFim, nuvemMaxima);

            return Json(new
            {
                success = true,
                message = $"{cenas.Count} nova(s) cena(s) encontrada(s) e salva(s).",
                data = cenas.Select(c => new
                {
                    sceneId = c.SceneId,
                    acquisitionDate = c.AcquisitionDate.ToString("dd/MM/yyyy"),
                    cloudCover = c.CloudCover,
                    assetUrl = c.AssetUrl
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObterCenas(Guid propertyId, Guid? talhaoId = null)
        {
            var cenas = await _monitoramentoService.ObterCenasSalvasAsync(propertyId, talhaoId);
            return Json(new
            {
                success = true,
                data = cenas.Select(c => new
                {
                    id = c.Id,
                    sceneId = c.SceneId,
                    collection = c.Collection,
                    acquisitionDate = c.AcquisitionDate.ToString("dd/MM/yyyy"),
                    cloudCover = c.CloudCover,
                    assetUrl = c.AssetUrl,
                    talhaoId = c.TalhaoId
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessarCena(Guid sceneId, Guid talhaoId)
        {
            var result = await _ndviProcessingService.ProcessSceneAsync(sceneId, talhaoId, forceReprocess: true);

            if (!result.Success)
                return Json(new { success = false, message = result.ErrorMessage });

            var a = result.Analysis;
            return Json(new
            {
                success = true,
                message = "Análise NDVI processada com sucesso.",
                data = new
                {
                    id = a.Id,
                    averageNdvi = a.AverageNdvi,
                    minimumNdvi = a.MinimumNdvi,
                    maximumNdvi = a.MaximumNdvi,
                    healthyAreaPercent = a.HealthyAreaPercent,
                    criticalAreaPercent = a.CriticalAreaPercent,
                    classification = a.Classification,
                    validPixelCount = a.ValidPixelCount,
                    acquisitionDate = a.AcquisitionDate.ToString("dd/MM/yyyy"),
                    previewImageBase64 = result.PreviewImageBase64
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObterAnalise(Guid analysisId)
        {
            var analysis = await _context.MonitoringAnalyses.FindAsync(analysisId);
            if (analysis == null)
                return Json(new { success = false, message = "Análise não encontrada." });

            return Json(new
            {
                success = true,
                data = new
                {
                    id = analysis.Id,
                    previewImageBase64 = analysis.PreviewImageBase64,
                    classification = analysis.Classification,
                    averageNdvi = analysis.AverageNdvi
                }
            });
        }
    }
}
