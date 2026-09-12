namespace MCR.API.Models
{
    public class CotacaoInformacaoBemModel
    {
        public string TipoEquipamento { get; set; }
        public int AnoFabricacao { get; set; }
        public decimal ValorEquipamento { get; set; }
        public string MarcaEquipamento { get; set; }
        public string ModeloEquipamento { get; set; }
        public string NumeroSerieEquipamento { get; set; }
        public string NumeroChassiEquipamento { get; set; }
        public bool InformarNotaFiscal { get; set; }
        public DateTime DataNotaFiscal { get; set; }
        public string NumeroNotaFiscal { get; set; }
    }
}