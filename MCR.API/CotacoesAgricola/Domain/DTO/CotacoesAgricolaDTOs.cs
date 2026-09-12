using MCR.API.Entities;
using MCR.API.Shared.Extensions;

namespace MCR.API.CotacoesAgricola.Domain.DTO
{
    public class CotacoesAgricolaCadastrarDTO
    {
        public Guid? Id { get; set; }
        public CotacoesAgricolaCadastrarClienteDTO ClienteInfo { get; set; } = new();
        public Guid? CulturaId { get; set; }
        public Guid? SafraId { get; set; }
        public string? Estado { get; set; }
        public string? Municipio { get; set; }
        public decimal AreaTotal { get; set; }
        public bool IsModalidadeProdutividade { get; set; }
        public decimal? PrecoSaca { get; set; }
        public decimal? ValorCusteio { get; set; }
        public int? TipoSolo { get; set; }
        public string? ClassificacaoSolo { get; set; }
        public bool PlantioConsorciado { get; set; }
        public bool LavouraIrrigada { get; set; }
        public bool PlantioDireto { get; set; }
        public bool PosCana { get; set; }
        public decimal CustoProducao { get; set; }
        public bool SubvencaoFederal { get; set; }
        public bool SubvencaoEstadual { get; set; }
        public Guid? CorretoraId { get; set; }
        public Guid? CanalId { get; set; }
        public Guid? PontoAtendimentoId { get; set; }
        public Guid? UsuarioId { get; set; }
        public bool? Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public string? UsuarioNome { get; set; }
        public string? UsuarioEmail { get; set; }
        public string? UsuarioTelefone { get; set; }
        public DateTime? DataInsucesso { get; set; }
        public string? Status { get; set; }
        public bool? Excluido { get; set; }
        public string? CodigoCotacao { get; set; }

        public CotacoesAgricolaEntity ToEntity()
        {
            return new CotacoesAgricolaEntity
            {
                Id = Id ?? Guid.NewGuid(),
                ClienteId = Guid.TryParse(ClienteInfo?.ClienteId, out var clienteGuid) ? clienteGuid : null,
                ClienteCPF = ClienteInfo?.Cpf?.OnlyNumbers(),
                ClienteNome = ClienteInfo?.Nome ?? string.Empty,
                CulturaId = CulturaId ?? Guid.Empty,
                SafraId = SafraId ?? Guid.Empty,
                Estado = Estado,
                Municipio = Municipio,
                AreaTotal = AreaTotal,
                IsModalidadeProdutividade = IsModalidadeProdutividade,
                PrecoSaca = PrecoSaca,
                ValorCusteio = ValorCusteio,
                PlantioConsorciado = PlantioConsorciado,
                LavouraIrrigada = LavouraIrrigada,
                PlantioDireto = PlantioDireto,
                PosCana = PosCana,
                CustoProducao = CustoProducao,
                SubvencaoFederal = SubvencaoFederal,
                SubvencaoEstadual = SubvencaoEstadual,
                CorretoraId = CorretoraId,
                CanalId = CanalId,
                PontoAtendimentoId = PontoAtendimentoId,
                Excluido = Excluido ?? false,
                CotacoesAgricolaTipoSolo = TipoSolo.HasValue ? new List<CotacoesAgricolaTipoSoloEntity> { new() { TipoSolo = TipoSolo.Value } } : new List<CotacoesAgricolaTipoSoloEntity>(),
                CotacoesAgricolaClassificacaoSolo = !string.IsNullOrEmpty(ClassificacaoSolo) ? new List<CotacoesAgricolaClassificacaoSoloEntity> { new() { ClassificacaoSolo = ClassificacaoSolo } } : new List<CotacoesAgricolaClassificacaoSoloEntity>(),
            };
        }

        public static CotacoesAgricolaCadastrarDTO FromEntity(CotacoesAgricolaEntity entity)
        {
            return new CotacoesAgricolaCadastrarDTO
            {
                Id = entity.Id,
                ClienteInfo = new CotacoesAgricolaCadastrarClienteDTO
                {
                    ClienteId = entity.ClienteId?.ToString(),
                    Cpf = entity.Cliente?.CPF ?? entity.ClienteCPF ?? string.Empty,
                    Nome = entity.Cliente?.Nome ?? entity.ClienteNome ?? string.Empty
                },
                CulturaId = entity.CulturaId,
                SafraId = entity.SafraId,
                Estado = entity.Estado,
                Municipio = entity.Municipio,
                AreaTotal = entity.AreaTotal,
                IsModalidadeProdutividade = entity.IsModalidadeProdutividade,
                PrecoSaca = entity.PrecoSaca,
                ValorCusteio = entity.ValorCusteio,
                TipoSolo = entity.CotacoesAgricolaTipoSolo?.FirstOrDefault()?.TipoSolo,
                ClassificacaoSolo = entity.CotacoesAgricolaClassificacaoSolo?.FirstOrDefault()?.ClassificacaoSolo,
                PlantioConsorciado = entity.PlantioConsorciado,
                LavouraIrrigada = entity.LavouraIrrigada,
                PlantioDireto = entity.PlantioDireto,
                PosCana = entity.PosCana,
                CustoProducao = entity.CustoProducao,
                SubvencaoFederal = entity.SubvencaoFederal,
                SubvencaoEstadual = entity.SubvencaoEstadual,
                CorretoraId = entity.CorretoraId,
                CanalId = entity.CanalId,
                PontoAtendimentoId = entity.PontoAtendimentoId,
                Excluido = entity.Excluido,
                UsuarioId = entity.UsuarioId,
                UsuarioEmail = entity.Usuario?.Email ?? string.Empty,
                UsuarioTelefone = entity.Usuario?.PhoneNumber ?? string.Empty,
                UsuarioNome = entity.Usuario?.Name ?? string.Empty,
                Status = entity.Status,
                CodigoCotacao = entity.CodigoCotacao,
                DataInsucesso = entity.DataInsucesso,
            };
        }

        public static void UpdateEntity(CotacoesAgricolaEntity entity, CotacoesAgricolaCadastrarDTO dto)
        {
            entity.ClienteId = Guid.TryParse(dto.ClienteInfo?.ClienteId, out var clienteGuid) ? clienteGuid : null;
            entity.ClienteCPF = dto.ClienteInfo?.Cpf?.OnlyNumbers();
            entity.ClienteNome = dto.ClienteInfo?.Nome ?? string.Empty;
            entity.CulturaId = dto.CulturaId ?? Guid.Empty;
            entity.SafraId = dto.SafraId ?? Guid.Empty;
            entity.Estado = dto.Estado;
            entity.Municipio = dto.Municipio;
            entity.AreaTotal = dto.AreaTotal;
            entity.IsModalidadeProdutividade = dto.IsModalidadeProdutividade;
            entity.PrecoSaca = dto.PrecoSaca;
            entity.ValorCusteio = dto.ValorCusteio;
            entity.PlantioConsorciado = dto.PlantioConsorciado;
            entity.LavouraIrrigada = dto.LavouraIrrigada;
            entity.PlantioDireto = dto.PlantioDireto;
            entity.PosCana = dto.PosCana;
            entity.CustoProducao = dto.CustoProducao;
            entity.SubvencaoFederal = dto.SubvencaoFederal;
            entity.SubvencaoEstadual = dto.SubvencaoEstadual;
            entity.CorretoraId = dto.CorretoraId;
            entity.CanalId = dto.CanalId;
            entity.PontoAtendimentoId = dto.PontoAtendimentoId;
            if (dto.TipoSolo.HasValue)
                entity.CotacoesAgricolaTipoSolo = new List<CotacoesAgricolaTipoSoloEntity> { new() { TipoSolo = dto.TipoSolo.Value } };
            if (!string.IsNullOrEmpty(dto.ClassificacaoSolo))
                entity.CotacoesAgricolaClassificacaoSolo = new List<CotacoesAgricolaClassificacaoSoloEntity> { new() { ClassificacaoSolo = dto.ClassificacaoSolo } };
        }
    }

    public class CotacoesAgricolaCadastrarClienteDTO
    {
        public string? ClienteId { get; set; }
        public string? Cpf { get; set; } = string.Empty;
        public string? Nome { get; set; } = string.Empty;
    }

    public class CotacaoAgricolaDadosPropostasDTO
    {
        public List<CotacaoAgricolaPropostaDTO> Coberturas { get; set; } = new();
        public Guid CotacaoAgricolaId { get; set; }
        public bool HasClient { get; set; }
        public int QtdProdutosElegiveis { get; set; }
        public string? MotivoSemPropostas { get; set; }
        public string? DetalheTaxas { get; set; }
        public string? NomeProdutoElegivel { get; set; }
        public List<MotorCotacaoErroValidacaoDTO>? ErrosValidacao { get; set; }
    }

    public class CotacaoAgricolaPropostaDTO
    {
        public string? TipoOferta { get; set; }
        public int OpcaoSeguradora { get; set; }
        public string? Seguradora { get; set; }
        public string? NomeProduto { get; set; }
        public string? TiposSoloProduto { get; set; }
        public string? RegulacaoSinistro { get; set; }
        public string? ProdutividadeEsperada { get; set; }
        public decimal NivelCobertura { get; set; }
        public decimal ProdutividadeSegurada { get; set; }
        public decimal LMIProducaoHectare { get; set; }
        public decimal LMIReplantioHectare { get; set; }
        public decimal LMIProducaoTotal { get; set; }
        public decimal LMIReplantioTotal { get; set; }
        public string PremioTotal { get; set; } = string.Empty;
        public string SubvencaoFederal { get; set; } = string.Empty;
        public string SubvencaoEstadual { get; set; } = string.Empty;
        public string ParcelaSegurado { get; set; } = string.Empty;
        public string CustoHectare { get; set; } = string.Empty;
        public string SacasPorHa { get; set; } = string.Empty;
        public string SacasPorAlqueire { get; set; } = string.Empty;
        public Guid? CotacaoAgricolaPropostaId { get; set; }
    }

    public class CotacaoAgricolaPdfDTO
    {
        public Guid CotacaoAgricolaId { get; set; }
        public DateTime DataCotacao { get; set; }

        public string NumeroCotacao { get; set; } = string.Empty;
        public string Corretora { get; set; } = string.Empty;
        public string Canal { get; set; } = string.Empty;
        public string PA { get; set; } = string.Empty;
        public string Consultor { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Cultura { get; set; } = string.Empty;
        public string Safra { get; set; } = string.Empty;
        public string UF { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string NomeCliente { get; set; } = string.Empty;
        public string AreaTotal { get; set; } = string.Empty;
        public string TipoSolo { get; set; } = string.Empty;
        public string ClassificacaoSolo { get; set; } = string.Empty;
        public string PlantioConsorciado { get; set; } = string.Empty;
        public string PlantioDireto { get; set; } = string.Empty;
        public string PlantioPosCanal { get; set; } = string.Empty;
        public string LavouraIrrigada { get; set; } = string.Empty;

        public bool IsModalidadeProdutividade { get; set; }
        public string Modalidade { get; set; } = string.Empty;
        public string PrecoSaca { get; set; } = string.Empty;
        public string CusteioHa { get; set; } = string.Empty;

        public List<CoberturaPdfDTO> Coberturas { get; set; } = new();
    }

    public class CoberturaPdfDTO
    {
        public string? TipoOferta { get; set; }
        public int OpcaoSeguradora { get; set; }
        public string Seguradora { get; set; } = string.Empty;
        public string NomeProduto { get; set; } = string.Empty;
        public string RegulacaoSinistro { get; set; } = string.Empty;
        public string ProdutividadeEsperada { get; set; } = string.Empty;

        public Guid CotacaoAgricolaPropostaId { get; set; }
        public decimal NivelCobertura { get; set; }
        public decimal ProdutividadeSegurada { get; set; }
        public decimal LMIProducaoHectare { get; set; }
        public decimal LMIReplantioHectare { get; set; }
        public decimal LMIProducaoTotal { get; set; }
        public decimal LMIReplantioTotal { get; set; }

        public string PremioTotal { get; set; } = string.Empty;
        public string SubvencaoFederal { get; set; } = string.Empty;
        public string SubvencaoEstadual { get; set; } = string.Empty;
        public string ParcelaSegurado { get; set; } = string.Empty;
        public string CustoHectare { get; set; } = string.Empty;
        public string SacasPorHa { get; set; } = string.Empty;
        public string SacasPorAlqueire { get; set; } = string.Empty;
        public string? TiposSoloProduto { get; set; }
    }

    public class TipoSoloOpcaoDTO
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class ProdutoDisponivelDTO
    {
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Modalidade { get; set; } = string.Empty;
        public string Cultura { get; set; } = string.Empty;
        public string Safra { get; set; } = string.Empty;
        public string TipoSolo { get; set; } = string.Empty;
        public string ClassificacaoSolo { get; set; } = string.Empty;
        public decimal? ValorSacaMinimo { get; set; }
        public decimal? ValorSacaMaximo { get; set; }
        public decimal? ValorCusteioMinimo { get; set; }
        public decimal? ValorCusteioMaximo { get; set; }
        public decimal AreaMinima { get; set; }
        public int QtdTaxas { get; set; }
        public bool VinculadoCanalPA { get; set; }
        public bool AreaAtende { get; set; }
        public string TaxaMunicipio { get; set; } = string.Empty;
        public decimal TaxaNc65 { get; set; }
        public decimal TaxaNc70 { get; set; }
        public decimal TaxaNc75 { get; set; }
        public string CpfTaxa { get; set; } = string.Empty;
    }
}
