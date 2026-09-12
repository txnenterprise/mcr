using System.ComponentModel.DataAnnotations;

namespace MCR.API.Produto.Domain.DTO
{
    public class ProdutosTaxasDTO
    {
        public Guid ProdutoTaxaId { get; set; }
        public Guid ProdutoId { get; set; }

        [Required(ErrorMessage = "UF é obrigatória")]
        public string? UF { get; set; }

        [Required(ErrorMessage = "Município é obrigatório")]
        public string? Municipio { get; set; }

        public decimal? ProdutividadeEsperada { get; set; }
        public decimal? TaxaNc65 { get; set; }
        public decimal? TaxaNc70 { get; set; }
        public decimal? TaxaNc75 { get; set; }
        public string? Cpf { get; set; }
        public bool Ativo { get; set; } = true;
        public bool Excluido { get; set; }

        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class ProdutosCanalListaDTO
    {
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string? Canal { get; set; }
        public string? PontoAtendimento { get; set; }
    }

    public class ProdutosCanalRegisterDTO
    {
        public Guid? ProdutoCanalId { get; set; }
        public Guid ProdutoId { get; set; }

        [Required(ErrorMessage = "Canal é obrigatório")]
        public Guid? CanalId { get; set; }

        public List<CanalDTO>? Canais { get; set; }
        public List<PontoAtendimentoDTO>? PontoAtendimento { get; set; }
        public List<Guid> PontoAtendimentoSelected { get; set; } = new();
    }

    public class CanalDTO
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
    }

    public class PontoAtendimentoDTO
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
    }
}
