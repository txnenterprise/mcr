using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MCR.API.Entities
{
    public class CotacaoEntity
    {
        [Key]
        public Guid Id { get; set; }
        //TODO obter pelo login e rever a relação
        [Required]
        public string Corretor { get; set; }
        [NotMapped]
        public IList<CotacaoBensDTO> ListaBensDTO { get; set; }
        [NotMapped]
        public CotacaoInformacaoSeguroEntity InformacaoSeguro { get; set; }
        [NotMapped]
        public CotacaoInformacaoSeguradoEntity InformacaoSegurado { get; set; }
        [NotMapped]
        public IList<CotacaoInformacaoBemEntity> ListaBens { get; set; }
        [NotMapped]
        public IList<CotacaoFormularioRiscoEntity> ListaCotacaoFormularioRisco { get; set; }
        [NotMapped]
        public IList<CotacaoCoberturaEntity> ListaCotacaoCobertura { get; set; }
        [NotMapped]
        public CotacaoCondicaoComercialEntity CotacaoCondicaoComercial { get; set; }
        [NotMapped]
        public CotacaoInformacoesBeneficiarioEntity CotacaoInformacoesBeneficiario { get; set; }
        [NotMapped]
        public CotacaoRetornoSeguradoraEntity CotacaoRetornoSeguradora { get; set; }
        [NotMapped]
        public IList<CotacaoRetornoSeguradoraEntity> ListaCotacaoRetornoSeguradora { get; set; }
        [NotMapped]
        public CorretoraEntity Corretora { get; set; }
        [NotMapped]
        public SombreroCoberturaDTO SombreroCoberturaDTO { get; set; }
        [NotMapped]
        public IList<CorretoraEntity> ListaCorretoras { get; set; }
        [NotMapped]
        public IDictionary<string, IList<RelatorioDemonstrativoCoberturaDTO>> RelatorioDemonstrativoCoberturaDTO { get; set; }
        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime DataHoraCotacao { get; set; }
        [Required]
        public bool Cancelado { get; set; }
        [Required]
        public bool Efetivada { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTime DataHoraEfetivacao { get; set; }
        public string Seguradora { get; set; }
        public string JsonCotacao { get; set; }
        public string CodigoCotacao { get; set; }
        public string LinkAcessoCotacao { get; set; }
        [NotMapped]
        public int TotalItensFiltrados { get; set; }
        [NotMapped]
        public int TotalItensPagina { get; set; }
        [NotMapped]
        public IList<LoginSeguradoraEntity> LoginSeguradoras { get; set; }
        [Required]
        public decimal Premio { get; set; }
        [NotMapped]
        public bool Sucesso { get; set; }
        [NotMapped]
        public string Mensagem { get; set; }
    }
    public class CotacaoBensDTO
    {
        public Guid Id { get; set; }
        public CotacaoInformacaoBemEntity Equipamento { get; set; }
        public CotacaoFormularioRiscoEntity CotacaoFormularioRisco { get; set; }
        public CotacaoCoberturaEntity CotacaoCobertura { get; set; }
    }
    public class SombreroCoberturaDTO
    {
        public int IdProdutoCobertura { get; set; }
        public bool DvBasica { get; set; }
        public string DescricaoCobertura { get; set; }
        public decimal LimiteIsMinimo { get; set; }
        public decimal LimiteIsMaximo { get; set; }
        public List<SombreroFranquiaDTO> Franquias { get; set; }
        public bool DvObrigatoria { get; set; }
        public bool LimitarPelaBasica { get; set; }
        public int LimiteBasicaPercentual { get; set; }
        public string LimiteBasicaDescricao { get; set; }
        public string CoberturaBasica { get; set; }
        public string Transporte500Ou1000Km { get; set; }
        public string DespesasBuscaSalvamento { get; set; }
        public string ValorOperaProximoAgua { get; set; }
    }
    public class SombreroFranquiaDTO
    {
        public int IdTipoFranquia { get; set; }
        public string DescricaoFranquia { get; set; }
    }
}
