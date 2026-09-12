namespace MCR.API.Models
{
    public class ParametrizacaoModel
    {
        public ParametrizacaoSeguradoraModel Seguradora { get; set; }
        public ParametrizacaoBeneficiarioModel Beneficiario { get; set; }
        public ParametrizacaoRiscoModel Risco { get; set; }
        public ParametrizacaoCustomizacaoRelatorioModel CustomizacaoRelatorio { get; set; }
    }
}