namespace MCR.API.Models
{
    public class CotacaoInformacaoSeguroModel
    {
        public string TempoVigenciaSeguro { get; set; }
        public string TipoSeguro { get; set; }
        public string ApoliceRenovacao { get; set; }
        public string SeguradoraAnterior { get; set; }
        public string BemFinanciado { get; set; }
        public string BancoBeneficiarioInformacaoSeguro { get; set; }
        public string FormaPagamentoSeguro { get; set; }
        public int QuantidadeParcelas { get; set; }
        public string PrazoSeguro { get; set; }
    }
}