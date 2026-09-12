using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Models.Dashboard;
using MCR.API.Services.Interfaces;
using DashboardModels = MCR.API.Models.Dashboard;

namespace MCR.API.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userLevel = await _dashboardService.GetUserLevelAsync(User);
                var initialData = await _dashboardService.GetInitialDataAsync(User);

                var request = new DashboardFilterRequest { UserLevel = userLevel };
                var dashboardData = await _dashboardService.GetDashboardDataAsync(request, User);

                var totalPropostas = dashboardData.StatusData?.Values?.Sum(s => s.Qtd) ?? 0;
                var totalArea = dashboardData.StatusData?.Values?.Sum(s => s.Area) ?? 0;
                var totalPremio = dashboardData.StatusData?.Values?.Sum(s => s.Premio) ?? 0;
                var totalLMI = dashboardData.StatusData?.Values?.Sum(s => s.LMI) ?? 0;

                ViewBag.InitialData = initialData;
                ViewBag.DashboardData = dashboardData;
                ViewBag.UserLevel = userLevel;
                ViewBag.TotalPropostas = totalPropostas;
                ViewBag.TotalArea = totalArea;
                ViewBag.TotalPremio = totalPremio;
                ViewBag.TotalLMI = totalLMI;
                ViewBag.Subtotal = dashboardData.Subtotal;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Erro ao carregar dados do dashboard";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDashboardTable([FromBody] DashboardFilterRequest request)
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync(request, User);
                return PartialView("_DashboardTable", dashboardData);
            }
            catch
            {
                return PartialView("_DashboardTable", new DashboardDataResponse());
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDashboardSubtotal([FromBody] DashboardFilterRequest request)
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync(request, User);
                return PartialView("_DashboardSubtotal", dashboardData.Subtotal);
            }
            catch
            {
                return PartialView("_DashboardSubtotal", new DashboardModels.SubtotalData());
            }
        }
    }
}
