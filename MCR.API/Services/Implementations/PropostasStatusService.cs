using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MCR.API.Entities;
using MCR.API.Propostas.Domain.DTO;
using MCR.API.Repository;
using MCR.API.Services.Interfaces;
using MCR.API.Shared.Extensions;

namespace MCR.API.Services.Implementations;

public class PropostasStatusService : IPropostasStatusService
{
    private readonly DbContextMCR _context;
    private readonly IHttpContextAccessor _contextAccessor;

    private static readonly HashSet<string> StatusExigeTalhoesCompletos = new(StringComparer.Ordinal)
    {
        "Apólice emitida",
        "Proposta aceita",
        "Proposta aceita → Devolutiva da Seguradora",
    };

    private static readonly HashSet<string> StatusTerminais = new(StringComparer.Ordinal)
    {
        "Apólice cancelada",
        "Proposta recusada",
        "Proposta recusada → Devolutiva da Seguradora",
        "Proposta cancelada",
        "Cotação encerrada sem sucesso"
    };

    public PropostasStatusService(
        DbContextMCR context,
        IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _contextAccessor = contextAccessor;
    }

    public async Task<IEnumerable<PropostasStatusEntity>> ObterStatusPorPropostaAsync(Guid propostaId)
    {
        return await _context.PropostasStatus
            .Include(s => s.Usuario)
            .Where(s => s.PropostaId == propostaId)
            .OrderByDescending(s => s.DataStatus)
            .ToListAsync();
    }

    public async Task<IEnumerable<PropostasStatusDTO>> ObterPropostaStatusLista(Guid propostaId)
    {
        var statusList = await _context.PropostasStatus
            .Include(s => s.Usuario)
            .Where(s => s.PropostaId == propostaId)
            .OrderByDescending(s => s.DataStatus)
            .ToListAsync();

        return statusList.Select(s => new PropostasStatusDTO
        {
            Id = s.Id,
            PropostaId = s.PropostaId,
            UsuarioId = s.UsuarioId,
            Status = s.Status,
            StatusAnterior = s.StatusAnterior,
            DataStatus = s.DataStatus,
            NomeUsuario = s.Usuario?.Name
        }).ToList();
    }

    public List<string> ListarStatusProposta()
    {
        var todosStatus = new List<string>
        {
            "Cotação em negociação",
            "Proposta em negociação",
            "Aguardando transmissão",
            "Proposta com pendência",
            "Proposta transmitida (Em análise)",
            "Proposta aceita → Devolutiva da Seguradora",
            "Proposta recusada → Devolutiva da Seguradora",
            "Proposta cancelada",
            "Apólice emitida",
            "Apólice cancelada",
            "Cotação encerrada sem sucesso",
            "Solicitar Endosso",
            "Endosso com pendência",
            "Endosso Transmitido",
            "Endosso Emitido",
            "Comunicar Sinistro",
            "Sinistro Com Pendência",
            "Sinistro Cancelado",
            "Sinistro Aberto/Em Regulação",
            "Sinistro Aguardando Pagamento",
            "Sinistro deferido Pago",
            "Sinistro indeferido",
        };

        if (_contextAccessor.IsCorretor())
        {
            var statusCorretor = new List<string>
            {
                "Proposta cancelada",
                "Proposta com pendência",
                "Proposta transmitida (Em análise)",
                "Proposta aceita → Devolutiva da Seguradora",
                "Proposta recusada → Devolutiva da Seguradora",
                "Apólice emitida",
                "Apólice cancelada",
                "Solicitar Endosso",
                "Comunicar Sinistro"
            };
            return todosStatus.Where(status => statusCorretor.Contains(status)).ToList();
        }

        if (_contextAccessor.IsConsultor() || _contextAccessor.IsAssistente())
        {
            var statusConsultor = new List<string>
            {
                "Proposta em negociação",
                "Aguardando transmissão",
                "Cotação encerrada sem sucesso",
                "Proposta com pendência",
                "Endosso com pendência",
                "Endosso Transmitido",
                "Sinistro Com Pendência"
            };
            if (_contextAccessor.IsConsultor())
                return todosStatus.Where(status => statusConsultor.Contains(status)).ToList();
            statusConsultor.Add("Sinistro Aberto/Em Regulação");
            return todosStatus.Where(status => statusConsultor.Contains(status)).ToList();
        }

        return todosStatus;
    }

    public List<string> ListarStatusPropostaFiltrado(Guid propostaId)
    {
        var proposta = _context.Propostas.FirstOrDefault(x => x.Id == propostaId);
        var statusAtualProposta = proposta?.Status ?? string.Empty;

        var statusEndossoSinistro = new List<string>
        {
            "Solicitar Endosso", "Endosso com pendência", "Endosso Transmitido", "Endosso Emitido",
            "Comunicar Sinistro", "Sinistro Com Pendência", "Sinistro Cancelado",
            "Sinistro Aberto/Em Regulação", "Sinistro Aguardando Pagamento",
            "Sinistro deferido Pago", "Sinistro indeferido"
        };

        var transicoesPermitidas = statusAtualProposta switch
        {
            "Proposta em negociação" => new List<string> { "Cotação encerrada sem sucesso" },
            "Aguardando transmissão" => new List<string> { "Proposta com pendência", "Proposta transmitida (Em análise)", "Proposta cancelada" },
            "Proposta com pendência" => new List<string> { "Aguardando transmissão", "Proposta cancelada" },
            "Proposta transmitida (Em análise)" => new List<string> { "Proposta cancelada", "Proposta aceita → Devolutiva da Seguradora", "Proposta recusada → Devolutiva da Seguradora" },
            "Proposta aceita → Devolutiva da Seguradora" => new List<string> { "Apólice emitida", "Proposta cancelada" },
            "Apólice emitida" => new List<string> { "Apólice cancelada", "Proposta cancelada", "Solicitar Endosso", "Comunicar Sinistro" },
            "Solicitar Endosso" => new List<string> { "Endosso com pendência", "Endosso Transmitido" },
            "Endosso com pendência" => new List<string> { "Solicitar Endosso", "Endosso Transmitido" },
            "Endosso Transmitido" => new List<string> { "Endosso Emitido", "Endosso com pendência" },
            "Endosso Emitido" => new List<string> { "Endosso com pendência", "Comunicar Sinistro" },
            "Comunicar Sinistro" => _contextAccessor.IsCorretor()
                ? new List<string> { "Sinistro Cancelado", "Sinistro Com Pendência", "Sinistro Aberto/Em Regulação" }
                : new List<string> { "Sinistro Com Pendência", "Sinistro Cancelado" },
            "Sinistro Com Pendência" => _contextAccessor.IsConsultor()
                ? new List<string> { "Comunicar Sinistro" }
                : new List<string> { "Comunicar Sinistro", "Sinistro Aberto/Em Regulação" },
            "Sinistro Cancelado" => new List<string> { "Comunicar Sinistro" },
            "Sinistro Aberto/Em Regulação" => _contextAccessor.IsCorretor()
                ? new List<string> { "Sinistro Aguardando Pagamento", "Sinistro indeferido" }
                : new List<string> { "Sinistro Com Pendência", "Sinistro Aguardando Pagamento", "Sinistro indeferido" },
            "Sinistro Aguardando Pagamento" => _contextAccessor.IsCorretor()
                ? new List<string> { "Sinistro deferido Pago" }
                : new List<string> { "Sinistro deferido Pago", "Sinistro indeferido" },
            "Sinistro deferido Pago" => new List<string> { "Sinistro Com Pendência" },
            "Sinistro indeferido" => new List<string> { "Sinistro Com Pendência", "Sinistro Aberto/Em Regulação" },
            _ => new List<string>()
        };

        if (!string.IsNullOrEmpty(statusAtualProposta) && !transicoesPermitidas.Contains(statusAtualProposta))
            transicoesPermitidas.Add(statusAtualProposta);

        if (statusAtualProposta == "Proposta em negociação")
        {
            var todosStatus = ListarStatusProposta();
            return todosStatus.Where(s => !statusEndossoSinistro.Contains(s)).ToList();
        }

        if (_contextAccessor.IsAdministrador())
            return ListarStatusProposta();

        return transicoesPermitidas;
    }

    public async Task<(bool Sucesso, string Mensagem)> SalvarStatusProposta(PropostasStatusDTO model, Guid usuarioId)
    {
        try
        {
            var proposta = await _context.Propostas.Where(x => x.Id == model.PropostaId).FirstOrDefaultAsync();
            if (proposta == null) return (false, "Proposta não encontrada!");

            var statusPermitidos = ListarStatusPropostaFiltrado(model.PropostaId);
            if (statusPermitidos.Any() && !statusPermitidos.Contains(model.Status))
                return (false, "Transição de status não permitida para esta proposta.");

            if (StatusTerminais.Contains(proposta.Status))
                return (false, "Proposta está em status terminal. Não é possível alterar.");

            if (StatusExigeTalhoesCompletos.Contains(model.Status))
            {
                var riscos = await _context.PropostasClientePropriedades
                    .Where(r => r.PropostaId == model.PropostaId)
                    .ToListAsync();

                if (!riscos.Any())
                    return (false, "Proposta não possui riscos/talhões cadastrados.");

                foreach (var risco in riscos)
                {
                    // Verificar talhões vinculados diretamente ao risco
                    var temTalhoes = await _context.PropostasClientePropriedadesTalhoes
                        .AnyAsync(t => t.PropostasClientePropriedadeId == risco.Id);

                    // Se não tem no risco, verificar se a propriedade vinculada tem talhões
                    if (!temTalhoes)
                    {
                        temTalhoes = await _context.Talhoes
                            .AnyAsync(t => t.PropriedadeId == risco.PropriedadeId);
                    }

                    if (!temTalhoes)
                        return (false, "Risco não possui talhões cadastrados.");

                    // Validar dados dos talhões vinculados ao risco (se existirem)
                    var talhoesDoRisco = await _context.PropostasClientePropriedadesTalhoes
                        .Where(t => t.PropostasClientePropriedadeId == risco.Id)
                        .ToListAsync();

                    foreach (var t in talhoesDoRisco)
                    {
                        if (!t.DataPlantio.HasValue) return (false, "Todos os talhões devem ter Data de Plantio preenchida.");
                        if (!t.GrupoVariedadeId.HasValue || t.GrupoVariedadeId.Value == Guid.Empty) return (false, "Todos os talhões devem ter Grupo de Variedade selecionado.");
                        if (!t.VariedadeId.HasValue || t.VariedadeId.Value == Guid.Empty) return (false, "Todos os talhões devem ter Variedade selecionada.");
                    }
                }
            }

            var perfilUsuario = _contextAccessor.GetRole();

            var lastStatus = _context.PropostasStatus
                .Where(x => x.PropostaId == model.PropostaId)
                .OrderByDescending(o => o.DataStatus)
                .FirstOrDefault();

            var baseEntity = new PropostasStatusEntity
            {
                Id = Guid.NewGuid(),
                PropostaId = model.PropostaId,
                UsuarioId = usuarioId,
                Status = model.Status,
                StatusAnterior = lastStatus?.Status,
                UsuarioPerfil = perfilUsuario,
                DataStatus = DateTime.UtcNow
            };

            PropostasStatusEntity entityToSave = baseEntity;

            switch (model.Status)
            {
                case "Cotação encerrada sem sucesso":
                case "Proposta cancelada":
                case "Proposta recusada":
                case "Apólice cancelada":
                    if (model.JustificativaDTO == null) return (false, "Justificativa é obrigatória.");
                    entityToSave = new PropostaStatusJustificativaEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Justificativa = model.JustificativaDTO.Justificativa ?? string.Empty
                    };
                    break;
                case "Proposta com pendência":
                    if (model.PendenciaDTO == null) return (false, "Dados da pendência são obrigatórios.");
                    entityToSave = new PropostaStatusPendenciaEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        UploadArquivo = model.PendenciaDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.PendenciaDTO.UploadArquivoNome ?? string.Empty,
                        RetornoPendencia = model.PendenciaDTO.RetornoPendencia ?? string.Empty,
                        TipoDocumento = model.PendenciaDTO.TipoDocumento ?? string.Empty
                    };
                    break;
                case "Proposta aceita":
                case "Proposta aceita → Devolutiva da Seguradora":
                    if (model.AceitaDTO == null) return (false, "Dados da proposta aceita são obrigatórios.");
                    entityToSave = new PropostaStatusAceitaEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        PropostaSeguradora = model.AceitaDTO.PropostaSeguradora ?? string.Empty,
                        PropostaSeguradoraNome = model.AceitaDTO.PropostaSeguradoraNome ?? string.Empty,
                        NumeroProposta = model.AceitaDTO.NumeroProposta ?? string.Empty,
                        LmiTotal = model.AceitaDTO.LmiTotal ?? string.Empty,
                        PremioTotal = model.AceitaDTO.PremioTotal ?? string.Empty,
                        SubFederal = model.AceitaDTO.SubFederal ?? string.Empty,
                        SubEstadual = model.AceitaDTO.SubEstadual ?? string.Empty,
                        ParcelaSegurado = model.AceitaDTO.ParcelaSegurado,
                        Boleto = model.AceitaDTO.Boleto, BoletoNome = model.AceitaDTO.BoletoNome,
                        DataVencimento = model.AceitaDTO.DataVencimento
                    };
                    break;
                case "Apólice emitida":
                    if (model.ApoliceEmitidaDTO == null) return (false, "Dados da apólice são obrigatórios.");
                    entityToSave = new PropostaStatusApoliceEmitidaEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        ApoliceEmitida = model.ApoliceEmitidaDTO.ApoliceEmitida ?? string.Empty,
                        ApoliceEmitidaNome = model.ApoliceEmitidaDTO.ApoliceEmitidaNome ?? string.Empty,
                        NumeroApolice = model.ApoliceEmitidaDTO.NumeroApolice ?? string.Empty,
                        InicioVigencia = model.ApoliceEmitidaDTO.InicioVigencia,
                        FinalVigencia = model.ApoliceEmitidaDTO.FinalVigencia,
                        Boleto = model.ApoliceEmitidaDTO.Boleto, BoletoNome = model.ApoliceEmitidaDTO.BoletoNome,
                        DataVencimento = model.ApoliceEmitidaDTO.DataVencimento
                    };
                    break;
                case "Solicitar Endosso":
                    if (model.EndossoSolicitacaoDTO == null) return (false, "Dados do endosso são obrigatórios.");
                    entityToSave = new PropostaStatusEndossoSolicitacaoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.EndossoSolicitacaoDTO.Acao ?? "Solicitar Endosso",
                        UploadArquivo = model.EndossoSolicitacaoDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.EndossoSolicitacaoDTO.UploadArquivoNome ?? string.Empty,
                        DescricaoAlteracao = model.EndossoSolicitacaoDTO.DescricaoAlteracao,
                        RetornoPendencia = model.EndossoSolicitacaoDTO.RetornoPendencia
                    };
                    break;
                case "Endosso com pendência":
                    if (model.EndossoPendenciaDTO == null) return (false, "Dados da pendência de endosso são obrigatórios.");
                    entityToSave = new PropostaStatusEndossoPendenciaEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.EndossoPendenciaDTO.Acao ?? "Endosso com pendência",
                        UploadArquivo = model.EndossoPendenciaDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.EndossoPendenciaDTO.UploadArquivoNome ?? string.Empty,
                        DescricaoPendencia = model.EndossoPendenciaDTO.DescricaoPendencia ?? string.Empty,
                        TipoDocumento = model.EndossoPendenciaDTO.TipoDocumento ?? string.Empty
                    };
                    break;
                case "Endosso Transmitido":
                    if (model.EndossoTransmitidoDTO == null) return (false, "Dados do endosso transmitido são obrigatórios.");
                    entityToSave = new PropostaStatusEndossoTransmitidoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.EndossoTransmitidoDTO.Acao ?? "Endosso Transmitido",
                        NumeroEndosso = model.EndossoTransmitidoDTO.NumeroEndosso,
                        UploadArquivo = model.EndossoTransmitidoDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.EndossoTransmitidoDTO.UploadArquivoNome ?? string.Empty,
                        TipoDocumento = model.EndossoTransmitidoDTO.TipoDocumento,
                        Observacoes = model.EndossoTransmitidoDTO.Observacoes
                    };
                    break;
                case "Endosso Emitido":
                    if (model.EndossoEmitidoDTO == null) return (false, "Dados do endosso emitido são obrigatórios.");
                    entityToSave = new PropostaStatusEndossoEmitidoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.EndossoEmitidoDTO.Acao ?? "Endosso Emitido",
                        NumeroEndosso = model.EndossoEmitidoDTO.NumeroEndosso ?? string.Empty,
                        UploadArquivo = model.EndossoEmitidoDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.EndossoEmitidoDTO.UploadArquivoNome ?? string.Empty,
                        TipoDocumento = model.EndossoEmitidoDTO.TipoDocumento,
                        LMITotal = model.EndossoEmitidoDTO.LMITotal, PremioTotal = model.EndossoEmitidoDTO.PremioTotal,
                        SubFederal = model.EndossoEmitidoDTO.SubFederal, SubEstadual = model.EndossoEmitidoDTO.SubEstadual,
                        ParcSegurado = model.EndossoEmitidoDTO.ParcSegurado, AreaTotal = model.EndossoEmitidoDTO.AreaTotal,
                        InicioVigencia = model.EndossoEmitidoDTO.InicioVigencia, FimVigencia = model.EndossoEmitidoDTO.FimVigencia,
                        Observacoes = model.EndossoEmitidoDTO.Observacoes
                    };
                    break;
                case "Comunicar Sinistro":
                    if (model.SinistroComunicarDTO == null) return (false, "Dados do sinistro são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroComunicarEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroComunicarDTO.Acao ?? "Comunicar Sinistro",
                        Cobertura = model.SinistroComunicarDTO.Cobertura ?? string.Empty,
                        Evento = model.SinistroComunicarDTO.Evento ?? string.Empty,
                        SeveridadeDano = model.SinistroComunicarDTO.SeveridadeDano,
                        DanoEstimado = model.SinistroComunicarDTO.DanoEstimado,
                        AreaTotalAfetada = model.SinistroComunicarDTO.AreaTotalAfetada,
                        DataInicio = model.SinistroComunicarDTO.DataInicio, DataFinal = model.SinistroComunicarDTO.DataFinal,
                        TipoRespVistoria = model.SinistroComunicarDTO.TipoRespVistoria,
                        NomeRespVistoria = model.SinistroComunicarDTO.NomeRespVistoria,
                        CPFRespVistoria = model.SinistroComunicarDTO.CPFRespVistoria,
                        Observacao = model.SinistroComunicarDTO.Observacao
                    };
                    break;
                case "Sinistro Com Pendência":
                    if (model.SinistroPendenciaDTO == null) return (false, "Dados da pendência do sinistro são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroPendenciaEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroPendenciaDTO.Acao ?? "Sinistro Com Pendência",
                        UploadArquivo = model.SinistroPendenciaDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.SinistroPendenciaDTO.UploadArquivoNome ?? string.Empty,
                        RetornoPendencia = model.SinistroPendenciaDTO.RetornoPendencia ?? string.Empty,
                        TipoDocumento = model.SinistroPendenciaDTO.TipoDocumento ?? string.Empty
                    };
                    break;
                case "Sinistro Cancelado":
                    if (model.SinistroCanceladoDTO == null) return (false, "Dados do cancelamento são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroCanceladoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroCanceladoDTO.Acao ?? "Sinistro Cancelado",
                        MotivoCancelamento = model.SinistroCanceladoDTO.MotivoCancelamento ?? string.Empty
                    };
                    break;
                case "Sinistro Aberto/Em Regulação":
                    if (model.SinistroAbertoRegulacaoDTO == null) return (false, "Dados da regulação são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroAbertoRegulacaoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroAbertoRegulacaoDTO.Acao ?? "Sinistro Aberto/Em Regulação",
                        UploadArquivo = model.SinistroAbertoRegulacaoDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.SinistroAbertoRegulacaoDTO.UploadArquivoNome ?? string.Empty,
                        TipoDocumento = model.SinistroAbertoRegulacaoDTO.TipoDocumento,
                        ProtocoloAvisoSinistro = model.SinistroAbertoRegulacaoDTO.ProtocoloAvisoSinistro,
                        DataAvisoSinistro = model.SinistroAbertoRegulacaoDTO.DataAvisoSinistro,
                        EmpresaPerito = model.SinistroAbertoRegulacaoDTO.EmpresaPerito,
                        Telefone = model.SinistroAbertoRegulacaoDTO.Telefone,
                        Observacoes = model.SinistroAbertoRegulacaoDTO.Observacoes
                    };
                    break;
                case "Sinistro Aguardando Pagamento":
                    if (model.SinistroAguardPagamentoDTO == null) return (false, "Dados do pagamento são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroAguardPagamentoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroAguardPagamentoDTO.Acao ?? "Sinistro Aguardando Pagamento",
                        UploadArquivo = model.SinistroAguardPagamentoDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.SinistroAguardPagamentoDTO.UploadArquivoNome ?? string.Empty,
                        TipoDocumento = model.SinistroAguardPagamentoDTO.TipoDocumento,
                        DataDeferimento = model.SinistroAguardPagamentoDTO.DataDeferimento,
                        ValorIndenizacao = model.SinistroAguardPagamentoDTO.ValorIndenizacao,
                        Observacoes = model.SinistroAguardPagamentoDTO.Observacoes
                    };
                    break;
                case "Sinistro deferido Pago":
                    if (model.SinistroDeferidoPagoDTO == null) return (false, "Dados do pagamento são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroDeferidoPagoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroDeferidoPagoDTO.Acao ?? "Sinistro deferido Pago",
                        DataPagamento = model.SinistroDeferidoPagoDTO.DataPagamento,
                        Observacoes = model.SinistroDeferidoPagoDTO.Observacoes
                    };
                    break;
                case "Sinistro indeferido":
                    if (model.SinistroIndeferidoDTO == null) return (false, "Dados da indeferência são obrigatórios.");
                    entityToSave = new PropostaStatusSinistroIndeferidoEntity
                    {
                        Id = baseEntity.Id, PropostaId = baseEntity.PropostaId, UsuarioId = baseEntity.UsuarioId,
                        Status = baseEntity.Status, StatusAnterior = baseEntity.StatusAnterior,
                        DataStatus = baseEntity.DataStatus, UsuarioPerfil = perfilUsuario,
                        Acao = model.SinistroIndeferidoDTO.Acao ?? "Sinistro indeferido",
                        UploadArquivo = model.SinistroIndeferidoDTO.UploadArquivo ?? string.Empty,
                        UploadArquivoNome = model.SinistroIndeferidoDTO.UploadArquivoNome ?? string.Empty,
                        TipoDocumento = model.SinistroIndeferidoDTO.TipoDocumento,
                        DataIndeferimento = model.SinistroIndeferidoDTO.DataIndeferimento,
                        Observacoes = model.SinistroIndeferidoDTO.Observacoes
                    };
                    break;
            }

            proposta.Status = model.Status;
            _context.Entry(proposta).State = EntityState.Modified;
            await _context.PropostasStatus.AddAsync(entityToSave);
            await _context.SaveChangesAsync();

            // Salvar documentos anexados no status na tabela PropostasDocumentos
            await SalvarDocumentosDoStatus(model, baseEntity.PropostaId, baseEntity.UsuarioId, baseEntity.Status);

            return (true, "Status salvo com sucesso.");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao salvar status: {ex.Message}");
        }
    }

    public async Task<bool> SalvarStatusAsync(Guid propostaId, string status, string observacao, Guid usuarioId, byte[]? anexo = null)
    {
        try
        {
            var proposta = await _context.Propostas.Where(x => x.Id == propostaId).FirstOrDefaultAsync();
            if (proposta == null) return false;

            var statusPermitidos = ListarStatusPropostaFiltrado(propostaId);
            if (statusPermitidos.Any() && !statusPermitidos.Contains(status))
                return false;

            if (StatusTerminais.Contains(proposta.Status))
                return false;

            if (StatusExigeTalhoesCompletos.Contains(status))
            {
                var riscos = await _context.PropostasClientePropriedades
                    .Include(r => r.PropostasClientePropriedadesTalhoes)
                    .Where(r => r.PropostaId == propostaId)
                    .ToListAsync();

                if (!riscos.Any()) return false;

                foreach (var risco in riscos)
                {
                    if (!risco.PropostasClientePropriedadesTalhoes.Any()) return false;

                    foreach (var t in risco.PropostasClientePropriedadesTalhoes)
                    {
                        if (!t.DataPlantio.HasValue) return false;
                        if (!t.GrupoVariedadeId.HasValue || t.GrupoVariedadeId.Value == Guid.Empty) return false;
                        if (!t.VariedadeId.HasValue || t.VariedadeId.Value == Guid.Empty) return false;
                    }
                }
            }

            var lastStatus = _context.PropostasStatus
                .Where(x => x.PropostaId == propostaId)
                .OrderByDescending(o => o.DataStatus)
                .FirstOrDefault();

            var statusEntity = new PropostasStatusEntity
            {
                Id = Guid.NewGuid(),
                PropostaId = propostaId,
                UsuarioId = usuarioId,
                Status = status,
                StatusAnterior = lastStatus?.Status,
                DataStatus = DateTime.UtcNow
            };

            proposta.Status = status;
            _context.Entry(proposta).State = EntityState.Modified;

            await _context.PropostasStatus.AddAsync(statusEntity);
            await _context.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(byte[] Conteudo, string Nome)> DownloadAnexoStatusAsync(Guid statusId)
    {
        var statusEntity = await _context.PropostasStatus.OfType<PropostaStatusPendenciaEntity>()
            .FirstOrDefaultAsync(s => s.Id == statusId);

        if (statusEntity == null || string.IsNullOrEmpty(statusEntity.UploadArquivo))
            return (Array.Empty<byte>(), string.Empty);

        var conteudo = Convert.FromBase64String(statusEntity.UploadArquivo);
        return (conteudo, statusEntity.UploadArquivoNome ?? "anexo");
    }

    private static decimal? ParseDecimalNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;
        return null;
    }

    private static decimal ParseDecimal(string? value)
    {
        return ParseDecimalNullable(value) ?? 0m;
    }

    private async Task SalvarDocumentosDoStatus(PropostasStatusDTO model, Guid propostaId, Guid? usuarioId, string statusNovo)
    {
        var documentos = new List<(string Base64, string Nome, string Tipo)>();

        switch (statusNovo)
        {
            case "Apólice emitida" when model.ApoliceEmitidaDTO != null:
                if (!string.IsNullOrEmpty(model.ApoliceEmitidaDTO.ApoliceEmitida))
                    documentos.Add((model.ApoliceEmitidaDTO.ApoliceEmitida, model.ApoliceEmitidaDTO.ApoliceEmitidaNome ?? "Apolice.pdf", "Apólice Emitida"));
                if (!string.IsNullOrEmpty(model.ApoliceEmitidaDTO.Boleto))
                    documentos.Add((model.ApoliceEmitidaDTO.Boleto, model.ApoliceEmitidaDTO.BoletoNome ?? "Boleto.pdf", "Boleto"));
                break;

            case "Proposta aceita → Devolutiva da Seguradora" when model.AceitaDTO != null:
                if (!string.IsNullOrEmpty(model.AceitaDTO.PropostaSeguradora))
                    documentos.Add((model.AceitaDTO.PropostaSeguradora, model.AceitaDTO.PropostaSeguradoraNome ?? "PropostaSeguradora.pdf", "Proposta Seguradora"));
                if (!string.IsNullOrEmpty(model.AceitaDTO.Boleto))
                    documentos.Add((model.AceitaDTO.Boleto, model.AceitaDTO.BoletoNome ?? "Boleto.pdf", "Boleto"));
                break;

            case "Proposta com pendência" when model.PendenciaDTO != null:
                if (!string.IsNullOrEmpty(model.PendenciaDTO.UploadArquivo))
                    documentos.Add((model.PendenciaDTO.UploadArquivo, model.PendenciaDTO.UploadArquivoNome ?? "Pendencia.pdf", "Pendência"));
                break;

            case "Solicitar Endosso" when model.EndossoSolicitacaoDTO != null:
                if (!string.IsNullOrEmpty(model.EndossoSolicitacaoDTO.UploadArquivo))
                    documentos.Add((model.EndossoSolicitacaoDTO.UploadArquivo, model.EndossoSolicitacaoDTO.UploadArquivoNome ?? "Endosso.pdf", "Solicitação de Endosso"));
                break;

            case "Endosso Emitido" when model.EndossoEmitidoDTO != null:
                if (!string.IsNullOrEmpty(model.EndossoEmitidoDTO.UploadArquivo))
                    documentos.Add((model.EndossoEmitidoDTO.UploadArquivo, model.EndossoEmitidoDTO.UploadArquivoNome ?? "EndossoEmitido.pdf", "Endosso Emitido"));
                break;
        }

        foreach (var doc in documentos)
        {
            try
            {
                _context.PropostasDocumentos.Add(new PropostasDocumentosEntity
                {
                    Id = Guid.NewGuid(),
                    PropostaId = propostaId,
                    UserId = usuarioId,
                    TipoDocumento = doc.Tipo,
                    StatusProposta = statusNovo,
                    DataUpload = DateTime.UtcNow,
                    NomeArquivo = doc.Nome,
                    UrlDocumento = doc.Base64
                });
            }
            catch { }
        }

        if (documentos.Any())
            await _context.SaveChangesAsync();
    }
}
