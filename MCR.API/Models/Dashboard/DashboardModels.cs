namespace MCR.API.Models.Dashboard
{
    public class DashboardFilterRequest
    {
        public string UserLevel { get; set; } = string.Empty;
        public List<Guid> CorretoraIds { get; set; } = new();
        public List<Guid> CanalIds { get; set; } = new();
        public List<Guid> PaIds { get; set; } = new();
        public List<Guid> CulturaIds { get; set; } = new();
        public List<Guid> SafraIds { get; set; } = new();
        public List<string> StatusInicial { get; set; } = new();
        public List<string> StatusFinal { get; set; } = new();
        public List<Guid> SeguradoraIds { get; set; } = new();
    }

    public class DashboardDataResponse
    {
        public Dictionary<string, StatusData> StatusData { get; set; } = new();
        public SubtotalData Subtotal { get; set; } = new();
        public ChartData Area { get; set; } = new();
        public ChartData Seguradora { get; set; } = new();
        public ChartData Status { get; set; } = new();
    }

    public class StatusData
    {
        public decimal Area { get; set; }
        public int Qtd { get; set; }
        public decimal LMI { get; set; }
        public decimal Premio { get; set; }
        public decimal Produtividade { get; set; }
        public decimal Taxa { get; set; }
    }

    public class SubtotalData
    {
        public decimal Area { get; set; }
        public int Qtd { get; set; }
        public decimal LMI { get; set; }
        public decimal Premio { get; set; }
        public decimal Produtividade { get; set; }
        public decimal Taxa { get; set; }
    }

    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Values { get; set; } = new();
    }

    public class InitialDataResponse
    {
        public List<SelectOption> Corretoras { get; set; } = new();
        public List<SelectOption> Canais { get; set; } = new();
        public List<SelectOption> PontosAtendimento { get; set; } = new();
        public List<SelectOption> Culturas { get; set; } = new();
        public List<SelectOption> Safras { get; set; } = new();
        public List<SelectOption> Seguradoras { get; set; } = new();
    }

    public class SelectOption
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class DashboardSummary
    {
        public decimal TotalArea { get; set; }
        public int TotalPropostas { get; set; }
        public decimal TotalLMI { get; set; }
        public decimal TotalPremio { get; set; }
        public decimal MediaProdutividade { get; set; }
        public decimal MediaTaxa { get; set; }
    }

    public class ResumoExecutivoResponse
    {
        public int TotalClientes { get; set; }
        public int ClientesAtivos { get; set; }
        public int TotalCorretoras { get; set; }
        public int TotalCanais { get; set; }
        public int TotalPontosAtendimento { get; set; }
        public int TotalPropostas { get; set; }
        public int PropostasEmAndamento { get; set; }
        public int CotacoesHoje { get; set; }
        public decimal TotalPremio { get; set; }
        public decimal TotalLMI { get; set; }
        public decimal TotalArea { get; set; }
    }

    public class PerformanceCorretoraResponse
    {
        public string Nome { get; set; } = string.Empty;
        public int TotalPropostas { get; set; }
        public decimal TotalPremio { get; set; }
        public decimal TotalArea { get; set; }
        public decimal TotalLMI { get; set; }
        public decimal TaxaConversao { get; set; }
    }

    public class EvolucaoTemporalResponse
    {
        public List<string> Meses { get; set; } = new();
        public List<decimal> PremioPorMes { get; set; } = new();
        public List<int> PropostasPorMes { get; set; } = new();
        public List<decimal> AreaPorMes { get; set; } = new();
    }
}
