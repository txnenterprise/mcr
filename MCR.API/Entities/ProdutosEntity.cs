using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace MCR.API.Entities
{
    public class ProdutosEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid SafraId { get; set; }
        public SafraEntity Safra { get; set; }
        public Guid CulturaId { get; set; }
        public CulturaEntity Cultura { get; set; }
        public Guid SeguradoraId { get; set; }
        public SeguradoraEntity Seguradora { get; set; }
        [Display(Name = "Nome do Produto")]
        public string NomeProduto { get; set; }
        [Display(Name = "Porcentagem de Comissão")]
        public decimal PorcentagemComissao { get; set; }
        [Display(Name = "Porcentagem de Repasse")]
        public decimal PorcentagemRepasse { get; set; }
        [Display(Name = "Processo SUSEP")]
        public string ProcessoSusep { get; set; }
        public string Modalidade { get; set; }
        [Display(Name = "Valor Saca Mínimo")]
        public decimal ValorSacaMinimo { get; set; }
        [Display(Name = "Valor Saca Máximo")]
        public decimal ValorSacaMaximo { get; set; }
        [Display(Name = "Valor Saca Sugerido")]
        public decimal ValorSacaSugerido { get; set; }
        [Display(Name = "Valor Custeio Mínimo")]
        public decimal ValorCusteioMinimo { get; set; }
        [Display(Name = "Valor Custeio Máximo")]
        public decimal ValorCusteioMaximo { get; set; }
        [Display(Name = "Valor Custeio Sugerido")]
        public decimal ValorCusteioSugerido { get; set; }
        [Display(Name = "Área Mínima por Item")]
        public decimal AreaMinimaItem { get; set; }
        [Display(Name = "Área Mínima Total")]
        public decimal AreaMinimaTotal { get; set; }
        [Display(Name = "Prêmio Mínimo")]
        public decimal PremioMinimo { get; set; }
        [Display(Name = "Forma de Pagamento")]
        public string FormaDePagamento { get; set; }
        public string Parcelamento { get; set; }
        public string[] TipoSolo { get; set; }
        [Display(Name = "Ajuste de Produtividade")]
        public decimal? AjusteProdutividade { get; set; }
        [Display(Name = "Ajuste de Taxa")]
        public decimal? AjusteTaxa { get; set; }
        public string[] ClassificacaoSolosAceitos { get; set; }
        public string Replantio { get; set; }
        [Display(Name = "Porcentagem Cobertura Produção")]
        public decimal? PorcentagemCoberturaProducao { get; set; }
        [Display(Name = "Valor Cobertura Adicional")]
        public decimal? ValorCoberturaAdicional { get; set; }
        [Display(Name = "Taxa Cobertura Adicional")]
        public decimal? TaxaCoberturaAdicional { get; set; }
        [Display(Name = "Regulação de Sinistro")]
        public string RegulacaoSinistro { get; set; }
        public bool UtilizaSubvencaoFederal { get; set; }
        [Display(Name = "Subvenção Federal")]
        public Guid? SubvencaoFederalId { get; set; }
        public SubvencaoFederalEntity? SubvencaoFederal { get; set; }
        public bool UtilizaSubvencaoEstadual { get; set; }
        [Display(Name = "Subvenção Estadual")]
        public Guid? SubvencaoEstadualId { get; set; }
        public SubvencaoEstadualEntity? SubvencaoEstadual { get; set; }
        public virtual ICollection<ProdutoSubvencaoEstadualEntity> ProdutosSubvencoesEstaduais { get; set; } = new List<ProdutoSubvencaoEstadualEntity>();

        [NotMapped]
        [Display(Name = "Subvenções Estaduais")]
        public List<Guid> SubvencoesEstaduaisIds { get; set; } = new();
        [Display(Name = "Termo de Ciência")]
        public string? TermoDeCiencia { get; set; }
        [Display(Name = "Capacidade Disponível")]
        public decimal? CapacidadeDisponivel { get; set; }
        [Display(Name = "Kilo por Saca")]
        public decimal? KiloPorSaca { get; set; }


        public bool AceitaPlantioConsorciado { get; set; }
        [Display(Name = "Ajuste Produtividade (Consorciado)")]
        public decimal PlantioConsorciadoAjusteProdutividade { get; set; }
        [Display(Name = "Ajuste Taxa (Consorciado)")]
        public decimal PlantioConsorciadoAjusteTaxa { get; set; }
        public bool AceitaPlantioConvencional { get; set; }
        [Display(Name = "Ajuste Produtividade (Convencional)")]
        public decimal PlantioConvencionalAjusteProdutividade { get; set; }
        [Display(Name = "Ajuste Taxa (Convencional)")]
        public decimal PlantioConvencionalAjusteTaxa { get; set; }
        public bool LavouraIrrigada { get; set; }
        [Display(Name = "Ajuste Produtividade (Irrigada)")]
        public decimal LavouraIrrigadaAjusteProdutividade { get; set; }
        [Display(Name = "Ajuste Taxa (Irrigada)")]
        public decimal LavouraIrrigadaAjusteTaxa { get; set; }
        public bool AceitaPlantioPosCanaDeAcucar { get; set; }
        [Display(Name = "Ajuste Produtividade (Pós-Cana)")]
        public decimal PlantioPosCanaAjusteProdutividade { get; set; }
        [Display(Name = "Ajuste Taxa (Pós-Cana)")]
        public decimal PlantioPosCanaAjusteTaxa { get; set; }

        public bool Ativo { get; set; }
        public bool Excluido { get; set; }

        public DateTime? DataCriacao { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ProdutosTaxasEntity> Taxas { get; set; } = new List<ProdutosTaxasEntity>();
        public virtual ICollection<ProdutosCanalPontoAtendimentoEntity> ProdutosCanalPontoAtendimento { get; set; } = new List<ProdutosCanalPontoAtendimentoEntity>();

        public virtual ICollection<CotacoesAgricolaPropostaEntity> CotacoesAgricolaProposta { get; set; } = new List<CotacoesAgricolaPropostaEntity>();

        public virtual ICollection<PropostasProdutosEntity> PropostasProdutos { get; set; } = new List<PropostasProdutosEntity>();
        public virtual ICollection<PropostasFormaPagamentosEntity> PropostasFormaPagamentos { get; set; } = new List<PropostasFormaPagamentosEntity>();
    }
}