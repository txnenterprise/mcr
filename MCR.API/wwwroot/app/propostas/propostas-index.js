var propostaIndexJs = {
    propostaId: null,
    init: function () {
        $('#btnConfirmarCancelamento').off("click").on("click", function () {
            gl.Post("Cancelar", "Propostas", { id: propostaIndexJs.propostaId }, true, function (result) {
                $('#modalConfirmacao').modal('hide');
                window.location.reload();
            });
        });

        $('#btnLimparFiltros').off("click").on("click", function () {
            $('#PontoAtendimentoId').val('');
            $('#SeguradoraId').val('');
            $('#NomeProdutor').val('');
            $('#CPF').val('');
            $('#Status').val('');
            $('form').submit();
        });

        // Máscara para CPF
        $('#CPF').mask('000.000.000-00', { reverse: true });

        // Botão para registrar ocorrência
        $(".btRegistrarOcorrencia").off("click").on("click", function () {
            var proposta = $(this).data("proposta");
            propostaIndexJs.abrirModalOcorrencia(proposta);
        });

        // Botão para alterar status
        $(".btStatus").off("click").on("click", function () {
            var propostaId = $(this).data("proposta");
            propostaStatusJs.openStatus(propostaId);
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
        propostaIndexJs.propostaId = id;
        $('#modalConfirmacao').modal('show');
    },

    navegarPara: function (pagina) {
        $('#Pagina').val(pagina);
        $('#formNavigator').submit();
    },

    abrirModalOcorrencia: function (propostaId) {
        // Usar o novo modal customizado
        gl.RenderGet("PropostasOcorrenciasRegistrar", "Propostas", { propostaId: propostaId }, "#modalOcorrencia", true, null, function () {
            propostaJS.closeModalOcorrencia();
            $("#modalOcorrencia").fadeIn(300);
        });
    },
}

$(document).ready(function () {
    propostaIndexJs.init();
});