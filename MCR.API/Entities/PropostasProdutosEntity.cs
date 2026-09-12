using System.ComponentModel.DataAnnotations;




namespace MCR.API.Entities;

public class PropostasProdutosEntity
{

    [Key]
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public Guid CotacoesAgricolaPropostaId { get; set; }
    public int Opcao { get; set; }

    // Referências ao produto/seguradora
    public Guid SeguradoraId { get; set; }
    public Guid ProdutoId { get; set; }

    // Dados de Cobertura
    public string RegulacaoSinistro { get; set; } = string.Empty;
    public decimal ProdutividadeEsperada { get; set; }
    public decimal NivelCobertura { get; set; }
    public decimal ProdutividadeSegurada { get; set; }
    public decimal LMIProducaoHectare { get; set; }
    public decimal LMIReplantioHectare { get; set; }
    public decimal LMIProducaoTotal { get; set; }
    public decimal LMIReplantioTotal { get; set; }

    // Dados de Prêmio
    public decimal PremioTotal { get; set; }
    public decimal SubvencaoFederal { get; set; }
    public decimal SubvencaoEstadual { get; set; }
    public decimal ParcelaSegurado { get; set; }
    public decimal CustoHectare { get; set; }
    public decimal CustoScHectare { get; set; }
    public decimal CustoScTotal { get; set; }

    public SeguradoraEntity? Seguradora { get; set; }
    public ProdutosEntity? Produto { get; set; }
    public virtual PropostasEntity? Proposta { get; set; }
    public virtual CotacoesAgricolaPropostaEntity? CotacoesAgricolaProposta { get; set; }
}
