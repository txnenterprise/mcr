using System.ComponentModel.DataAnnotations;

namespace MCR.API.Propostas.Domain.DTO
{
    public class ObterPropostasPaginadoResponseDTO
    {
        public List<PropostaItemDTO> Propostas { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPages { get; set; }
        public int TamanhoPagina { get; set; }
    }

    public class PropostaItemDTO
    {
        public string Id { get; set; }
        public string CodigoCotacao { get; set; }
        public string PontoAtendimento { get; set; }
        public string Seguradora { get; set; }
        public string NomeProponente { get; set; }
        public string NomeRisco { get; set; }
        public decimal AreaTotalRisco { get; set; }
        public string MunicipioUF { get; set; }
        public string Status { get; set; }
    }

    public class PropostasCadastrarDTO
    {
        public Guid? PropostaId { get; set; }
        public string ClienteId { get; set; }
        public string Status { get; set; }
        public bool SeguradoSelected { get; set; }
    }

    public class PropostaDetalhesDTO
    {
        public Guid Id { get; set; }
        public Guid CotacaoAgricolaId { get; set; }
        public string? CodigoCotacao { get; set; }
        public Guid ClienteId { get; set; }
        public string? ClienteName { get; set; }
        public string? ClienteCpf { get; set; }
        public Guid CulturaId { get; set; }
        public string? CulturaName { get; set; }
        public Guid SafraId { get; set; }
        public string? SafraName { get; set; }
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public decimal AreaTotal { get; set; }
        public bool IsModalidadeProdutividade { get; set; }
        public decimal? PrecoSaca { get; set; }
        public decimal? ValorCusteio { get; set; }
        public bool PlantioConsorciado { get; set; }
        public bool LavouraIrrigada { get; set; }
        public bool PlantioDireto { get; set; }
        public bool PosCana { get; set; }
        public decimal CustoProducao { get; set; }
        public bool SubvencaoFederal { get; set; }
        public bool SubvencaoEstadual { get; set; }
        public Guid? CorretoraId { get; set; }
        public string? CorretoraName { get; set; }
        public Guid? CanalId { get; set; }
        public string? CanalName { get; set; }
        public Guid? PontoAtendimentoId { get; set; }
        public string? PontoAtendimentoName { get; set; }
        public DateTime? DataCotacao { get; set; }
        public string? Status { get; set; }
        public Guid UsuarioId { get; set; }
        public string? UsuarioName { get; set; }
        public string? UsuarioTelefone { get; set; }
        public string? UsuarioEmail { get; set; }
        public List<PropostaDetalheProdutoDTO> Produtos { get; set; } = new();
        public List<PropostaDetalheTipoSoloDTO> TiposSolo { get; set; } = new();
        public List<PropostaDetalheClassificacaoSoloDTO> ClassificacoesSolo { get; set; } = new();
        public List<PropostaDetalheSeguradoDTO> Segurados { get; set; } = new();
    }

    public class PropostaDetalheProdutoDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public int Opcao { get; set; }
        public Guid SeguradoraId { get; set; }
        public string? SeguradoraName { get; set; }
        public Guid ProdutoId { get; set; }
        public string? ProdutoName { get; set; }
        public string? RegulacaoSinistro { get; set; }
        public string? ProdutividadeEsperada { get; set; }
        public decimal NivelCobertura { get; set; }
        public decimal ProdutividadeSegurada { get; set; }
        public decimal LMIProducaoHectare { get; set; }
        public decimal LMIReplantioHectare { get; set; }
        public decimal LMIProducaoTotal { get; set; }
        public decimal LMIReplantioTotal { get; set; }
        public decimal PremioTotal { get; set; }
        public decimal SubvencaoFederal { get; set; }
        public decimal SubvencaoEstadual { get; set; }
        public decimal ParcelaSegurado { get; set; }
        public decimal CustoHectare { get; set; }
        public decimal SacasPorHa { get; set; }
        public decimal SacasPorAlqueire { get; set; }
    }

    public class PropostaDetalheTipoSoloDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public int TipoSolo { get; set; }
        public string TipoSoloName { get; set; } = string.Empty;
    }

    public class PropostaDetalheClassificacaoSoloDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public string? ClassificacaoSolo { get; set; }
        public string ClassificacaoSoloName { get; set; } = string.Empty;
    }

    public class PropostaDetalheSeguradoDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public Guid ClienteId { get; set; }
        public string? ClienteName { get; set; }
        public Guid? VinculoFamiliarId { get; set; }
        public string? VinculoFamiliarName { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? RelacaoParental { get; set; }
    }
    public class PropostasQuestionarioCulturasDTO
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class PropostasQuestionarioDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        [Display(Name = "Sistema de Plantio")]
        public string? SistemaPlantio { get; set; }
        [Display(Name = "Lavoura Plantada")]
        public bool LavouraPlantada { get; set; }
        [Display(Name = "Possui Danos Pre-existentes")]
        public bool? PossuiDanosPreExistentes { get; set; }
        [Display(Name = "Conhece e adota o ZARC")]
        public bool ConheceZARC { get; set; }
        [Display(Name = "Possui Outro Seguro")]
        public bool PossuiOutroSeguro { get; set; }
        [Display(Name = "Lavoura implantada após outra área")]
        public bool LavouraImplantadaAposOutraArea { get; set; }
        [Display(Name = "Notas fiscais em nome do próprio segurado")]
        public bool NotasFiscaisProprioSegurado { get; set; }
        public Guid? CulturaAnteriorId { get; set; }
        [Display(Name = "Cultura Anterior")]
        public string? CulturaAnterior { get; set; }
        [Display(Name = "Lavoura Irrigada")]
        public bool LavouraIrrigada { get; set; }
        [Display(Name = "Possui Crédito Bancário")]
        public bool PossuiCreditoBancario { get; set; }
        public List<PropostasQuestionarioFamiliarDTO> Familiares { get; set; } = new();
        public List<PropostasQuestionarioBancoDTO> Bancos { get; set; } = new();
    }

    public class PropostasQuestionarioFamiliarDTO
    {
        public Guid Id { get; set; }
        public Guid? QuestionarioId { get; set; }
        public Guid? ClienteId { get; set; }
        public Guid? VinculoFamiliarId { get; set; }
        public Guid? QuestionarioFamiliarId { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Relacao { get; set; }
        public bool Selecionado { get; set; }
    }

    public class PropostasQuestionarioBancoDTO
    {
        public Guid Id { get; set; }
        public Guid? PropostasQuestionarioBancosId { get; set; }
        public Guid? QuestionarioId { get; set; }
        public Guid? BeneficiarioId { get; set; }
        public string? NumeroCreditoCedula { get; set; }
        public DateTime? DataVencimento { get; set; }
        public bool Selecionado { get; set; }
        public bool IsCliente { get; set; }
        public PropostaBeneficiarioItemDTO? Beneficiario { get; set; }
    }

    public class PropostasRiscosDTO
    {
        public List<PropostasRiscoItemDTO> Riscos { get; set; } = new();
    }

    public class PropostasRiscoItemDTO
    {
        public Guid PropostaRiscoId { get; set; }
        public Guid PropriedadeId { get; set; }
        public string? NomePropriedade { get; set; }
        public string? Municipio { get; set; }
        public string? UF { get; set; }
        public decimal? AreaTotal { get; set; }
        public List<PropostasRiscoTalhaoDTO> Talhoes { get; set; } = new();
    }

    public class PropostasRiscoTalhaoDTO
    {
        public Guid PropostaTalhaoId { get; set; }
        public Guid TalhaoId { get; set; }
        public string? NomeTalhao { get; set; }
        public decimal? AreaTalhao { get; set; }
        public string? TipoSolo { get; set; }
        public string? ClassificacaoSolo { get; set; }
        public DateTime? DataPlantio { get; set; }
        public string? GrupoVariedadeNome { get; set; }
        public string? VariedadeNome { get; set; }
    }
    public class PropostasRiscosCadastrarDTO
    {
        public Guid PropostaId { get; set; }
        public Guid? ClienteId { get; set; }
        public List<PropostaRiscoCadastrarItemDTO> Riscos { get; set; } = new();
    }

    public class PropostaRiscoCadastrarItemDTO
    {
        public Guid PropriedadeId { get; set; }
        public string? NomePropriedade { get; set; }
        public string? Municipio { get; set; }
        public string? UF { get; set; }
        public decimal? AreaTotal { get; set; }
        public bool Selecionado { get; set; }
    }
    public class PropostasVistoriaDTO
    {
        public List<PropostasVistoriaPessoaDTO> PessoasVistoria { get; set; } = new();
    }

    public class PropostasVistoriaPessoaDTO
    {
        public Guid Id { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }
    }
    public class PropostasVistoriaCadastrarDTO
    {
        public PropostasVistoriaCadastrarDTO() { }
        public PropostasVistoriaCadastrarDTO(Guid propostaId, Guid? clienteId)
        {
            PropostaId = propostaId;
            ClienteId = clienteId;
        }
        public Guid PropostaId { get; set; }
        public Guid? ClienteId { get; set; }
        public PropostasVistoriaListaDTO? PessoasVistoria { get; set; }
        public List<PropostasVistoriaListaDTO> PessoasDisponiveis { get; set; } = new();
        public string? NomeNovo { get; set; }
        public string? CPFNovo { get; set; }
        public string? TelefoneNovo { get; set; }
    }

    public class PropostasVistoriaListaDTO
    {
        public Guid? Id { get; set; }
        public Guid? PropostasSeguradoId { get; set; }
        public Guid? VinculoFamiliarId { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }
        public string? RelacaoParental { get; set; }
    }
    public class PropostasTalhoesDTO
    {
        public List<PropostasRiscoTalhaoDTO> Talhoes { get; set; } = new();
    }
    public class PropostasTalhoesCadastrarDTO
    {
        public Guid PropostaId { get; set; }
        public List<PropostasTalhoesPropriedadeDTO> Propriedades { get; set; } = new();
    }

    public class PropostasTalhoesPropriedadeDTO
    {
        public Guid PropriedadeId { get; set; }
        public Guid PropostaPropriedadeId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public decimal AreaTotal { get; set; }
        public List<PropostasTalhoesItemDTO> Talhoes { get; set; } = new();
    }

    public class PropostasTalhoesItemDTO
    {
        public Guid TalhaoId { get; set; }
        public Guid PropriedadeId { get; set; }
        public string NomeTalhao { get; set; } = string.Empty;
        public decimal AreaTalhao { get; set; }
        public string? TipoSolo { get; set; }
        public string? ClassificacaoSolo { get; set; }
        public DateTime? DataPlantio { get; set; }
        public Guid? GrupoVariedadeId { get; set; }
        public Guid? VariedadeId { get; set; }
        public bool Selecionado { get; set; }
    }
    public class PropostasBeneficiariosDTO
    {
        public Guid PropostaId { get; set; }
        public List<PropostaBeneficiarioItemDTO> PessoasBeneficiarios { get; set; } = new();
    }

    public class PropostaBeneficiarioItemDTO
    {
        public Guid Id { get; set; }
        public Guid? ClienteId { get; set; }
        public Guid? BeneficiarioId { get; set; }
        public bool Selecionado { get; set; }
        public string? Nome { get; set; }
        public string? Documento { get; set; }
        public string? CNPJ { get; set; }
        public decimal? Percentual { get; set; }
        public string? Banco { get; set; }
        public string? Agencia { get; set; }
        public string? Conta { get; set; }
        public string? ChavePIX { get; set; }
    }
    public class PropostasDocumentosListaDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string? StatusProposta { get; set; }
        public DateTime DataUpload { get; set; }
        public string NomeArquivo { get; set; } = string.Empty;
        public string? NomeUsuario { get; set; }
        public string? UrlDocumento { get; set; }
    }
    public class PropostasDocumentosUploadDTO
    {
        public PropostasDocumentosUploadDTO() { }
        public PropostasDocumentosUploadDTO(Guid propostaId) { PropostaId = propostaId; }
        public Guid PropostaId { get; set; }
        public string TipoDocumento { get; set; }
        public string ArquivoBase64 { get; set; }
        public string? ArquivoPath { get; set; }
        public string? ArquivoNome { get; set; }
    }
    public class PropostasObservacoesDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public Guid? UsuarioId { get; set; }
        public string? Observacao { get; set; }
        public List<PropostaObservacaoItemDTO> Observacoes { get; set; } = new();
    }

    public class PropostaObservacaoItemDTO
    {
        public Guid Id { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public string? NomeUsuario { get; set; }
    }

    public class PropostasFormaPagamentosDTO
    {
        public Guid Id { get; set; }
        public string? ProdutoName { get; set; }
        public string? FormaDePagamento { get; set; }
        public string? Parcelamento { get; set; }
    }

    public class PropostasOcorrenciaDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public string? NomeArquivo { get; set; }
        public string? CaminhoAnexo { get; set; }
        public string? NomeUsuario { get; set; }
    }
    public class PropostasSeguradosDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public Guid ClienteId { get; set; }
        public Guid? VinculoFamiliarId { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? RelacaoParental { get; set; }
    }
    public class PropostasTimelineDTO
    {
        public Guid Id { get; set; }
        public Guid PropostaId { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? NomeUsuario { get; set; }
        public string? UsuarioPerfil { get; set; }
        public string? Status { get; set; }
        public string? StatusAnterior { get; set; }
        public string? NomeArquivo { get; set; }
        public string? CaminhoAnexo { get; set; }

        public PropostasStatusJustificativaDTO? JustificativaDTO { get; set; }
        public PropostasStatusPendenciaDTO? PendenciaDTO { get; set; }
        public PropostasStatusAceitaDTO? AceitaDTO { get; set; }
        public PropostasStatusApoliceEmitidaDTO? ApoliceEmitidaDTO { get; set; }
        public PropostasStatusEndossoSolicitacaoDTO? EndossoSolicitacaoDTO { get; set; }
        public PropostasStatusEndossoPendenciaDTO? EndossoPendenciaDTO { get; set; }
        public PropostasStatusEndossoTransmitidoDTO? EndossoTransmitidoDTO { get; set; }
        public PropostasStatusEndossoEmitidoDTO? EndossoEmitidoDTO { get; set; }
        public PropostasStatusSinistroComunicarDTO? SinistroComunicarDTO { get; set; }
        public PropostasStatusSinistroPendenciaDTO? SinistroPendenciaDTO { get; set; }
        public PropostasStatusSinistroCanceladoDTO? SinistroCanceladoDTO { get; set; }
        public PropostasStatusSinistroAbertoRegulacaoDTO? SinistroAbertoRegulacaoDTO { get; set; }
        public PropostasStatusSinistroAguardPagamentoDTO? SinistroAguardPagamentoDTO { get; set; }
        public PropostasStatusSinistroDeferidoPagoDTO? SinistroDeferidoPagoDTO { get; set; }
        public PropostasStatusSinistroIndeferidoDTO? SinistroIndeferidoDTO { get; set; }
        public List<PropostasDocumentosListaDTO>? Documentos { get; set; }
    }
}
