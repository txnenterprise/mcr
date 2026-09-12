// Fallback: define customDropzone se mod-upload.js não carregou
if (typeof jQuery !== 'undefined' && typeof jQuery.fn.customDropzone === 'undefined') {
    (function ($) {
        $.fn.customDropzone = function (options) {
            var defaults = {
                url: "/File/UploadFile", maxFiles: 10, maxFilesize: 10, acceptedFiles: "image/*",
                addRemoveLinks: true, parallelUploads: 1, uploadMultiple: false, autoQueue: true,
                disablePreviews: true, clickable: true,
                onSending: function () { }, onQueueComplete: function () { },
                onSuccess: function () { }, onError: function () { }
            };
            var settings = $.extend({}, defaults, options);
            return this.each(function () {
                var $this = $(this);
                if ($this.data('dropzone-initialized')) return;
                if ($this.data('dropzone')) $this.data('dropzone').destroy();
                var isButton = $this.is('button');
                var $container = isButton ? ($this.parent('.dropzone-container').length ? $this.parent('.dropzone-container') : (function () { var c = $('<div class="dropzone-container"></div>'); $this.wrap(c); return $this.parent(); })()) : $this;
                var dz = new Dropzone($container[0], {
                    url: settings.url, maxFiles: settings.maxFiles, maxFilesize: settings.maxFilesize,
                    acceptedFiles: settings.acceptedFiles, addRemoveLinks: settings.addRemoveLinks,
                    parallelUploads: settings.parallelUploads, uploadMultiple: settings.uploadMultiple,
                    autoQueue: settings.autoQueue, disablePreviews: settings.disablePreviews,
                    clickable: isButton ? $this[0] : settings.clickable
                });
                dz.on("sending", settings.onSending);
                dz.on("queuecomplete", function () { settings.onQueueComplete(dz); dz.removeAllFiles(true); });
                dz.on("success", settings.onSuccess);
                dz.on("error", settings.onError);
                $container.data('dropzone', dz);
                $this.data('dropzone-initialized', true);
                if (isButton) $this.off('click').on('click', function (e) { e.preventDefault(); e.stopPropagation(); });
            });
        };
    }(jQuery));
    console.log("[Fallback] customDropzone definido via inline");
}

var propostaStatusJs = {
    normalizarParcelaSeguradoInvariant: function () {
        ["#AceitaDTO_ParcelaSegurado", "#EndossoEmitidoDTO_ParcSegurado"].forEach(function (sel) {
            var $el = $(sel);
            if (!$el.length) return;
            var v = $el.val();
            if (v == null || v === "") return;
            v = String(v).trim().replace(/\s/g, "");
            var lastComma = v.lastIndexOf(",");
            var lastDot = v.lastIndexOf(".");
            if (lastComma > lastDot) {
                v = v.replace(/\./g, "").replace(",", ".");
            } else {
                v = v.replace(/,/g, "");
            }
            $el.val(v);
        });
    },

    propostaId: null,
    onSuccessCallbackStatus: function (r) {
        if (r.success == true) {
            $("#modalStatus").fadeOut(300);
            notify.success("Status", r.message, null);

            var url = "/Propostas";
            if (r.status) {
                var s = r.status.toLowerCase();
                if (s.includes("apólice") || s.includes("apolice"))
                    url = "/Apolices";
                else if (s.includes("sinistro") || s.includes("comunicar_sinistro") || s.includes("deferido") || s.includes("indeferido"))
                    url = "/Sinistro";
                else if (s.includes("endosso"))
                    url = "/Endosso";
                else
                    url = "/Propostas";
            }

            setTimeout(function () {
                window.location.href = url;
            }, 1500);
        } else {
            notify.error("Atenção", r.message);
        }
    },
    verificarBoletoExistente: function (propostaId) {
        $.ajax({
            url: "/PropostasStatus/VerificarBoletoExistente",
            type: "GET",
            data: { propostaId: propostaId },
            success: function (res) {
                if (res && res.existe) {
                    $("#divBoletoApolice").hide();
                }
            },
            error: function () { }
        });
    },

    verificarBoletoExistenteSync: function () {
        var existe = false;
        $.ajax({
            url: "/PropostasStatus/VerificarBoletoExistente",
            type: "GET",
            data: { propostaId: propostaStatusJs.propostaId },
            async: false,
            success: function (res) { existe = res && res.existe; }
        });
        return existe;
    },

    openStatus: function (id) {
        propostaStatusJs.propostaId = id;
        gl.RenderGet("PropostasStatusCadastrar", "Propostas", { propostaId: propostaStatusJs.propostaId }, "#modalStatus", true, null, function () {
            propostaStatusJs.initStatusModal();
            propostaStatusJs.verificarBoletoExistente(propostaStatusJs.propostaId);
            $("#modalStatus").fadeIn(300);
        });
    },

    // Função para controlar a exibição dos campos com base no status selecionado
    showHideFieldsByStatus: function (selectedStatus) {
        // Primeiro esconde todas as seções específicas
        $(".status-section").hide();

        // Identifica e mostra a seção correta com base no status selecionado
        if (["Cotação encerrada sem sucesso", "Proposta cancelada", "Proposta recusada", "Apólice cancelada"].includes(selectedStatus)) {
            // Status que precisam de justificativa
            $("#secaoJustificativa").show();
        }
        else if (selectedStatus === "Proposta com pendência" || selectedStatus === "Endosso com pendência") {
            // Status com campos de pendência
            $("#secaoPendencia").show();
        }
        else if (selectedStatus === "Proposta aceita" || selectedStatus === "Proposta aceita → Devolutiva da Seguradora") {
            // Status de proposta aceita com campos específicos
            $("#secaoPropostaAceita").show();
        }
        else if (selectedStatus === "Apólice emitida") {
            // Status de apólice emitida
            $("#secaoApoliceEmitida").show();
            // Verificar se já existe boleto para ocultar campos
            try { propostaStatusJs.verificarBoletoExistente(propostaStatusJs.propostaId); } catch (e) { console.warn("verificarBoletoExistente error:", e); }
        }
        else if (selectedStatus === "Solicitar Endosso") {
            // Status de solicitar endosso
            $("#secaoEndossoSolicitacao").show();
        }
        else if (selectedStatus === "Endosso com pendência") {
            // Status de endosso com pendência
            $("#secaoEndossoPendencia").show();
        }
        else if (selectedStatus === "Endosso Transmitido") {
            // Status de endosso transmitido
            $("#secaoEndossoTransmitido").show();
        }
        else if (selectedStatus === "Endosso Emitido") {
            // Status de endosso emitido
            $("#secaoEndossoEmitido").show();
        }
        else if (selectedStatus === "Comunicar Sinistro") {
            $("#secaoSinistroComunicar").show();
            // Configurar validação de datas e campos de responsável
            propostaStatusJs.configurarSinistroComunicar();
        }
        else if (selectedStatus === "Sinistro Com Pendência") {
            $("#secaoSinistroPendencia").show();
        }
        else if (selectedStatus === "Sinistro Cancelado") {
            $("#secaoSinistroCancelado").show();
        }
        else if (selectedStatus === "Sinistro Aberto/Em Regulação") {
            $("#secaoSinistroAbertoRegulacao").show();
        }
        else if (selectedStatus === "Sinistro Aguardando Pagamento") {
            $("#secaoSinistroAguardPagamento").show();
        }
        else if (selectedStatus === "Sinistro deferido Pago") {
            $("#secaoSinistroDeferidoPago").show();
        }
        else if (selectedStatus === "Sinistro indeferido") {
            $("#secaoSinistroIndeferido").show();
        }
        // Os outros status não precisam de campos adicionais, só ficam com o status e observação
    },

    initStatusModal: function () {
        $(".btnFecharStatusModal").off("click").on("click", function () {
            $("#modalStatus").fadeOut(300);
        });

        // Inicializa o Select2 somente se disponível
        if (typeof $.fn.select2 !== 'undefined') {
            $('.select2').select2({
                width: '100%',
                dropdownAutoWidth: true
            }).on('select2:select', function (e) {
                var selectedText = e.params && e.params.data ? e.params.data.text : $(this).find('option:selected').text();
                $(this).siblings('.select2-container').find('.select2-selection__rendered').text(selectedText);

                propostaStatusJs.preencherAcaoAutomaticamente(selectedText);
                propostaStatusJs.showHideFieldsByStatus(selectedText);
            });
            // Aplicar status atual ao carregar
            var currentStatus = $("#Status").val();
            if (currentStatus) {
                var currentText = $("#Status option:selected").text();
                propostaStatusJs.preencherAcaoAutomaticamente(currentText);
                propostaStatusJs.showHideFieldsByStatus(currentText);
            }
        } else {
            console.warn("[Status] select2 não carregado, usando change event nativo");
            $('.select2').on('change', function () {
                var selectedText = $(this).find('option:selected').text();
                propostaStatusJs.preencherAcaoAutomaticamente(selectedText);
                propostaStatusJs.showHideFieldsByStatus(selectedText);
            });
        }

        // Verifica o status atual ao carregar o modal (para o caso de edição)
        var currentStatus = $("#Status").val();
        if (currentStatus) {
            // Preenche automaticamente o campo Ação com o valor do status
            propostaStatusJs.preencherAcaoAutomaticamente(currentStatus);
            propostaStatusJs.showHideFieldsByStatus(currentStatus);
        } else {
            // Se for uma nova seleção, esconde todas as seções específicas
            $(".status-section").hide();
        }

        // Valida o formulário antes de submeter
        $("#formStatus").off('submit').on('submit', function (e) {
            e.preventDefault();
            var selectedStatus = $("#Status").val();

            // Validações específicas por tipo de status
            if (["Cotação encerrada sem sucesso", "Proposta cancelada", "Proposta recusada", "Apólice cancelada"].includes(selectedStatus)) {
                if (!$("#JustificativaDTO_Justificativa").val()) {
                    e.preventDefault();
                    alert("A justificativa é obrigatória para este status.");
                    return false;
                }
            }
            else if (selectedStatus === "Proposta com pendência" || selectedStatus === "Endosso com pendência") {
                if (!$("#PendenciaDTO_RetornoPendencia").val() || !$("#PendenciaDTO_TipoDocumento").val()) {
                    e.preventDefault();
                    alert("O retorno da pendência e o tipo de documento são obrigatórios.");
                    return false;
                }
            }
            else if (selectedStatus === "Proposta aceita" || selectedStatus === "Proposta aceita → Devolutiva da Seguradora") {
                // Validar campos obrigatórios (exceto boleto e data de vencimento)
                var camposObrigatorios = [];
                
                if (!$("#AceitaDTO_NumeroProposta").val()) {
                    camposObrigatorios.push("Número da proposta");
                }
                if (!$("#AceitaDTO_ParcelaSegurado").val()) {
                    camposObrigatorios.push("Parcela do segurado");
                }
                if (!$("#AceitaDTO_LmiTotal").val()) {
                    camposObrigatorios.push("LMI Total");
                }
                if (!$("#AceitaDTO_PremioTotal").val()) {
                    camposObrigatorios.push("Prêmio Total");
                }
                if (!$("#AceitaDTO_SubFederal").val()) {
                    camposObrigatorios.push("Sub Federal");
                }
                if (!$("#AceitaDTO_SubEstadual").val()) {
                    camposObrigatorios.push("Sub Estadual");
                }
                if (!$("#AceitaDTO_PropostaSeguradora").val()) {
                    camposObrigatorios.push("Proposta da Seguradora");
                }
                
                if (camposObrigatorios.length > 0) {
                    e.preventDefault();
                    alert("Os seguintes campos são obrigatórios:\n• " + camposObrigatorios.join("\n• "));
                    return false;
                }
            }
            else if (selectedStatus === "Apólice emitida") {
                if (!$("#ApoliceEmitidaDTO_NumeroApolice").val() || !$("#ApoliceEmitidaDTO_InicioVigencia").val() || !$("#ApoliceEmitidaDTO_FinalVigencia").val()) {
                    e.preventDefault();
                    alert("Número da apólice e datas de vigência são obrigatórios.");
                    return false;
                }
                
                // Verificar se já existe boleto antes de validar data de vencimento
                var boletoExistente = propostaStatusJs.verificarBoletoExistenteSync();
                if (!boletoExistente && !$("#ApoliceEmitidaDTO_DataVencimento").val()) {
                    e.preventDefault();
                    alert("Data de vencimento é obrigatória quando não há boleto cadastrado anteriormente.");
                    return false;
                }
            }
            else if (selectedStatus === "Endosso com pendência") {
                if (!$("#EndossoPendenciaDTO_DescricaoPendencia").val() || 
                    !$("#EndossoPendenciaDTO_TipoDocumento").val()) {
                    e.preventDefault();
                    alert("Descrição da pendência e tipo de documento são obrigatórios.");
                    return false;
                }
            }
            else if (selectedStatus === "Endosso Emitido") {
                if (!$("#EndossoEmitidoDTO_NumeroEndosso").val()) {
                    e.preventDefault();
                    alert("Número do endosso é obrigatório.");
                    return false;
                }
            }
            else if (selectedStatus === "Comunicar Sinistro") {
                if (!$("#SinistroComunicarDTO_Cobertura").val() || !$("#SinistroComunicarDTO_Evento").val()) {
                    e.preventDefault();
                    alert("Cobertura e evento são obrigatórios.");
                    return false;
                }
                
                // Validar datas no range da apólice
                var dataInicio = $("#SinistroComunicarDTO_DataInicio").val();
                var dataFinal = $("#SinistroComunicarDTO_DataFinal").val();
                var apoliceInicio = $("#apoliceInicioVigencia").val();
                var apoliceFinal = $("#apoliceFinalVigencia").val();
                
                if (dataInicio && apoliceInicio && new Date(dataInicio) < new Date(apoliceInicio)) {
                    e.preventDefault();
                    alert("Data de início deve estar dentro do período de vigência da apólice.");
                    return false;
                }
                
                if (dataFinal && apoliceFinal && new Date(dataFinal) > new Date(apoliceFinal)) {
                    e.preventDefault();
                    alert("Data final deve estar dentro do período de vigência da apólice.");
                    return false;
                }
                
                if (dataInicio && dataFinal && new Date(dataInicio) > new Date(dataFinal)) {
                    e.preventDefault();
                    alert("Data de início deve ser anterior à data final.");
                    return false;
                }
                
                // Validar tipo e campos de responsável
                var tipoResp = $("#SinistroComunicarDTO_TipoRespVistoria").val();
                if (!tipoResp) {
                    e.preventDefault();
                    alert("Tipo do responsável pela vistoria é obrigatório.");
                    return false;
                }
                
                if (tipoResp === "Segurado" || tipoResp === "Grupo Familiar") {
                    if (!$("#SinistroComunicarDTO_ResponsavelSelecionado").val()) {
                        e.preventDefault();
                        alert("Selecione o responsável pela vistoria.");
                        return false;
                    }
                } else if (tipoResp === "Outro") {
                    if (!$("#SinistroComunicarDTO_NomeRespVistoria").val() || !$("#SinistroComunicarDTO_CPFRespVistoria").val()) {
                        e.preventDefault();
                        alert("Nome e CPF do responsável pela vistoria são obrigatórios.");
                        return false;
                    }
                }
                
                // Validar área total afetada não pode ser maior que a área da apólice
                var areaTotalAfetada = parseFloat($("#SinistroComunicarDTO_AreaTotalAfetada").val());
                var propostaAreaTotal = parseFloat($("#propostaAreaTotal").val());
                if (areaTotalAfetada && propostaAreaTotal && areaTotalAfetada > propostaAreaTotal) {
                    e.preventDefault();
                    alert("A área total afetada não pode ser maior que a área da apólice (" + propostaAreaTotal.toFixed(2) + " ha).");
                    return false;
                }
            }
            else if (selectedStatus === "Sinistro Com Pendência") {
                if (!$("#SinistroPendenciaDTO_RetornoPendencia").val() || !$("#SinistroPendenciaDTO_TipoDocumento").val()) {
                    e.preventDefault();
                    alert("Retorno da pendência e tipo de documento são obrigatórios.");
                    return false;
                }
            }
            else if (selectedStatus === "Sinistro Cancelado") {
                if (!$("#SinistroCanceladoDTO_MotivoCancelamento").val()) {
                    e.preventDefault();
                    alert("Motivo do cancelamento é obrigatório.");
                    return false;
                }
            }

            // Normalizar parcela antes de enviar
            propostaStatusJs.normalizarParcelaSeguradoInvariant();

            // Enviar via FormData + fetch
            var form = document.getElementById("formStatus");
            var formData = new FormData(form);
            var formAction = form.getAttribute("action") || "/PropostasStatus/SalvarStatus";

            gl.BeginAjax.call(form);
            fetch(formAction, { method: "POST", body: formData })
                .then(function (response) { return response.json(); })
                .then(function (data) {
                    gl.CompleteAjax.call(form);
                    propostaStatusJs.onSuccessCallbackStatus(data);
                })
                .catch(function (err) {
                    gl.CompleteAjax.call(form);
                    gl.onFailed({ status: 0, responseText: err.message || "Erro ao salvar status" });
                });
        });

        var formStatusEl = document.getElementById("formStatus");
        if (formStatusEl && !formStatusEl.__parcelaNormalizeCapture) {
            formStatusEl.__parcelaNormalizeCapture = true;
            formStatusEl.addEventListener(
                "submit",
                function () {
                    propostaStatusJs.normalizarParcelaSeguradoInvariant();
                },
                true
            );
        }

        $("#UplPendencia").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='PendenciaDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='PendenciaDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);

            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });
        // Máscara monetária para Parcela do Segurado
        Basics.currencyMask("#AceitaDTO_ParcelaSegurado");

        $("#UplPropostaAceita").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: "image/*,.pdf,application/pdf",
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='AceitaDTO.PropostaSeguradora']").val(res.data);
                    $("#formStatus input[name='AceitaDTO.PropostaSeguradoraNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);

            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });
        $("#UplPropostaAceitaBoleto").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: "image/*,.pdf,application/pdf",
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='AceitaDTO.Boleto']").val(res.data);
                    $("#formStatus input[name='AceitaDTO.BoletoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);

            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });
        $("#UplApoliceEmitida").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: "image/*,.pdf,application/pdf",
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='ApoliceEmitidaDTO.ApoliceEmitida']").val(res.data);
                    $("#formStatus input[name='ApoliceEmitidaDTO.ApoliceEmitidaNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);

            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });
        $("#UplApoliceEmitidaBoleto").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: "image/*,.pdf,application/pdf",
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='ApoliceEmitidaDTO.Boleto']").val(res.data);
                    $("#formStatus input[name='ApoliceEmitidaDTO.BoletoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);

            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });
        
        // Upload de arquivo para Endosso Solicitação
        $("#UplEndossoSolicitacao").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='EndossoSolicitacaoDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='EndossoSolicitacaoDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Upload de arquivo para Endosso Pendência
        $("#UplEndossoPendencia").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='EndossoPendenciaDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='EndossoPendenciaDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Upload de arquivo para Endosso Transmitido
        $("#UplEndossoTransmitido").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='EndossoTransmitidoDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='EndossoTransmitidoDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Upload de arquivo para Endosso Emitido
        $("#UplEndossoEmitido").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='EndossoEmitidoDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='EndossoEmitidoDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Os campos de ação agora são hidden e preenchidos automaticamente

        // Evento para atualizar eventos quando cobertura de Comunicar Sinistro for alterada
        $("#SinistroComunicarDTO_Cobertura").on('change', function () {
            propostaStatusJs.atualizarEventosPorCobertura();
        });

        // Upload de arquivo para Sinistro Pendência
        $("#UplSinistroPendencia").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='SinistroPendenciaDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='SinistroPendenciaDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Upload de arquivo para Sinistro Aberto/Em Regulação
        $("#UplSinistroAbertoRegulacao").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='SinistroAbertoRegulacaoDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='SinistroAbertoRegulacaoDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Máscara monetária para Valor da Indenização
        Basics.currencyMask("#SinistroAguardPagamentoDTO_ValorIndenizacao");

        // Upload de arquivo para Sinistro Aguardando Pagamento
        $("#UplSinistroAguardPagamento").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: "image/*,.pdf,application/pdf",
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='SinistroAguardPagamentoDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='SinistroAguardPagamentoDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });

        // Upload de arquivo para Sinistro Indeferido
        $("#UplSinistroIndeferido").customDropzone({
            url: "/File/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) { },
            onQueueComplete: function (progress, res) { },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formStatus input[name='SinistroIndeferidoDTO.UploadArquivo']").val(res.data);
                    $("#formStatus input[name='SinistroIndeferidoDTO.UploadArquivoNome']").val(res.nome);
                } else
                    notify.error("Erro no Upload", res.message);
            },
            onError: function (file, errorMessage) {
                notify.error("Erro no Upload", errorMessage);
            }
        });
    },

    // Função para verificar se já existe boleto (assíncrona)
    verificarBoletoExistente: function(propostaId) {
        $.ajax({
            url: '/PropostasDocumentos/Listar',
            data: { propostaId: propostaId },
            success: function(response) {
                if (response.success) {
                    var temBoleto = response.data.some(function(doc) {
                        return doc.tipoDocumento === "Boleto";
                    });
                    
                    if (temBoleto) {
                        // Ocultar campos de upload de boleto no status "Apólice Emitida"
                        $("#divBoletoApolice").hide();
                        $("input[name='ApoliceEmitidaDTO.DataVencimento']").closest('.col-md-6').hide();
                        
                        if ($("#msgBoletoExistente").length === 0) {
                            $("#secaoApoliceEmitida").prepend(
                            );
                        }
                    } else {
                        // Mostrar campos se não há boleto
                        $("#divBoletoApolice").show();
                        $("input[name='ApoliceEmitidaDTO.DataVencimento']").closest('.col-md-6').show();
                        $("#msgBoletoExistente").remove();
                    }
                }
            },
            error: function() {
                // Em caso de erro, mostrar os campos por padrão
                $("#divBoletoApolice").show();
                $("input[name='ApoliceEmitidaDTO.DataVencimento']").closest('.col-md-6').show();
                $("#msgBoletoExistente").remove();
            }
        });
    },

    // Função para verificar se já existe boleto (síncrona - para validação)
    verificarBoletoExistenteSync: function() {
        var temBoleto = false;
        $.ajax({
            url: '/PropostasDocumentos/Listar',
            data: { propostaId: propostaStatusJs.propostaId },
            async: false,
            success: function(response) {
                if (response.success) {
                    temBoleto = response.data.some(function(doc) {
                        return doc.tipoDocumento === "Boleto";
                    });
                }
            }
        });
        return temBoleto;
    },


    // Função para preencher automaticamente o campo Ação com o valor do status
    preencherAcaoAutomaticamente: function(selectedStatus) {
        // Endossos
        if (selectedStatus === "Solicitar Endosso") {
            $("#EndossoSolicitacaoDTO_Acao").val("Solicitar Endosso");
        }
        else if (selectedStatus === "Endosso com pendência") {
            $("#EndossoPendenciaDTO_Acao").val("Endosso com pendência");
        }
        else if (selectedStatus === "Endosso Transmitido") {
            $("#EndossoTransmitidoDTO_Acao").val("Endosso Transmitido");
        }
        else if (selectedStatus === "Endosso Emitido") {
            $("#EndossoEmitidoDTO_Acao").val("Endosso Emitido");
        }
        // Sinistros
        else if (selectedStatus === "Comunicar Sinistro") {
            $("#SinistroComunicarDTO_Acao").val("Comunicar Sinistro");
        }
        else if (selectedStatus === "Sinistro Com Pendência") {
            $("#SinistroPendenciaDTO_Acao").val("Sinistro Com Pendência");
        }
        else if (selectedStatus === "Sinistro Cancelado") {
            $("#SinistroCanceladoDTO_Acao").val("Sinistro Cancelado");
        }
        else if (selectedStatus === "Sinistro Aberto/Em Regulação") {
            $("#SinistroAbertoRegulacaoDTO_Acao").val("Sinistro Aberto/Em Regulação");
        }
        else if (selectedStatus === "Sinistro Aguardando Pagamento") {
            $("#SinistroAguardPagamentoDTO_Acao").val("Sinistro Aguardando Pagamento");
        }
        else if (selectedStatus === "Sinistro deferido Pago") {
            $("#SinistroDeferidoPagoDTO_Acao").val("Sinistro deferido Pago");
        }
        else if (selectedStatus === "Sinistro indeferido") {
            $("#SinistroIndeferidoDTO_Acao").val("Sinistro indeferido");
        }
    },

    // Função para configurar campos e validações do formulário de Comunicar Sinistro
    configurarSinistroComunicar: function() {
        // Obter datas da apólice se existir
        var apoliceInicio = $("#apoliceInicioVigencia").val();
        var apoliceFinal = $("#apoliceFinalVigencia").val();
        
        if (apoliceInicio && apoliceFinal) {
            // Configurar min e max nas datas
            $("#SinistroComunicarDTO_DataInicio").attr("min", apoliceInicio).attr("max", apoliceFinal);
            $("#SinistroComunicarDTO_DataFinal").attr("min", apoliceInicio).attr("max", apoliceFinal);
        }
        
        // Validação em tempo real da área total afetada
        var propostaAreaTotal = parseFloat($("#propostaAreaTotal").val());
        if (propostaAreaTotal) {
            $("#SinistroComunicarDTO_AreaTotalAfetada").off('input blur').on('input blur', function() {
                var areaTotalAfetada = parseFloat($(this).val());
                var validationSpan = $(this).siblings('.text-danger').first();
                
                if (areaTotalAfetada && areaTotalAfetada > propostaAreaTotal) {
                    if (validationSpan.length === 0 || !validationSpan.hasClass('area-validation')) {
                        $(this).after('<span class="text-danger area-validation">A área total afetada não pode ser maior que a área da apólice (' + propostaAreaTotal.toFixed(2) + ' ha).</span>');
                    }
                    $(this).addClass('is-invalid');
                } else {
                    $(this).siblings('.area-validation').remove();
                    $(this).removeClass('is-invalid');
                }
            });
        }
        
        // Evento para mudança do tipo de responsável
        $("#SinistroComunicarDTO_TipoRespVistoria").off('change').on('change', function() {
            var tipo = $(this).val();
            
            // Limpar campos
            $("#SinistroComunicarDTO_NomeRespVistoria").val("");
            $("#SinistroComunicarDTO_CPFRespVistoria").val("");
            $("#SinistroComunicarDTO_ResponsavelSelecionado").val("");
            
            if (tipo === "Segurado" || tipo === "Grupo Familiar") {
                $("#divSeguradoGrupoFamiliar").show();
                $("#divOutro").hide();
            } else if (tipo === "Outro") {
                $("#divSeguradoGrupoFamiliar").hide();
                $("#divOutro").show();
            } else {
                $("#divSeguradoGrupoFamiliar").hide();
                $("#divOutro").hide();
            }
        });
        
        // Evento para seleção do responsável (Segurado/Grupo Familiar)
        $("#SinistroComunicarDTO_ResponsavelSelecionado").off('change').on('change', function() {
            var option = $(this).find("option:selected");
            if (option.val()) {
                $("#SinistroComunicarDTO_NomeRespVistoria").val(option.data("nome"));
                $("#SinistroComunicarDTO_CPFRespVistoria").val(option.data("cpf"));
            }
        });
        
        // Verificar se já tem valor selecionado ao carregar
        var tipoAtual = $("#SinistroComunicarDTO_TipoRespVistoria").val();
        if (tipoAtual) {
            $("#SinistroComunicarDTO_TipoRespVistoria").trigger('change');
        }
    },

    // Função para atualizar eventos baseado na cobertura selecionada
    atualizarEventosPorCobertura: function() {
        var cobertura = $("#SinistroComunicarDTO_Cobertura").val();
        var eventoSelect = $("#SinistroComunicarDTO_Evento");
        
        // Limpa as opções atuais
        eventoSelect.empty();
        
        if (!cobertura) {
            eventoSelect.append('<option value="">Selecione a Cobertura primeiro...</option>');
            return;
        }
        
        eventoSelect.append('<option value="">Selecione...</option>');
        
        // Eventos baseados na cobertura
        var eventos = [];
        
        if (cobertura === "Produção") {
            eventos = [
                "Chuvas Excessivas",
                "Geada",
                "Granizo",
                "Inundação",
                "Incêndio e/ou Raio",
                "Tromba d'água",
                "Variação Excessiva de Temperatura",
                "Ventos Fortes",
                "Ventos Frios",
                "Seca/Estiagem"
            ];
        } else if (cobertura === "Replantio") {
            eventos = [
                "Chuvas Excessivas",
                "Geada",
                "Granizo",
                "Incêndio",
                "Raio"
            ];
        }
        
        // Adiciona as opções
        eventos.forEach(function(evento) {
            eventoSelect.append('<option value="' + evento + '">' + evento + '</option>');
        });
    }
}

