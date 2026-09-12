using MCR.API.Entities;

namespace MCR.API.Models
{
    public class MonitoramentoModel
    {
        public PropriedadeEntity Propriedade { get; set; }
        public IList<TalhaoEntity> Talhoes { get; set; } = new List<TalhaoEntity>();
        public MonitoringConfigurationEntity Configuracao { get; set; }
        public IList<MonitoringExecutionEntity> Execucoes { get; set; } = new List<MonitoringExecutionEntity>();
        public IList<MonitoringAlertEntity> Alertas { get; set; } = new List<MonitoringAlertEntity>();
        public IList<SatelliteSceneEntity> CenasSatelite { get; set; } = new List<SatelliteSceneEntity>();
        public IList<MonitoringAnalysisEntity> Analises { get; set; } = new List<MonitoringAnalysisEntity>();
        public MonitoringAnalysisEntity UltimaAnalise { get; set; }
        public int TotalExecucoes { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}
