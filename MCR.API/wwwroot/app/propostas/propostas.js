var propostaJS = {
    init: function () {
        // Inicializar todas as listas
        propostaJS.initSeguradoList();
        propostaJS.initRiscoList();
        propostaJS.initTalhaoList();
        propostaJS.initVistoriaList();
        propostaJS.initBeneficiarioList();
        propostaJS.initOcorrenciasList();
        propostaJS.initQuestionario();
        propostaJS.initObservacoes();
        propostaJS.initStatus();
        propostaJS.initFormasPagamento();
        propostaJS.initDocumentos();
        
        // Inicializar botões da timeline
        propostaJS.initTimelineButtons();
        
        // Inicializar documentos se existir
        if (typeof propostasDocumentosJS !== 'undefined') {
            propostasDocumentosJS.init();
        }

        // Configurar botão de transmissão
        $("#btTransmitir").off("click").on("click", function () {
            propostaJS.confirmarEnviarTransmissao();
        });

        // Inicializar status das abas (apenas na página Cadastrar)
        if ($('#PropostaId').length > 0) {
            setTimeout(propostaJS.atualizarStatusAbas, 500);
        }

        // Ativar aba Status se a URL tiver hash #status
        if (window.location.hash === '#status') {
            setTimeout(function() {
                var statusTab = $('#status-tab');
                if (statusTab.length) {
                    statusTab.tab('show');
                }
            }, 300);
        }
    },

    // Função para atualizar o status das abas
    atualizarStatusAbas: function() {
        // Só executar se existir o campo hidden PropostaId (página Cadastrar/Gerenciar)
        var $el = $('#PropostaId');
        if ($el.length === 0) return;
        
        var propostaId = $el.val();
        if (!propostaId || propostaId === '00000000-0000-0000-0000-000000000000' || propostaId.length < 36) {
            return;
        }
        
        $.ajax({
            url: '/Propostas/ObterStatusValidacao',
            type: 'GET',
            data: { propostaId: propostaId },
            success: function(data) {
                propostaJS.atualizarIndicador('detalhes', true); // Sempre true pois é a aba principal
                propostaJS.atualizarIndicador('segurados', data.seguradosPreenchido);
                propostaJS.atualizarIndicador('riscos', data.riscosPreenchido);
                propostaJS.atualizarIndicador('vistoria', data.vistoriaPreenchido);
                propostaJS.atualizarIndicador('beneficiarios', data.beneficiariosPreenchido);
                propostaJS.atualizarIndicador('questionarios', data.questionariosPreenchido);
                propostaJS.atualizarIndicador('observacoes', data.observacoesPreenchido);
                propostaJS.atualizarIndicador('formas-pagamento', data.formasPagamentoPreenchido);
                propostaJS.atualizarIndicador('documentos', data.documentosPreenchido);
            },
            error: function(xhr, status, error) {
                // Erro silencioso para não poluir o console
            }
        });
    },

    atualizarIndicador: function(tabId, preenchido) {
        var tabButton = $('#' + tabId + '-tab');
        var indicator = $('#' + tabId + '-status');
        
        if (preenchido) {
            tabButton.addClass('completed');
            indicator.addClass('completed');
        } else {
            tabButton.removeClass('completed');
            indicator.removeClass('completed');
        }
    },

    onSuccessCallback: function (r) {
        if (r.success == true) {
            var produtoId = r.produtoId;
            notify.success("Produto Salvo", r.message, null);
            setTimeout(propostaJS.atualizarStatusAbas, 300);
        } else {
            notify.error("Atenção", r.message);
        }
    },

    openSegurado: function () {
        gl.RenderGet("PropostasSeguradosListar", "Propostas", { clienteId: $("#param_clienteId").val() }, "#modalSegurado-content", true, null, function () {
            propostaJS.initSegurados();
            $("#modalSegurado").fadeIn(300);
        });
    },

    openRiscos: function () {
        gl.RenderGet("PropostasRiscosRegistrar", "Propostas", { propostaId: $("#PropostaId").val(), clienteId: $("#param_clienteId").val() }, "#modalRiscos", true, null, function () {
            propostaJS.closeModalRiscos();
            $("#modalRiscos").fadeIn(300);
        });
    },

    openTalhoes: function (propriedadeId) {
        gl.RenderGet("PropostasTalhoesRegistrar", "Propostas", { propostaId: $("#PropostaId").val(), propriedadeId: propriedadeId }, "#modalTalhoes", true, null, function () {
            propostaJS.closeModalTalhoes();
            $("#modalTalhoes").fadeIn(300);
            
            // Inicializar o controle dos checkboxes após o modal ser carregado
            setTimeout(function() {
                propostaJS.initTalhaoCheckboxControl();
            }, 100);
        });
    },

    openVistoria: function () {
        gl.RenderGet("PropostasVistoriaRegistrar", "Propostas", { propostaId: $("#PropostaId").val(), clienteId: $("#param_clienteId").val() }, "#modalVistoria", true, null, function () {
            propostaJS.closeModalVistoria();
            $("#modalVistoria").fadeIn(300);
        });
    },

    openBeneficiario: function () {
        gl.RenderGet("PropostasBeneficiariosRegistrar", "Propostas", { propostaId: $("#PropostaId").val() }, "#modalBeneficiario", true, null, function () {
            propostaJS.closeModalBeneficiario();
            $("#modalBeneficiario").fadeIn(300);
        });
    },

    openStatus: function () {
        gl.RenderGet("PropostasStatusCadastrar", "Propostas", { propostaId: $("#PropostaId").val() }, "#modalStatus", true, null, function () {
            propostaJS.closeModalStatus();
            $("#modalStatus").fadeIn(300);
        });
    },

    openOcorrencia: function (propostaId) {
        gl.RenderGet("PropostasOcorrenciasRegistrar", "Propostas", { propostaId: propostaId }, "#modalOcorrencia", true, null, function () {
            propostaJS.closeModalOcorrencia();
            $("#modalOcorrencia").fadeIn(300);
        });
    },

    closeModalSegurado: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalSegurado").fadeOut(300);
        });
    },

    closeModalRiscos: function () {
        $(".btn-cancelar-modal").off("click").on("click", function () {
            $("#modalRiscos").fadeOut(300);
        });
    },

    closeModalTalhoes: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalTalhoes").fadeOut(300);
        });
    },

    closeModalVistoria: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalVistoria").fadeOut(300);
        });
    },

    closeModalBeneficiario: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalBeneficiario").fadeOut(300);
        });
    },

    closeModalStatus: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalStatus").fadeOut(300);
        });
    },

    closeModalOcorrencia: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalOcorrencia").fadeOut(300);
        });
        
        // Adicionar lógica para o botão cancelar
        $(".btn-cancelar-modal").off("click").on("click", function () {
            $("#modalOcorrencia").fadeOut(300);
        });
        
        // Adicionar lógica para o botão salvar
        $("#btnSalvarOcorrencia").off("click").on("click", function () {
            propostaJS.salvarOcorrencia();
        });
        
        // Adicionar lógica para upload de arquivo
        propostaJS.initUploadAnexoOcorrencia();
    },

    refreshSegurados: function () {
        gl.RenderGet("PropostasSegurados", "Propostas", { propostaId: $("#PropostaId").val() }, "#divSegurados", true, null, function () {
            propostaJS.initSeguradoList();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshRiscos: function () {
        gl.RenderGet("PropostasRiscos", "Propostas", { propostaId: $("#PropostaId").val(), clienteId: $("#param_clienteId").val() }, "#divRiscos", true, null, function () {
            propostaJS.initRiscoList();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshVistoria: function () {
        gl.RenderGet("PropostasVistoria", "Propostas", { propostaId: $("#PropostaId").val(), clienteId: $("#param_clienteId").val() }, "#divVistoria", true, null, function () {
            propostaJS.initVistoriaList();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshBeneficiarios: function () {
        gl.RenderGet("PropostasBeneficiarios", "Propostas", { propostaId: $("#PropostaId").val() }, "#divBeneficiarios", true, null, function () {
            propostaJS.initBeneficiarioList();
            propostaJS.refreshQuestionario();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshQuestionario: function () {
        gl.RenderGet("PropostasQuestionario", "Propostas", { propostaId: $("#PropostaId").val() }, "#divQuestionarios", true, null, function () {
            propostaJS.initQuestionario();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshObservacoes: function () {
        gl.RenderGet("PropostasObservacoes", "Propostas", { propostaId: $("#PropostaId").val() }, "#divObservacoes", true, null, function () {
            propostaJS.initObservacoes();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshFormasPagamento: function () {
        gl.RenderGet("PropostasFormaPagamentos", "Propostas", { propostaId: $("#PropostaId").val() }, "#divFormasPagamento", true, null, function () {
            propostaJS.initFormasPagamento();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    refreshDocumentos: function () {
        gl.RenderGet("PropostasDocumentos", "Propostas", { propostaId: $("#PropostaId").val() }, "#divDocumentos", true, null, function () {
            propostaJS.initDocumentos();
            setTimeout(propostaJS.atualizarStatusAbas, 100);
        });
    },

    initSegurados: function () {
        $(".btn-fechar-modal").off("click").on("click", function () {
            $("#modalSegurado").fadeOut(300);
        });
        $(".btnSegurado").off("click").on("click", function () {
            var propostaId = $("#PropostaId").val();
            var clienteId = $(this).data("cliente");
            var vinculoId = $(this).data("vinculo");

            gl.Post("VincularPropostaSegurado", "PropostasSegurados", { propostaId: propostaId, clienteId: clienteId, vinculoFamiliarId: vinculoId, __RequestVerificationToken: gl.GetToken() }, true, function (response) {
                if (response.success == true) {
                    propostaJS.refreshSegurados();
                    $("#modalSegurado").fadeOut(300);
                    notify.success("Proponentes", response.message, null);
                    setTimeout(propostaJS.atualizarStatusAbas, 300);
                } else {
                    notify.error(response.message);
                }
            });
        });
    },

    initSeguradoList: function () {
        $(".seguradoRemove").off("click").on("click", function () {
            var seguradoId = $(this).data("id");
            var nome = $(this).data("nome");
            propostaJS.confirmarRemoverSegurado(seguradoId, nome);
        });
    },

    initRiscoList: function () {
        $(".riscoRemover").off("click").bind("click", function () {
            var riscoId = $(this).data("risco");
            var propriedade = $(this).data("propriedade");
            propostaJS.confirmarRemoverRisco(riscoId, propriedade);
        });
        $(".talhaoAdd").off("click").bind("click", function () {
            var riscoId = $(this).data("risco");
            var propriedade = $(this).data("propriedade");
            propostaJS.openTalhoes([propriedade]);
        });

        $(".talhaoRemover").off("click").bind("click", function () {
            var talhaoId = $(this).data("talhao");
            var nome = $(this).data("nome");
            propostaJS.confirmarRemoverTalhao(talhaoId, nome);
        });
    },

    initVistoriaList: function () {
        $(".vistoriaRemover").off("click").bind("click", function () {
            var vistoriaId = $(this).data("vistoria");
            var nome = $(this).data("nome");
            propostaJS.confirmarRemoverVistoria(vistoriaId, nome);
        });
    },

    initOcorrenciasList: function () {
        $(".btRegistrarOcorrencia").off("click").on("click", function () {
            var propostaId = $(this).data("proposta");
            propostaJS.openOcorrencia(propostaId);
        });
    },

    onSuccessCallbackVistoria: function (r) {
        if (r.success == true) {
            propostaJS.refreshVistoria();
            $("#modalVistoria").fadeOut(300);
            notify.success("Vistoria", r.message, null);
            setTimeout(propostaJS.atualizarStatusAbas, 300);
        } else {
            notify.error("Atenção", r.message);
        }
    },

    onSuccessCallbackBeneficiarios: function (r) {
        if (r.success == true) {
            propostaJS.refreshBeneficiarios();
            $("#modalBeneficiario").fadeOut(300);
            notify.success("Beneficiários", r.message, null);
            setTimeout(propostaJS.atualizarStatusAbas, 300);
        } else {
            notify.error("Atenção", r.message);
        }
    },

    onSuccessCallbackRiscos: function (r) {
        if (r.success == true) {
            propostaJS.refreshRiscos();
            $("#modalRiscos").fadeOut(300);
            setTimeout(propostaJS.atualizarStatusAbas, 300);
        } else {
            notify.error("Atenção", r.message);
        }
    },

    onSuccessCallbackQuestionario: function (r) {
        if (r.success == true) {
            notify.success("Questionário", r.message, null);
            setTimeout(propostaJS.atualizarStatusAbas, 300);
        } else {
            notify.error("Atenção", r.message);
        }
    },
    onSuccessCallbackObservacao: function (r) {
        if (r.success == true) {
            notify.success("Observação", r.message, null);
            propostaJS.refreshObservacoes();
            setTimeout(propostaJS.atualizarStatusAbas, 300);
        } else {
            notify.error("Atenção", r.message);
        }
    },

    initTalhaoList: function () {
        $(".talhaoRemover").off("click").bind("click", function () {
            var talhaoId = $(this).data("talhao");
            var propriedade = $(this).data("propriedade");
            propostaJS.confirmarRemoverTalhao(talhaoId, propriedade);
        });

        // Adicionar funcionalidade para controlar campos quando checkbox é selecionado
        propostaJS.initTalhaoCheckboxControl();
    },

    // Nova função para controlar o estado dos campos do talhão
    initTalhaoCheckboxControl: function () {
        // Função para controlar o estado disabled dos campos
        function toggleTalhaoFields(checkbox) {
            var $row = $(checkbox).closest('tr');
            var $dataPlantioInput = $row.find('input[type="date"]');
            var $grupoVariedadeSelect = $row.find('.grupoVariedadeSelect');
            var $variedadeSelect = $row.find('.variedadeSelect');

            if (checkbox.checked) {
                $dataPlantioInput.prop('disabled', false);
                $grupoVariedadeSelect.prop('disabled', false);
                $variedadeSelect.prop('disabled', false);
                
                // Re-inicializar Select2 para os campos habilitados
                $grupoVariedadeSelect.select2({
                    placeholder: "Selecione...",
                    allowClear: true
                });
                $variedadeSelect.select2({
                    placeholder: "Selecione...",
                    allowClear: true
                });
            } else {
                $dataPlantioInput.prop('disabled', true);
                $grupoVariedadeSelect.prop('disabled', true);
                $variedadeSelect.prop('disabled', true);
                
                // Limpar valores quando desabilitado
                $dataPlantioInput.val('');
                $grupoVariedadeSelect.val('').trigger('change');
                $variedadeSelect.val('').trigger('change');
            }
        }

        // Evento para quando o checkbox for alterado
        $(document).on('change', '.select-talhao', function() {
            toggleTalhaoFields(this);
        });

        // Inicializar o estado dos campos baseado no estado inicial do checkbox
        $('.select-talhao').each(function() {
            toggleTalhaoFields(this);
        });

        // Evento para carregar variedades quando grupo for selecionado
        $(document).on('change', '.grupoVariedadeSelect', function() {
            var $row = $(this).closest('tr');
            var $variedadeSelect = $row.find('.variedadeSelect');
            var grupoId = $(this).val();

            // Limpar variedade select
            $variedadeSelect.empty().append('<option value="">Selecione...</option>');

            if (grupoId) {
                // Fazer requisição AJAX para buscar variedades
                $.ajax({
                    url: '/PropostasVistoria/ListarCulturaVariedadePorGrupoId',
                    type: 'GET',
                    data: { grupoId: grupoId },
                    success: function(data) {
                        if (data && data.length > 0) {
                            data.forEach(function(variedade) {
                                $variedadeSelect.append(
                                    $('<option></option>')
                                        .val(variedade.id)
                                        .text(variedade.text)
                                );
                            });
                        }
                    },
                    error: function() {
                        console.error('Erro ao carregar variedades');
                    }
                });
            }
        });
    },

    onSuccessCallbackTalhoes: function (r) {
        if (r.success == true) {
            notify.success("Talhões", r.message, null);
            // Recarregar a página para mostrar os talhões vinculados
            var propostaId = $("#PropostaId").val();
            if (propostaId && propostaId !== '00000000-0000-0000-0000-000000000000') {
                window.location.href = '/Propostas/Cadastrar/' + propostaId + '#risco';
            }
        } else {
            notify.error("Atenção", r.message);
        }
    },

    refreshTalhoes: function () {
        var propriedadeId = $("#propriedadeId").val();
        if (!propriedadeId || propriedadeId === "" || propriedadeId === "undefined") {
            $("#divTalhoes").html('');
            return;
        }
        gl.RenderGet("PropostasTalhoes", "Propostas",
            {
                propostaId: $("#PropostaId").val()
            },
            "#divTalhoes",
            true,
            null,
            function () {
                propostaJS.initTalhaoList();
            }
        );
    },

    confirmarRemoverTalhao: function (talhaoId, nome) {
        notify.confirmYesNo(
            "danger",
            "Confirmação de Exclusão",
            "Tem certeza que deseja remover este talhão: " + nome + "?",
            function (confirmed) {
                gl.Post("RemoverTalhao", "PropostasTalhoes", { talhaoPropostaId: talhaoId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                    if (r.success) {
                        propostaJS.refreshTalhoes();
                        notify.success("Talhões", r.message, null);
                        setTimeout(propostaJS.atualizarStatusAbas, 300);
                    } else {
                        notify.error("Atenção", r.message);
                    }
                });
            },
            null,
            "Sim, remover",
            "Cancelar"
        );
    },

    confirmarRemoverSegurado: function (seguradoId, nome) {
        notify.confirmYesNo(
            "danger",
            "Confirmação de Exclusão",
            "Tem certeza que deseja remover este proponente: " + nome + "?",
            function (confirmed) {
                gl.Post("RemoverPropostaSegurado", "PropostasSegurados", { propostaSeguradoId: seguradoId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                    if (r.success) {
                        $("#divRiscos").html('');
                        $("#divTalhoes").html('');
                        propostaJS.refreshSegurados();
                        notify.success("Proponentes", r.message, null);
                        setTimeout(propostaJS.atualizarStatusAbas, 300);
                    } else {
                        notify.error("Atenção", r.message);
                    }
                });
            },
            null,
            "Sim, remover",
            "Cancelar"
        );
    },

    confirmarRemoverVistoria: function (vistoriaId, nome) {
        notify.confirmYesNo(
            "danger",
            "Confirmação de Exclusão",
            "Tem certeza que deseja remover esta pessoa da vistoria: " + nome + "?",
            function (confirmed) {
                gl.Post("RemoverPropostaVistoria", "PropostasVistoria", { vistoriaId: vistoriaId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                    if (r.success) {
                        propostaJS.refreshVistoria();
                        notify.success("Vistoria", r.message, null);
                        setTimeout(propostaJS.atualizarStatusAbas, 300);
                    } else {
                        notify.error("Atenção", r.message);
                    }
                });
            },
            null,
            "Sim, remover",
            "Cancelar"
        );
    },

    confirmarRemoverBeneficiario: function (beneficiarioId, nome) {
        notify.confirmYesNo(
            "danger",
            "Confirmação de Exclusão",
            "Tem certeza que deseja remover este beneficiário: " + nome + "?",
            function (confirmed) {
                gl.Post("RemoverPropostaBeneficiario", "PropostasBeneficiarios", { propostaBeneficiarioId: beneficiarioId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                    if (r.success) {
                        propostaJS.refreshBeneficiarios();
                        notify.success("Beneficiários", r.message, null);
                        setTimeout(propostaJS.atualizarStatusAbas, 300);
                    } else {
                        notify.error("Atenção", r.message);
                    }
                });
            },
            null,
            "Sim, remover",
            "Cancelar"
        );
    },

    confirmarRemoverRisco: function (riscoId, nome) {
        notify.confirmYesNo(
            "danger",
            "Confirmação de Exclusão",
            "Tem certeza que deseja remover esta propriedade: " + nome + "?",
            function (confirmed) {
                gl.Post("RemoverPropostaRisco", "PropostasRiscos", { riscoId: riscoId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                    if (r.success) {
                        propostaJS.refreshRiscos();
                        notify.success("Riscos", r.message, null);
                        setTimeout(propostaJS.atualizarStatusAbas, 300);
                    } else {
                        notify.error("Atenção", r.message);
                    }
                });
            },
            null,
            "Sim, remover",
            "Cancelar"
        );
    },

    confirmarEnviarTransmissao: function () {
        var propostaId = $("#PropostaId").val();
        notify.confirmYesNo(
            "primary",
            "Confirmação de Transmissão",
            "Tem certeza que deseja enviar esta proposta para transmissão?",
            function (confirmed) {
                if (confirmed) {
                    gl.Post("EnviarTransmissao", "Propostas", { propostaId: propostaId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                        if (r.success) {
                            notify.success("Transmissão", r.message, null);
                            setTimeout(function () {
                                window.location.href = "/Propostas";
                            }, 2000);
                        } else {
                            notify.error("Atenção", r.message);
                        }
                    });
                }
            },
            null,
            "Sim, enviar",
            "Cancelar"
        );
    },

    initBeneficiarioList: function () {
        $(".beneficiarioRemover").off("click").on("click", function () {
            var beneficiarioId = $(this).data("beneficiario");
            var nome = $(this).data("nome");
            propostaJS.confirmarRemoverBeneficiario(beneficiarioId, nome);
        });
    },

    initQuestionario: function () {
        // Inicialização do questionário se necessário
        propostaJS.toggleDanosPreExistentes();
        propostaJS.toggleIntegranteFamiliar();
        propostaJS.toggleCreditoBancario();
        
        // Adicionar evento para preparar dados antes do envio
        $('#formQuestionario').on('submit', function() {
            propostaJS.prepararDadosQuestionario();
        });
    },

    toggleDanosPreExistentes: function () {
        var lavouraPlantada = $('input[name="LavouraPlantada"]:checked').val();
        var divDanosPreExistentes = $('#divDanosPreExistentes');
        
        if (lavouraPlantada === 'true') {
            divDanosPreExistentes.show();
        } else {
            divDanosPreExistentes.hide();
            // Limpar seleção quando esconder
            $('input[name="PossuiDanosPreExistentes"]').prop('checked', false);
        }
    },

    toggleIntegranteFamiliar: function () {
        var notasFiscaisProprioSegurado = $('input[name="NotasFiscaisProprioSegurado"]:checked').val();
        var divIntegranteFamiliar = $('#divIntegranteFamiliar');
        
        if (notasFiscaisProprioSegurado === 'false') {
            divIntegranteFamiliar.show();
        } else {
            divIntegranteFamiliar.hide();
            // Limpar seleção quando esconder
            $('input[name="familiaresSelecionado"]').prop('checked', false);
            $('input[name$="].Selecionado"]').val('false');
        }
    },

    toggleCreditoBancario: function () {
        var possuiCreditoBancario = $('input[name="PossuiCreditoBancario"]:checked').val();
        var divCreditoBancario = $('#divCreditoBancario');
        
        if (possuiCreditoBancario === 'true') {
            divCreditoBancario.show();
        } else {
            divCreditoBancario.hide();
            // Limpar seleção quando esconder
            $('input[name$="].Selecionado"]').prop('checked', false).val('false');
            // Desabilitar todos os campos de input
            $('input[name$="].NumeroCreditoCedula"]').prop('disabled', true).val('');
            $('input[name$="].DataVencimento"]').prop('disabled', true).val('');
        }
    },

    toggleBancoInputs: function (checkbox) {
        var $row = $(checkbox).closest('tr');
        var $numeroCreditoInput = $row.find('input[name$="].NumeroCreditoCedula"]');
        var $dataVencimentoInput = $row.find('input[name$="].DataVencimento"]');
        
        if (checkbox.checked) {
            $numeroCreditoInput.prop('disabled', false);
            $dataVencimentoInput.prop('disabled', false);
        } else {
            $numeroCreditoInput.prop('disabled', true).val('');
            $dataVencimentoInput.prop('disabled', true).val('');
        }
    },

    initObservacoes: function () {
        // Inicialização das observações se necessário
    },

    initStatus: function () {
        // Inicialização do status se necessário
    },

    initFormasPagamento: function () {
        // Inicialização das formas de pagamento se necessário
    },

    initDocumentos: function () {
        // Inicialização dos documentos se necessário
    },

    prepararDadosQuestionario: function () {
        // Preparar dados dos familiares (radio buttons)
        var familiarSelecionado = $('input[name="familiaresSelecionado"]:checked');
        if (familiarSelecionado.length > 0) {
            var index = familiarSelecionado.data('index');
            $('input[name="Familiares[' + index + '].Selecionado"]').val('true');
        }

        // Preparar dados dos bancos (checkboxes)
        $('input[name$="].Selecionado"]').each(function() {
            var $checkbox = $(this);
            var $hidden = $checkbox.siblings('input[type="hidden"][name$="].Selecionado"]');
            if ($checkbox.is(':checked')) {
                $hidden.val('true');
            } else {
                $hidden.val('false');
            }
        });
    },

    // Função para download de anexo de ocorrência
    downloadAnexoOcorrencia: function (ocorrenciaId, isImage = false) {
        try {
            // Criar um link temporário para download
            var link = document.createElement('a');
            link.href = '/PropostasOcorrencias/DownloadAnexoOcorrencia?ocorrenciaId=' + ocorrenciaId;
            link.target = '_blank';
            link.download = '';
            
            // Adicionar o link ao DOM, clicar e remover
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        } catch (error) {
            console.error('Erro ao fazer download do anexo:', error);
            notify.error("Erro", "Erro ao fazer download do arquivo.");
        }
    },

    // Adicionar função para salvar ocorrência
    salvarOcorrencia: function () {
        const descricao = $("#descricaoOcorrencia").val();
        const propostaId = $("#propostaIdOcorrencia").val();
        const arquivoBase64 = $("input[name='arquivoBase64']").val();
        const arquivoNome = $("input[name='arquivoNome']").val();
        
        // Validação básica
        if (!descricao || descricao.trim() === '') {
            notify.error("Erro", "A descrição da notificação é obrigatória.");
            return;
        }
        
        if (!propostaId) {
            notify.error("Erro", "ID da proposta não encontrado.");
            return;
        }
        
        // Desabilitar botão durante o envio
        const btnSalvar = $("#btnSalvarOcorrencia");
        const originalText = btnSalvar.text();
        btnSalvar.prop('disabled', true).text('Salvando...');
        
        // Preparar dados para envio
        const formData = new FormData();
        formData.append('propostaId', propostaId);
        formData.append('descricao', descricao.trim());
        
        if (arquivoBase64 && arquivoNome) {
            formData.append('arquivoBase64', arquivoBase64);
            formData.append('arquivoNome', arquivoNome);
        }
        
        // Adicionar token antiforgery
        formData.append('__RequestVerificationToken', gl.GetToken());
        
        // Fazer requisição AJAX
        $.ajax({
            url: '/PropostasOcorrencias/SalvarOcorrencia',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    notify.success("Sucesso", response.message || "Notificação registrada com sucesso!", function () {
                        // Fechar modal
                        $("#modalOcorrencia").fadeOut(300);
                        
                        // Limpar formulário
                        $("#formRegistrarOcorrencia")[0].reset();
                        $("#anexoInfo").empty();
                        
                        // Atualizar lista de ocorrências se existir
                        if (typeof propostaJS.refreshOcorrencias === 'function') {
                            propostaJS.refreshOcorrencias();
                        }
                        
                        // ATUALIZAR TIMELINE DINAMICAMENTE
                        propostaJS.refreshTimeline();
                    });
                } else {
                    notify.error("Erro", response.message || "Erro ao registrar notificação.");
                }
            },
            error: function (xhr, status, error) {
                notify.error("Erro", "Erro ao registrar notificação. Tente novamente.");
            },
            complete: function () {
                // Reabilitar botão
                btnSalvar.prop('disabled', false).text(originalText);
            }
        });
    },

    // Adicionar função para inicializar upload de anexo
    initUploadAnexoOcorrencia: function () {
        // Criar input file oculto
        if (!$("#fileInputOcorrencia").length) {
            $("body").append('<input type="file" id="fileInputOcorrencia" accept="image/*" style="display: none;" />');
        }
        
        // Configurar botão de upload
        $("#UploadAnexoOcorrencia").off("click").on("click", function () {
            $("#fileInputOcorrencia").click();
        });
        
        // Configurar evento de mudança do input file
        $("#fileInputOcorrencia").off("change").on("change", function () {
            const file = this.files[0];
            if (file) {
                
                // Validar tipo de arquivo
                if (!file.type.startsWith('image/')) {
                    notify.error("Erro", "Por favor, selecione apenas arquivos de imagem (JPG, PNG, GIF).");
                    return;
                }
                
                // Validar tamanho (5MB)
                if (file.size > 5 * 1024 * 1024) {
                    notify.error("Erro", "O arquivo deve ter no máximo 5MB.");
                    return;
                }
                
                // Ler arquivo como base64
                const reader = new FileReader();
                reader.onload = function (event) {
                    const dataUrl = event.target.result;
                    
                    // Extrair apenas o conteúdo base64, removendo o prefixo data:image/...
                    const base64String = dataUrl.split(',')[1];
                    
                    // Armazenar dados do arquivo
                    $("input[name='arquivoBase64']").val(base64String);
                    $("input[name='arquivoNome']").val(file.name);
                    
                    // Mostrar informações do arquivo
                    $("#anexoInfo").html(`
                        <div class="alert alert-success">
                            <i class="fas fa-check-circle me-2"></i>
                            Arquivo selecionado: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)
                            <button type="button" class="btn btn-sm btn-outline-danger ms-2" onclick="propostaJS.removerAnexoOcorrencia()">
                                <i class="fas fa-times"></i> Remover
                            </button>
                        </div>
                    `);
                };
                reader.readAsDataURL(file);
            }
        });
    },

    // Adicionar função para remover anexo
    removerAnexoOcorrencia: function () {
        $("input[name='arquivoBase64']").val('');
        $("input[name='arquivoNome']").val('');
        $("#anexoInfo").empty();
        $("#fileInputOcorrencia").val('');
    },

    // Adicionar função para atualizar lista de ocorrências
    refreshOcorrencias: function () {
        const propostaId = $("#PropostaId").val();
        if (propostaId) {
            gl.RenderGet("PropostasOcorrencias", "Propostas", { propostaId: propostaId }, "#divOcorrencias", true, null, function () {
                propostaJS.initOcorrenciasList();
                setTimeout(propostaJS.atualizarStatusAbas, 100);
            });
            
            // Também atualizar a timeline quando as ocorrências são atualizadas
            propostaJS.refreshTimeline();
        }
    },

    // Adicionar função para atualizar timeline
    refreshTimeline: function () {
        const propostaId = $("#PropostaId").val();
        if (propostaId) {
            gl.RenderGet("PropostasTimeline", "Propostas", { propostaId: propostaId }, "#divStatus", true, null, function () {
                // Re-inicializar os botões da timeline após a atualização
                propostaJS.initTimelineButtons();
                
                // Atualizar status das abas
                setTimeout(propostaJS.atualizarStatusAbas, 100);
            });
        }
    },

    // Adicionar função para inicializar botões da timeline
    initTimelineButtons: function () {
        // Re-inicializar botão de alterar status
        $(".btStatus").off("click").on("click", function () {
            var proposta = $(this).data("proposta");
            propostaStatusJs.openStatus(proposta);
        });
        
        // Re-inicializar botão de registrar ocorrência
        $(".btRegistrarOcorrencia").off("click").on("click", function () {
            var propostaId = $(this).data("proposta");
            propostaJS.openOcorrencia(propostaId);
        });
    }
};

// Atualizar status quando a página carrega
$(document).ready(function() {
    setTimeout(function() {
        propostaJS.init();
        setTimeout(propostaJS.atualizarStatusAbas, 1000);
    }, 500);
});
