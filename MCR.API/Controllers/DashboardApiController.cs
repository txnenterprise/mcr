using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MCR.API.Services.Interfaces;
using MCR.API.Models.Dashboard;

namespace MCR.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardApiController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardApiController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("user-level")]
        public async Task<IActionResult> GetUserLevel()
        {
            try
            {
                var userLevel = await _dashboardService.GetUserLevelAsync(User);
                return Ok(new { userLevel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("initial-data")]
        public async Task<IActionResult> GetInitialData()
        {
            try
            {
                var data = await _dashboardService.GetInitialDataAsync(User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("pontos-atendimento")]
        public async Task<IActionResult> GetPontosAtendimento([FromBody] CanalFilterRequest request)
        {
            try
            {
                var data = await _dashboardService.GetPontosAtendimentoAsync(request.CanalIds, User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("data")]
        public async Task<IActionResult> GetDashboardData([FromBody] DashboardFilterRequest request)
        {
            try
            {
                var data = await _dashboardService.GetDashboardDataAsync(request, User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("sinistro-data")]
        public async Task<IActionResult> GetSinistroDashboardData([FromBody] DashboardFilterRequest request)
        {
            try
            {
                var data = await _dashboardService.GetSinistroDashboardDataAsync(request, User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("resumo-executivo")]
        public async Task<IActionResult> GetResumoExecutivo()
        {
            try
            {
                var data = await _dashboardService.GetResumoExecutivoAsync(User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("performance-corretora")]
        public async Task<IActionResult> GetPerformanceCorretora([FromBody] DashboardFilterRequest request)
        {
            try
            {
                var data = await _dashboardService.GetPerformanceCorretoraAsync(request, User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("evolucao-temporal")]
        public async Task<IActionResult> GetEvolucaoTemporal([FromBody] DashboardFilterRequest request)
        {
            try
            {
                var data = await _dashboardService.GetEvolucaoTemporalAsync(request, User);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class CanalFilterRequest
    {
        public List<Guid> CanalIds { get; set; } = new();
    }
}
