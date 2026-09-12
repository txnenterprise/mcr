const propostasDocumentosJS  = {
    init: function () {
        this.initFormUpload();
        this.initDeleteButtons();
    },

    initFormUpload: function () {
        $("#UploadArquivo").customDropzone({
            url: "/PropostasDocumentos/UploadFile",
            maxFiles: 1,
            maxFilesize: 50,
            acceptedFiles: "image/*,.pdf,application/pdf",
            addRemoveLinks: true,
            autoQueue: true,
            onSending: function (file) {
                console.log("Enviando arquivo: " + file.name);
            },
            onQueueComplete: function (progress, res) {
                console.log("Upload completo!");
            },
            onSuccess: function (file, res) {
                if (res.success) {
                    $("#formUploadDocumento input[name=ArquivoBase64]").val(res.data);
                    $("#formUploadDocumento input[name=ArquivoNome]").val(res.nome);
                    $("#formUploadDocumento input[name=ArquivoPath]").val(res.path);

                } else {
                    console.error("Erro no upload: " + res.message);
                    notify.error("Erro no Upload", res.message);
                }
            },
            onError: function (file, errorMessage) {
                console.error("Erro no upload do arquivo " + file.name + ": " + errorMessage);
                notify.error("Erro no Upload", errorMessage);
            }
        });
    },
    onSuccessCallbackDocumento: function (r) {

        if (r.success == true) {
            // Capturar o tipo de documento ANTES de limpar o formulário
            var tipoDocumento = $("#formUploadDocumento select[name=TipoDocumento]").val();
            
            $("#formUploadDocumento input[name=ArquivoBase64]").val("");
            $("#formUploadDocumento input[name=ArquivoNome]").val("");
            $("#formUploadDocumento input[name=ArquivoPath]").val("");
            $("#formUploadDocumento select[name=TipoDocumento]").val("");
            propostasDocumentosJS.refreshDocumentos();
            
            // Atualizar timeline se foi upload de boleto
            if (tipoDocumento === "Boleto") {
                setTimeout(function() {
                    if (typeof propostaJS !== 'undefined' && propostaJS.refreshTimeline) {
                        propostaJS.refreshTimeline();
                    }
                }, 1000);
            }
            
            // Atualizar status das abas após adicionar documento
            setTimeout(function() {
                if (typeof propostaJS !== 'undefined' && propostaJS.atualizarStatusAbas) {
                    propostaJS.atualizarStatusAbas();
                }
            }, 500);
            notify.success("Documento", r.message, null);
        } else
            notify.error("Atenção", r.message);
    },
    refreshDocumentos: function () {
        gl.RenderGet("PropostasDocumentos", "Propostas", { propostaId: $("#PropostaId").val() }, "#documento_container", true, null, function () {
            propostasDocumentosJS.initDeleteButtons();
            // Atualizar status das abas após refresh dos documentos
            setTimeout(function() {
                if (typeof propostaJS !== 'undefined' && propostaJS.atualizarStatusAbas) {
                    propostaJS.atualizarStatusAbas();
                }
            }, 300);
        });
    },
    initDeleteButtons: function () {
        $('.excluir-documento').off("click").on("click", function () {
            const documentoId = $(this).data("id");
            const nomeArquivo = $(this).data("nome");
            propostasDocumentosJS.confirmarRemoverDocumento(documentoId, nomeArquivo);
        });
    },
    confirmarRemoverDocumento: function (documentoId, nome) {
        notify.confirmYesNo(
            "danger",
            "Confirmação de Exclusão",
            "Tem certeza que deseja excluir este documento: " + nome + "?",
            function (confirmed) {
                if (confirmed) {
                    gl.Post("Excluir", "PropostasDocumentos", { documentoId: documentoId, __RequestVerificationToken: gl.GetToken() }, true, function (r) {
                        if (r.success) {
                            propostasDocumentosJS.refreshDocumentos();
                            // Atualizar status das abas após excluir documento
                            setTimeout(function() {
                                if (typeof propostaJS !== 'undefined' && propostaJS.atualizarStatusAbas) {
                                    propostaJS.atualizarStatusAbas();
                                }
                            }, 500);
                            notify.success("Documentos", r.message || "Documento excluído com sucesso!", null);
                        } else {
                            notify.error("Atenção", r.message || "Erro ao excluir documento.");
                        }
                    }, function (xhr, status, error) {
                        notify.error("Erro AJAX", "Falha ao tentar excluir o documento. Verifique o console.");
                    });
                }
            },
            null,
            "Sim, remover",
            "Cancelar"
        );
    }
};
 