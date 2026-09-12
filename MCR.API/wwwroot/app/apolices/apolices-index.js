var apoliceIndexJs = {
    propostaId: null,
    init: function () {
        $('#btnConfirmarCancelamento').off("click").on("click", function () {
            gl.Post("Cancelar", "Apolices", { id: apoliceIndexJs.propostaId }, true, function (result) {
                $('#modalConfirmacao').modal('hide');
                window.location.reload();
            });
        });

        $('#btnLimparFiltros').off("click").on("click", function () {
            $('#PontoAtendimentoId').val('');
            $('#NomeProdutor').val('');
            $('#CPF').val('');
            $('form').submit();
        });

        // Máscara para CPF
        $('#CPF').mask('000.000.000-00', { reverse: true });

        // Botão para registrar ocorrência
        $(".btRegistrarOcorrencia").off("click").on("click", function () {
            var proposta = $(this).data("proposta");
            apoliceIndexJs.abrirModalOcorrencia(proposta);
        });

        // Clique na coluna Status para abrir proposta na aba Status
        $(document).on("click", ".status-clickable-grid", function () {
            var propostaId = $(this).data("proposta-id");
            if (propostaId) {
                window.location.href = "/Propostas/Cadastrar/" + propostaId;
            }
        });
    },

    confirmarCancelamento: function (id) {
        apoliceIndexJs.propostaId = id;
        $('#modalConfirmacao').modal('show');
    },

    navegarPara: function (pagina) {
        $('#Pagina').val(pagina);
        $('#formNavigator').submit();
    },

    abrirModalOcorrencia: function (propostaId) {
        // Usar o novo modal customizado
        gl.RenderGet("PropostasOcorrenciasRegistrar", "Apolices", { propostaId: propostaId }, "#modalOcorrencia", true, null, function () {
            propostaJS.closeModalOcorrencia();
            $("#modalOcorrencia").fadeIn(300);
        });
    },
}

$(document).ready(function () {
    apoliceIndexJs.init();
});

