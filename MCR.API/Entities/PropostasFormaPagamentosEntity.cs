using System.ComponentModel.DataAnnotations;


namespace MCR.API.Entities;

public class PropostasFormaPagamentosEntity
{

    [Key]
    public Guid Id { get; set; }

    public Guid PropostaId { get; set; }
    public Guid ProdutoId { get; set; }
    public string? FormaDePagamento { get; set; }
    public string? Parcelamento { get; set; }

    public virtual PropostasEntity? Proposta { get; set; }
    public virtual ProdutosEntity? Produto { get; set; }
}