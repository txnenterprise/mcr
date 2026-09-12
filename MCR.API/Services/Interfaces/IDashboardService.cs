using System.Security.Claims;
using MCR.API.Models.Dashboard;

namespace MCR.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<string> GetUserLevelAsync(ClaimsPrincipal user);
        Task<InitialDataResponse> GetInitialDataAsync(ClaimsPrincipal user);
        Task<List<SelectOption>> GetPontosAtendimentoAsync(List<Guid> canalIds, ClaimsPrincipal user);
        Task<DashboardDataResponse> GetDashboardDataAsync(DashboardFilterRequest request, ClaimsPrincipal user);
        Task<DashboardDataResponse> GetSinistroDashboardDataAsync(DashboardFilterRequest request, ClaimsPrincipal user);
        Task<ResumoExecutivoResponse> GetResumoExecutivoAsync(ClaimsPrincipal user);
        Task<List<PerformanceCorretoraResponse>> GetPerformanceCorretoraAsync(DashboardFilterRequest request, ClaimsPrincipal user);
        Task<EvolucaoTemporalResponse> GetEvolucaoTemporalAsync(DashboardFilterRequest request, ClaimsPrincipal user);
        int GetTotalPropostas();
    }
}
