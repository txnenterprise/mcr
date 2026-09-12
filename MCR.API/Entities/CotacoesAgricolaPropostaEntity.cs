using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace MCR.API.Entities;

public class CotacoesAgricolaPropostaEntity
{
    [Key]
    public Guid Id { get; set; }
    [ForeignKey(nameof(CotacoesAgricola))]
    public Guid CotacaoAgricolaId { get; set; }
    public int Opcao { get; set; }

    public string TipoOferta { get; set; } = string.Empty;

    // Referências ao produto/seguradora
    public Guid SeguradoraId { get; set; }
    public SeguradoraEntity Seguradora { get; set; }
    public Guid ProdutoId { get; set; }
    public ProdutosEntity Produto { get; set; }

    // Dados de Cobertura
    public string RegulacaoSinistro { get; set; } = string.Empty;
    public decimal ProdutividadeEsperada { get; set; }
    public decimal NivelCobertura { get; set; }
    public decimal ProdutividadeSegurada { get; set; }
    public decimal LMIProducaoHectare { get; set; }
    public decimal LMIReplantioHectare { get; set; }
    public decimal LMIProducaoTotal { get; set; }
    public decimal LMIReplantioTotal { get; set; }

    // Dados de Pr�mio
    public decimal PremioTotal { get; set; }
    public decimal SubvencaoFederal { get; set; }
    public decimal SubvencaoEstadual { get; set; }
    public decimal ParcelaSegurado { get; set; }
    public decimal CustoHectare { get; set; }
    public decimal CustoScHectare { get; set; }
    public decimal CustoScTotal { get; set; }

    // Campos padr�o
    public bool Ativo { get; set; }
    public bool Excluido { get; set; }

    [NotMapped]
    public bool Sucesso { get; set; }
    [NotMapped]
    public string Mensagem { get; set; }
    [NotMapped]
    public int TotalItems { get; set; }
    [NotMapped]
    public int TotalPages { get; set; }

    public virtual CotacoesAgricolaEntity? CotacoesAgricola { get; set; }

    public virtual ICollection<PropostasProdutosEntity> PropostasProdutos { get; set; } = new List<PropostasProdutosEntity>();
}
