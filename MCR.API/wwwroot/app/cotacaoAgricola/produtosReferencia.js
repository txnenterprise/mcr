var produtosReferencia = produtosReferencia || {};

produtosReferencia.carregar = function (culturaId, safraId, canalId, paId, estado, municipio, areaTotal) {
    if (!culturaId || !safraId) {
        $('#container-produtos-referencia').hide();
        return;
    }
    $.get('/CotacaoAgricola/ProdutosDisponiveis', {
        culturaId: culturaId,
        safraId: safraId,
        canalId: canalId || null,
        pontoAtendimentoId: paId || null,
        estado: estado || null,
        municipio: municipio || null,
        areaTotal: areaTotal || 0
    }, function (data) {
        var $tbody = $('#tbodyProdutosRef');
        $tbody.empty();
        if (!data || data.length === 0) {
            $('#container-produtos-referencia').hide();
            return;
        }
        data.forEach(function (p) {
            var preco = p.modalidade === 'Produtividade'
                ? (p.valorSacaMinimo + ' — ' + p.valorSacaMaximo)
                : (p.valorCusteioMinimo + ' — ' + p.valorCusteioMaximo);
            var vinculo = p.vinculadoCanalPA
                ? '<i class="bi bi-check-circle-fill text-success"></i>'
                : '<i class="bi bi-x-circle-fill text-danger"></i>';
            var areaOk = p.areaAtende
                ? '<i class="bi bi-check-circle-fill text-success"></i>'
                : '<i class="bi bi-x-circle-fill text-danger" title="' + p.areaMinima + ' ha mín."></i>';
            var taxaMunicipio = p.taxaMunicipio || '<span class="text-muted">—</span>';
            var nc65 = p.taxaNc65 > 0 ? p.taxaNc65 + '%' : '<span class="text-muted">—</span>';
            var nc70 = p.taxaNc70 > 0 ? p.taxaNc70 + '%' : '<span class="text-muted">—</span>';
            var nc75 = p.taxaNc75 > 0 ? p.taxaNc75 + '%' : '<span class="text-muted">—</span>';
            var tr = '<tr>' +
                '<td><strong>' + p.nomeProduto + '</strong></td>' +
                '<td>' + p.modalidade + '</td>' +
                '<td>' + (p.tipoSolo || '—') + '</td>' +
                '<td>' + (p.classificacaoSolo || '—') + '</td>' +
                '<td>' + preco + '</td>' +
                '<td>' + p.areaMinima + ' ha</td>' +
                '<td class="text-center">' + vinculo + '</td>' +
                '<td class="text-center">' + areaOk + '</td>' +
                '<td>' + taxaMunicipio + '</td>' +
                '<td>' + nc65 + '</td>' +
                '<td>' + nc70 + '</td>' +
                '<td>' + nc75 + '</td>' +
                '</tr>';
            $tbody.append(tr);
        });
        $('#thPreco').text(data[0].modalidade === 'Produtividade' ? 'Preço Saca' : 'Custeio/ha');
        $('#container-produtos-referencia').show();
    }).fail(function () {
        $('#container-produtos-referencia').hide();
    });
};

produtosReferencia.bindAutoRefresh = function (formId) {
    var selector = '#' + formId + ' #CulturaId, #' + formId + ' #SafraId, #' + formId + ' #CanalId, #' + formId + ' #PontoAtendimentoId, #' + formId + ' #Estado, #' + formId + ' #Municipio, #' + formId + ' #AreaTotal';
    $(selector).on('change', function () {
        produtosReferencia.atualizar(formId);
    });
};

produtosReferencia.atualizar = function (formId) {
    var prefix = '#' + formId + ' ';
    var culturaId = $(prefix + '#CulturaId').val();
    var safraId = $(prefix + '#SafraId').val();
    var canalId = $(prefix + '#CanalId').val();
    var paId = $(prefix + '#PontoAtendimentoId').val();
    var estado = $(prefix + '#Estado').val();
    var municipio = $(prefix + '#Municipio').val();
    var areaTotal = parseFloat($(prefix + '#AreaTotal').val()) || 0;
    produtosReferencia.carregar(culturaId, safraId, canalId, paId, estado, municipio, areaTotal);
};

$(document).ready(function () {
    $('#filtroProdutos').on('keyup', function () {
        var termo = $(this).val().toLowerCase();
        $('#tbodyProdutosRef tr').each(function () {
            var texto = $(this).text().toLowerCase();
            $(this).toggle(texto.indexOf(termo) > -1);
        });
    });
});
