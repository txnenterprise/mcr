var endossoIndexJs = {
    propostaId: null,
    init: function () {
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
            endossoIndexJs.abrirModalOcorrencia(proposta);
        });

        // Clique na coluna Status para abrir proposta na aba Status
        $(document).on("click", ".status-clickable-grid", function () {
            var propostaId = $(this).data("proposta-id");
            if (propostaId) {
                window.location.href = "/Propostas/Cadastrar/" + propostaId;
            }
        });
    },

    navegarPara: function (pagina) {
        $('#Pagina').val(pagina);
        $('#formNavigator').submit();
    },

    abrirModalOcorrencia: function (propostaId) {
        // Usar o novo modal customizado
        gl.RenderGet("PropostasOcorrenciasRegistrar", "Endosso", { propostaId: propostaId }, "#modalOcorrencia", true, null, function () {
            propostaJS.closeModalOcorrencia();
            $("#modalOcorrencia").fadeIn(300);
        });
    },
}

$(document).ready(function () {
    endossoIndexJs.init();
});

