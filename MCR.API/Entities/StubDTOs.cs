namespace MCR.API.Entities
{
    public class RelatorioDemonstrativoCoberturaDTO { }
    public class RelatorioDemonstrativoCotacaoDTO { }
}

namespace MCR.API.CotacoesAgricola.Domain.DTO
{
    public class MotorCotacaoDiagnosticoDTO
    {
        public Guid CotacaoId { get; set; }
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public Guid? CulturaId { get; set; }
        public Guid? SafraId { get; set; }
        public Guid? CanalId { get; set; }
        public Guid? PontoAtendimentoId { get; set; }
        public decimal AreaTotal { get; set; }
        public bool IsModalidadeProdutividade { get; set; }
        public int QtdProdutosElegiveis { get; set; }
        public int QtdPropostasGravadas { get; set; }
        public string? MotivoSemPropostas { get; set; }
        public Guid? ProdutoIdElegivel { get; set; }
        public string? NomeProdutoElegivel { get; set; }
        public string? DetalheTaxas { get; set; }
        public List<MotorCotacaoErroValidacaoDTO>? ErrosValidacao { get; set; }
        public List<MotorCotacaoPassoDiagnosticoDTO>? PassoAPasso { get; set; }
    }

    public class MotorCotacaoErroValidacaoDTO
    {
        public string Campo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }

    public class MotorCotacaoPassoDiagnosticoDTO
    {
        public string Etapa { get; set; } = string.Empty;
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? Dica { get; set; }
        public List<ProdutoExcluidoDiagnosticoDTO>? ProdutosExcluidos { get; set; }
    }

    public class ProdutoExcluidoDiagnosticoDTO
    {
        public string NomeProduto { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
    }
}
