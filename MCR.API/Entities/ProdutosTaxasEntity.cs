using System.ComponentModel.DataAnnotations;

namespace MCR.API.Entities
{
    public class ProdutosTaxasEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public ProdutosEntity Produto { get; set; }
        public string UF { get; set; }
        public string Municipio { get; set; }
        public decimal ProdutividadeEsperada { get; set; }
        public decimal TaxaNc65 { get; set; }
        public decimal? TaxaNc70 { get; set; }
        public decimal? TaxaNc75 { get; set; }
        public string? Cpf { get; set; }
        public bool Ativo { get; set; } = true;
        public bool Excluido { get; set; }
    }
}