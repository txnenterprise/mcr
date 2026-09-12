namespace MCR.API.Models
{
    public class CotacaoCoberturaModel
    {
        public bool AplicarCoberturaTotal { get; set; }
        public bool ContratarResponsabilidadeCivilMaquinariaAgricola { get; set; }
        public decimal ValorResponsabilidadeCivilMaquinariaAgricola { get; set; }
        public bool ContratarResponsabilidadeCivilEmpregador { get; set; }
        public decimal ValorResponsabilidadeCivilEmpregador { get; set; }
        public bool ContratarCoberturaRelativaPerdaPagamentoAluguel { get; set; }
        public decimal ValorCoberturaRelativaPerdaPagamentoAluguel { get; set; }
        public bool ContratarFurtoSimples { get; set; }
        public decimal ValorFurtoSimples { get; set; }
        public bool ContratarDanosEletricos { get; set; }
        public decimal ValorDanosEletricos { get; set; }
        public bool ContratarQuebraVidros { get; set; }
        public decimal ValorQuebraVidros { get; set; }
    }
}