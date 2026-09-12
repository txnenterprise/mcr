namespace MCR.API.Models
{
    public class CotacaoFilter
    {
        public string Seguradora { get; set; }
        public string NumeroCotacao { get; set; }
        public string CodigoInterno { get; set; }
        public string TipoSeguro { get; set; }
        public string Beneficiado { get; set; }
        public DateTime? DataCotacao { get; set; }
        public string Corretor { get; set; }
        public bool? EstaEfetivado { get; set; }
    }
    public class Pagination
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}