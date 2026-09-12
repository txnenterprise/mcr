var userJs = {
    enList: [],
    init: function () {
        var utilizaOAuthEl = document.getElementById('UtilizaOAuth');
        if (utilizaOAuthEl) {
            utilizaOAuthEl.onchange = function () {
                document.getElementById('Password').disabled = this.checked;
                document.getElementById('ConfirmPassword').disabled = this.checked;
            };
        }

        Basics.maskCpf("#formCadastro input[name=Document]");
        Basics.maskPhone("#formCadastro input[name=PhoneNumber]");

        $('.selected2').select2({
            placeholder: 'Selecione...',
            allowClear: true,
            width: '100%'
        });

        $('#DefaultCorretoraId').select2().off('select2:select').on('select2:select', function () {
            var corretoraId = $(this).val();
            $("#DefaultCanalId").empty().append('<option value="">Selecione...</option>');
            $("#DefaultPontoAtendimentoId").empty().append('<option value="">Selecione...</option>');
            if (!corretoraId) return;
            gl.Get("ListarCanaisPorCorretora", "Canal", { corretoraId: corretoraId }, false, function (r) {
                if (r && r.length > 0) {
                    r.forEach(function (item) {
                        $("#DefaultCanalId").append('<option value="' + item.id + '">' + item.text + '</option>');
                    });
                    if (r.length === 1) {
                        $("#DefaultCanalId").val(r[0].id).trigger('change');
                    }
                } else {
                    $("#DefaultCanalId").append('<option value="">Nenhum canal encontrado</option>');
                }
            });
        });

        $('#DefaultCanalId').select2().off('select2:select').on('select2:select', function (e) {
            const canalId = $(this).val();
            $("#DefaultPontoAtendimentoId").empty();

            if (!canalId) {
                $("#DefaultPontoAtendimentoId").append('<option value="">Selecione...</option>');
                return;
            }

            gl.Get("ListarPontosAtendimentoPorCanal", "PontoAtendimento", { canalId: canalId }, false, function (r) {
                if (r && r.length > 0) {
                    $("#DefaultPontoAtendimentoId").append('<option value="">Selecione...</option>');
                    r.forEach(item => {
                        $("#DefaultPontoAtendimentoId").append(`<option value="${item.id}">${item.text}</option>`);
                    });
                    if (r.length === 1) {
                        $("#DefaultPontoAtendimentoId").val(r[0].id).trigger('change');
                    }
                } else {
                    $("#DefaultPontoAtendimentoId").append('<option value="">Nenhum ponto de atendimento encontrado</option>');
                }
            });
        });

        $("#btnAddEN").off("click").on("click", function () {
            // Busca os valores dos elementos selecionados
            var corretoraId = $("#DefaultCorretoraId").val();
            var canalId = $("#DefaultCanalId").val();
            var pontoAtendimentoId = $("#DefaultPontoAtendimentoId").val();

            // Busca os textos selecionados
            var corretoraText = $("#DefaultCorretoraId option:selected").text();
            var canalText = $("#DefaultCanalId option:selected").text();
            var pontoAtendimentoText = $("#DefaultPontoAtendimentoId option:selected").text();

            // Verifica se os valores estão vazios para aplicar o '*'
            if (!corretoraId) corretoraText = "*";
            if (!canalId) canalText = "*";
            if (!pontoAtendimentoId) pontoAtendimentoText = "*";

            // Adiciona o novo item à lista
            userJs.enList.push({
                corretoraId: corretoraId,
                canalId: canalId,
                pontoAtendimentoId: pontoAtendimentoId,
                corretoraText: corretoraText,
                canalText: canalText,
                pontoAtendimentoText: pontoAtendimentoText
            });

            // Verifica se os selects possuem apenas 1 registro com value
            // Se tiver mais de um, limpa a seleção para forçar o usuário a selecionar novamente

            // Verifica o select de Corretora
            if ($("#DefaultCorretoraId option").length > 2) { // Mais de 2 porque conta a opção "Selecione..."
                $("#DefaultCorretoraId").val("").trigger('change');
            } else if ($("#DefaultCorretoraId option").length == 2) {
                // Se tiver apenas 1 opção além do "Selecione...", seleciona automaticamente
                var singleOption = $("#DefaultCorretoraId option:not([value=''])").val();
                $("#DefaultCorretoraId").val(singleOption).trigger('change');
            }

            // Verifica o select de Canal
            if ($("#DefaultCanalId option").length > 2) {
                $("#DefaultCanalId").val("").trigger('change');
            } else if ($("#DefaultCanalId option").length == 2) {
                var singleOption = $("#DefaultCanalId option:not([value=''])").val();
                $("#DefaultCanalId").val(singleOption).trigger('change');
            }

            // Verifica o select de Ponto de Atendimento
            if ($("#DefaultPontoAtendimentoId option").length > 2) {
                $("#DefaultPontoAtendimentoId").val("").trigger('change');
            } else if ($("#DefaultPontoAtendimentoId option").length == 2) {
                var singleOption = $("#DefaultPontoAtendimentoId option:not([value=''])").val();
                $("#DefaultPontoAtendimentoId").val(singleOption).trigger('change');
            }

            // Atualiza a tabela
            userJs.reloadENTable();
        });

        // Inicializa a tabela
        userJs.reloadENTable();
    },
    OnSuccessCallback: function (r) {
        if (r.success == true) {
            notify.success("Usuário", r.message, function () {
                window.location.href = "/Usuario";
            });

        } else
            notify.error("Atenção", r.message);
    },
    reloadENTable: function () {
        var $tableBody = $("#table_body");
        $tableBody.empty();

        // Recriar a tabela com base na lista atual
        $.each(userJs.enList, function (index, item) {
            var corretoraText = item.corretoraText || "*";
            var canalText = item.canalText || "*";
            var pontoAtendimentoText = item.pontoAtendimentoText || "*";

            var template = `
                <tr data-index="${index}">
                    <td>${corretoraText}</td>
                    <td>${canalText}</td>
                    <td>${pontoAtendimentoText}</td>
                    <td class="text-center">
                        <input type="hidden" name="EstruturaNegocio[${index}].CorretoraId" value="${item.corretoraId || ''}" />
                        <input type="hidden" name="EstruturaNegocio[${index}].CanalId" value="${item.canalId || ''}" />
                        <input type="hidden" name="EstruturaNegocio[${index}].PontoAtendimentoId" value="${item.pontoAtendimentoId || ''}" />
                        <button type="button" class="btn btn-danger removeEN" data-index="${index}">
                            <i class="bi bi-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
            $tableBody.append(template);
        });

        // Reativa os listeners para os botões de remoção
        userJs.bindRemoveENEvents();
    },
    // Função para vincular eventos aos botões de remoção
    bindRemoveENEvents: function () {
        $(".removeEN").off("click").on("click", function () {
            var index = $(this).data("index");

            // Remove o item da lista pela posição
            userJs.enList.splice(index, 1);

            // Recarrega a tabela para atualizar os índices
            userJs.reloadENTable();
        });
    },

}

$(document).ready(function () {
    userJs.init();






});
