namespace MCR.API.Models.ViewModels
{
    public class ParametrizacaoModel
    {
        public ParametrizacaoSeguradoraWrapper Seguradora { get; set; } = new();
        public ParametrizacaoRiscoWrapper Risco { get; set; } = new();
        public ParametrizacaoBeneficiarioWrapper Beneficiario { get; set; } = new();
        public ParametrizacaoCustomizacaoRelatorioWrapper CustomizacaoRelatorio { get; set; } = new();
    }

    public class ParametrizacaoSeguradoraWrapper
    {
        public ParametrizacaoSeguradoraItem Seguradora { get; set; } = new();
    }

    public class ParametrizacaoSeguradoraItem
    {
        public string Id { get; set; }
        public bool Sucesso { get; set; }
    }

    public class ParametrizacaoRiscoWrapper
    {
        public ParametrizacaoRiscoItem Risco { get; set; } = new();
    }

    public class ParametrizacaoRiscoItem
    {
        public string Id { get; set; }
        public bool Sucesso { get; set; }
    }

    public class ParametrizacaoBeneficiarioWrapper
    {
        public ParametrizacaoBeneficiarioItem Beneficiario { get; set; } = new();
    }

    public class ParametrizacaoBeneficiarioItem
    {
        public string Id { get; set; }
        public bool Sucesso { get; set; }
    }

    public class ParametrizacaoCustomizacaoRelatorioWrapper
    {
        public ParametrizacaoCustomizacaoRelatorioItem ParametrizacaoCustomizacaoRelatorio { get; set; } = new();
    }

    public class ParametrizacaoCustomizacaoRelatorioItem
    {
        public string Id { get; set; }
        public bool Sucesso { get; set; }
    }
}
