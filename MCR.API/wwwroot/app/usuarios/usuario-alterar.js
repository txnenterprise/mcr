var userJs = {
    enList: [],
    init: function () {

        Basics.maskCpf("#formCadastro input[name=Document]");
        Basics.maskPhone("#formCadastro input[name=PhoneNumber]");

        $('.selected2').select2({
            placeholder: 'Selecione...',
            allowClear: true,
            width: '100%'
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

            // Limpa apenas Canal e PA para forçar nova seleção
            // Mantém Corretora porque o usuário pode vincular o mesmo canal/PA a diferentes combos
            if ($("#DefaultCanalId option").length > 2) {
                $("#DefaultCanalId").val("").trigger('change');
            } else if ($("#DefaultCanalId option").length == 2) {
                var singleOption = $("#DefaultCanalId option:not([value=''])").val();
                $("#DefaultCanalId").val(singleOption).trigger('change');
            }

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
        var historyRaw = $("#enHistory").val();
        var history = (historyRaw && historyRaw !== "null" && historyRaw !== "") ? JSON.parse(historyRaw) : [];
        if (!Array.isArray(history)) history = [];
        userJs.enList = history;
        userJs.reloadENTable();

        // Inicializa o evento de alteração de senha
        userJs.initChangePassword();
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

    initChangePassword: function() {
        $("#btnChangePassword").off("click").on("click", function() {
            var newPassword = $("#newPassword").val();
            var confirmPassword = $("#confirmPassword").val();

            // Validações básicas
            if (!newPassword || !confirmPassword) {
                notify.error("Atenção", "Todos os campos são obrigatórios");
                return;
            }

            if (newPassword !== confirmPassword) {
                notify.error("Atenção", "As senhas não coincidem");
                return;
            }

            // Monta o objeto para envio (UserId = usuário em edição; se houver, altera a senha dele)
            var userIdInput = $("#formCadastro input[name='Id']").val();
            var data = {
                newPass: newPassword
            };
            if (userIdInput) {
                data.userId = userIdInput;
            }

            // Envia a requisição
            console.log("Iniciando requisição");
            $.ajax({
                url: "/Usuario/ChangePassword",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(data),
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                },
                beforeSend: function() {
                    console.log("Before send");
                },
                success: function(response) {
                    if (response.success) {
                        notify.success("Sucesso", response.message);
                        $("#changePasswordModal").modal("hide");
                        $("#changePasswordForm")[0].reset();
                    } else {
                        notify.error("Atenção", response.message);
                    }
                },
                error: function(xhr, status, error) {
                    console.log("Error:", {xhr: xhr, status: status, error: error});
                    notify.error("Erro", "Ocorreu um erro ao alterar a senha");
                },
                complete: function() {
                    console.log("Complete");
                }
            });
        });
    },

}

$(document).ready(function () {
    userJs.init();
});
