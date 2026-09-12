namespace MCR.API.Services.Implementations
{
    public static class NdviClassification
    {
        public const decimal ExcelenteMin = 0.75m;
        public const decimal BomMin = 0.60m;
        public const decimal AtencaoMin = 0.40m;

        public const string LabelExcelente = "Excelente";
        public const string LabelBom = "Bom";
        public const string LabelAtencao = "Atenção";
        public const string LabelCritico = "Crítico";

        public const string CorExcelente = "#22c55e";
        public const string CorBom = "#84cc16";
        public const string CorAtencao = "#eab308";
        public const string CorCritico = "#ef4444";

        public static string Classificar(decimal ndvi)
        {
            if (ndvi >= ExcelenteMin) return LabelExcelente;
            if (ndvi >= BomMin) return LabelBom;
            if (ndvi >= AtencaoMin) return LabelAtencao;
            return LabelCritico;
        }

        public static string ObterCor(decimal ndvi)
        {
            if (ndvi >= ExcelenteMin) return CorExcelente;
            if (ndvi >= BomMin) return CorBom;
            if (ndvi >= AtencaoMin) return CorAtencao;
            return CorCritico;
        }
    }
}
