using MCR.API.Entities;

namespace MCR.API.Models
{
    public class CotacaoModel
    {
        public CotacaoInformacaoSeguroModel InformacaoSeguro { get; set; }
        public CotacaoInformacaoSeguradoModel InformacaoSegurado { get; set; }
        public CotacaoInformacaoBemModel CotacaoInformacaoBem { get; set; }
        public CotacaoFormularioRiscoModel CotacaoFormularioRisco { get; set; }
        public CotacaoCoberturaModel CotacaoCobertura { get; set; }
        public CotacaoCondicaoComercialModel CotacaoCondicaoComercial { get; set; }
        public CotacaoInformacoesBeneficiarioModel CotacaoInformacoesBeneficiario { get; set; }
        public string JsonItensAdicionar { get; set; }
        public IList<CotacaoEntity> ListaCotacoes { get; set; }
        public ParametrizacaoModel Parametrizacao { get; set; }
        public bool CotacaoContrada { get; set; }
        public string NomeSeguradoraContratada { get; set; }
        public string Seguradora { get; set; }
        public Guid SeguradoraContratadaId { get; set; }
        public decimal ValorPremioContratado { get; set; }
        public DateTime DataHoraCotacao { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public CotacaoEntity Cotacao { get; set; }
        public IList<GrupoCotacaoInformacaoBem> BensAgrupados { get; set; }
        public IList<CotacaoRetornoSeguradoraEntity> ListaCotacaoRetornoSeguradora { get; set; }
        public CotacaoRetornoSeguradoraEntity CotacaoRetornoSeguradoraPottencial { get; set; }
        public CotacaoRetornoSeguradoraEntity CotacaoRetornoSeguradoraSombrero { get; set; }
        public CotacaoRetornoSeguradoraEntity CotacaoRetornoSeguradoraSwissRe { get; set; }
        public ParametrizacaoMultiCalculoParceiroEntity ParametrizacaoMultiCalculoParceiro { get; set; }
        public DateTime? DataHoraCotacaoInicio { get; set; }
        public DateTime? DataHoraCotacaoFim { get; set; }
        public string? NumeroCotacao { get; set; }
        public string? Corretor { get; set; }
        public string? CorretoraCnpj { get; set; }
        public string CorretorLogado { get; set; }
        public Guid? CotacaoId { get; set; }
        public int PaginaAtual { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalPaginas { get; set; }
        public Guid? CorretoraId { get; set; }
        public string NomeParceiro { get; set; }
        public string LogoParceiro { get; set; }
        public IList<CorretoraEntity> ListaCorretoras { get; set; }
        public SombreroCoberturas SombreroCoberturas { get; set; }
    }
    public class SombreroCoberturas
    {
        public string CoberturaBasica { get; set; }
        public string Transporte500Ou1000Km { get; set; }
        public string DespesasBuscaSalvamento { get; set; }
        public string ValorOperaProximoAgua { get; set; }
    }
    public class GrupoCotacaoInformacaoBem
    {
        public string TipoEquipamento { get; set; }
        public IList<CotacaoInformacaoBemEntity> ListaBens { get; set; }
        public int Quantidade { get; set; }
    }
    public class ItensJsonEquipamento
    {
        public string Id { get; set; }
        public string TipoEquipamento { get; set; }
        public string AnoFabricacao { get; set; }
        public string ValorEquipamento { get; set; }
        public string MarcaEquipamento { get; set; }
        public string ModeloEquipamento { get; set; }
        public string NumeroSerieEquipamento { get; set; }
        public string NumeroChassiEquipamento { get; set; }
        public string InformarNotaFiscal { get; set; }
        public string DataNotaFiscal { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string EquipamentoAlugadoDuranteVigencia { get; set; }
        public string EquipamentoCedidoTerceirosDuranteVigencia { get; set; }
        public string EquipamentoAtividadesRurais { get; set; }
        public string EquipamentoAtividadeFlorestal { get; set; }
        public string EquipamentoOperaProximoAgua { get; set; }
        public string SeguradoColaboradorOperador { get; set; }
        public string AplicarCoberturaTotal { get; set; }
        public string ContratarResponsabilidadeCivilMaquinariaAgricola { get; set; }
        public string ValorResponsabilidadeCivilMaquinariaAgricola { get; set; }
        public string ContratarResponsabilidadeCivilEmpregador { get; set; }
        public string ValorResponsabilidadeCivilEmpregador { get; set; }
        public string ContratarCoberturaRelativaPerdaPagamentoAluguel { get; set; }
        public string ValorCoberturaRelativaPerdaPagamentoAluguel { get; set; }
        public string ContratarFurtoSimples { get; set; }
        public string ValorFurtoSimples { get; set; }
        public string ContratarDanosEletricos { get; set; }
        public string ValorDanosEletricos { get; set; }
        public string ContratarQuebraVidros { get; set; }
        public string ValorQuebraVidros { get; set; }
    }
}