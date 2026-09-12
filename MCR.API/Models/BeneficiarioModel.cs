using MCR.API.Entities;

namespace MCR.API.Models
{
    public class BeneficiarioModel
    {
        public BeneficiarioEntity Beneficiario { get; set; }
        public IList<BeneficiarioEntity> ListaBeneficiarios { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public string ObjectImagem { get; set; }
        public string PesquisaNome { get; set; }
        public string PesquisaAtivoInativo { get; set; }
        public string PesquisaRazaoSocialPA { get; set; }
        public string PesquisaCNPJ { get; set; }
    }
}