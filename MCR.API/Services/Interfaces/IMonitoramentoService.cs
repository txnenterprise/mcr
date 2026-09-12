using MCR.API.Entities;

namespace MCR.API.Services.Interfaces
{
    public interface IMonitoramentoService
    {
        Task<MonitoringConfigurationEntity> ObterConfiguracaoAsync(Guid propertyId);
        Task<MonitoringConfigurationEntity> AtivarMonitoramentoAsync(Guid propertyId);
        Task<MonitoringConfigurationEntity> DesativarMonitoramentoAsync(Guid propertyId);
        Task<IEnumerable<MonitoringExecutionEntity>> ObterExecucoesAsync(Guid propertyId, int page = 1, int pageSize = 10);
        Task<MonitoringExecutionEntity> ExecutarAnaliseAsync(Guid propertyId);
        Task<IEnumerable<MonitoringAlertEntity>> ObterAlertasAsync(Guid propertyId, bool? apenasNaoVisualizados = null);
        Task<MonitoringAlertEntity> MarcarAlertaVisualizadoAsync(Guid alertId);
        Task<List<SatelliteSceneEntity>> BuscarCenasSateliteAsync(
            Guid propertyId, Guid talhaoId, DateTime dataInicio, DateTime dataFim, decimal? nuvemMaxima = null);
        Task<List<SatelliteSceneEntity>> ObterCenasSalvasAsync(Guid propertyId, Guid? talhaoId = null);
    }
}
