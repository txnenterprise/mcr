// Última alteração: Município da cotação no select (view Gerenciar renderiza opção do Model.Municipio)
// e logs de diagnóstico no Console (init, submit, resposta) para evitar envio com Município vazio e "Não há propostas disponíveis".
// Diagnóstico: F12 > Console. Sem redirect: cotacaoAgricolaJS.debugSemRedirect = true
// Copiar resposta: copy(JSON.stringify(cotacaoAgricolaJS.ultimaResposta))
var cotacaoAgricolaJS = {
    debugSemRedirect: false,
    ultimaResposta: null,
    ultimoSubmit: null,
    init: function () {
        var cotacaoId = $("#Id").val();
        var estado = $("#Estado").val();
        var municipioVal = $("#Municipio").val();
        var municipioDataValue = $("#Municipio").data("value") || $("#Municipio").attr("data-value");
        var municipioOk = !!(municipioVal && municipioVal.trim().length > 0);

        console.log("[CotacaoAgricola] --- INIT ---");
        console.log("[CotacaoAgricola] URL:", window.location.href);
        console.log("[CotacaoAgricola] Cotação Id:", cotacaoId || "(nova)");
        console.log("[CotacaoAgricola] Estado:", estado || "(vazio)");
        console.log("[CotacaoAgricola] Município (valor do select):", municipioVal || "(vazio)");
        console.log("[CotacaoAgricola] Município (data-value):", municipioDataValue || "(vazio)");
        console.log("[CotacaoAgricola] Município OK para envio?", municipioOk ? "SIM" : "NAO - risco de 'Não há propostas disponíveis'");
        console.log("[CotacaoAgricola] TipoSolo:", $("#TipoSolo").val());
        console.log("[CotacaoAgricola] ClassificacaoSolo:", $("#ClassificacaoSolo").val());
        console.log("[CotacaoAgricola] CulturaId:", $("#CulturaId").val());
        console.log("[CotacaoAgricola] SafraId:", $("#SafraId").val());

        // Aplicar máscaras
        Basics.maskCpf("#ClienteInfo_Cpf");
        Basics.currencyMask("#CustoProducao");
        Basics.currencyMask("#PrecoSaca");
        Basics.currencyMask("#ValorCusteio");
        Basics.currencyMask("#AreaTotal");
        Basics.maskPhone("#UsuarioTelefone");
        $('.selected2').select2({
            placeholder: 'Selecione...',
            allowClear: true,
            width: '100%'
        });
        $(".proposta_order").off("click").bind("click", function () {
            var cotacaoId = $(this).data("cotacao");
            var order = $(this).data("order");
            gl.RenderGet("CotacaoAgricolaDadosPropostas", "CotacaoAgricola", { cotacaoId: cotacaoId, order: order },
                "#container_propostas",
                true,
                null,
                function () {
                    
                }
            );
 
        });

        // Função para buscar cliente por CPF
        $("#ClienteInfo_Cpf").off("blur").bind("blur", function () {
            var cpf = $(this).val().replace(/[^\d]/g, '');
            if (!cpf || cpf.length < 11) {
                $("#ClienteInfo_ClienteId").val('');
                $("#ClienteInfo_Nome").val('');
                return;
            }

            gl.Get("ObterClienteByCpf", "Cliente", { cpf: cpf }, false, function (r) {
                if (r && r.id) {
                    $("#ClienteInfo_ClienteId").val(r.id);
                    $("#ClienteInfo_Nome").val(r.nome);
                } else {
                    $("#ClienteInfo_ClienteId").val('');
                    $("#ClienteInfo_Nome").val('');
                    alert("Cliente não encontrado para o CPF informado. Cadastre o cliente antes de criar a cotação.");
                }
            });
        });

        // Função para controlar visibilidade dos campos de modalidade
        function toggleModalidadeFields() {
            var isModalidadeProdutividade = $('#IsModalidadeProdutividade').is(':checked');
            const $divPrecoSaca = $('.divPrecoSaca');
            const $divValorCusteio = $('#divValorCusteio');

            if (isModalidadeProdutividade) {
                $divPrecoSaca.show();
                $divValorCusteio.hide();
                $('#ValorCusteio').val('');
            } else {
                $divPrecoSaca.hide();
                $divValorCusteio.show();
                $('#PrecoSaca').val('');
            }
        }

        // Estado inicial dos campos de modalidade
        toggleModalidadeFields();

        // Monitor mudanças na modalidade
        $('#IsModalidadeProdutividade').off("change").bind("change", toggleModalidadeFields);

        // Função para carregar municípios
        async function atualizarMunicipios($select, estado, municipioSelecionado = null) {
            if (!estado) {
                $select.empty().append('<option value="">Selecione primeiro um estado</option>');
                return;
            }

            try {
                $select.empty().append('<option value="">Carregando...</option>');
                const municipios = await carregarMunicipios(estado);

                $select.empty().append('<option value="">Selecione o município</option>');
                municipios.forEach(municipio => {
                    $select.append(
                        $('<option>', {
                            value: municipio.nome,
                            text: municipio.nome,
                            selected: municipioSelecionado && municipio.nome === municipioSelecionado
                        })
                    );
                });
                // Garantir que o valor seja aplicado (Select2 não reflete só o atributo selected)
                if (municipioSelecionado) {
                    var temOpcao = $select.find('option').filter(function () { return $(this).val() === municipioSelecionado; }).length > 0;
                    if (temOpcao) {
                        $select.val(municipioSelecionado).trigger('change');
                        console.log("[CotacaoAgricola] Município restaurado (lista API):", municipioSelecionado);
                    } else {
                        var opt = municipios.find(function (m) { return m.nome && m.nome.trim() === municipioSelecionado.trim(); });
                        if (opt) {
                            $select.val(opt.nome).trigger('change');
                            console.log("[CotacaoAgricola] Município restaurado (fallback trim):", opt.nome);
                        } else {
                            console.warn("[CotacaoAgricola] Município da cotação NÃO encontrado na lista da API:", municipioSelecionado);
                        }
                    }
                }
            } catch (error) {
                console.error("[CotacaoAgricola] Erro ao carregar municípios:", error);
                $select.empty().append('<option value="">Erro ao carregar municípios</option>');
                alert("Não foi possível carregar a lista de municípios. Por favor, tente novamente.");
            }
        }

        // Monitor mudanças no estado
        $('#Estado').select2().off('select2:select').on('select2:select', async function (e) {
            const estado = $(this).val();
            await atualizarMunicipios($("#Municipio"), estado);
        });

        // Carrega municípios iniciais e restaura o município da cotação (data-value)
        const estadoInicial = $("#Estado").val();
        const municipioAnterior = $("#Municipio").data("value") || $("#Municipio").attr("data-value");
        if (estadoInicial) {
            (async function () {
                await atualizarMunicipios($("#Municipio"), estadoInicial, municipioAnterior);
                var valFinal = $("#Municipio").val();
                if (municipioAnterior && !valFinal) {
                    console.warn("[CotacaoAgricola] Município da cotação NÃO foi restaurado após carregar lista. Esperado:", municipioAnterior, "| Valor atual:", valFinal);
                } else if (municipioAnterior && valFinal) {
                    console.log("[CotacaoAgricola] Município após carregar lista:", valFinal);
                }
                console.log("[CotacaoAgricola] --- INIT (após municípios) --- Município no form:", $("#Municipio").val() || "(vazio)");
            })();
        }

        // Cascata: Corretora → Canal
        $('#CorretoraId').select2().off('select2:select').on('select2:select', function (e) {
            const corretoraId = $(this).val();
            const $canal = $('#CanalId');
            var canalPreSelected = $canal.val();
            $canal.empty().append('<option value="">Carregando...</option>');
            if (!corretoraId) {
                $canal.empty().append('<option value="">Selecione uma corretora primeiro</option>');
                return;
            }
            gl.Get("ObterCanaisPorCorretora", "CotacaoAgricola", { corretoraId: corretoraId }, false, function (r) {
                $canal.empty().append('<option value="">Selecione...</option>');
                var jaIncluido = false;
                if (r && r.length > 0) {
                    r.forEach(function (item) {
                        $canal.append('<option value="' + item.id + '">' + item.text + '</option>');
                        if (canalPreSelected && item.id == canalPreSelected) jaIncluido = true;
                    });
                }
                // Se o canal pré-selecionado não está na lista, adicionar
                if (canalPreSelected && !jaIncluido) {
                    var textoPre = $canal.find('option[value="' + canalPreSelected + '"]').text();
                    if (!textoPre) {
                        // Buscar texto do select original antes de limpar
                        $canal.append('<option value="' + canalPreSelected + '" selected>' + (canalPreSelected + '').substring(0, 8) + '</option>');
                    }
                }
                if (canalPreSelected) {
                    $canal.val(canalPreSelected).trigger('change');
                } else {
                    $canal.trigger('change');
                }
            });
        });

        // Disparar cascata inicial APENAS se corretora tem valor E canal NÃO tem
        var corretoraInicial = $('#CorretoraId').val();
        var canalInicial = $('#CanalId').val();
        if (corretoraInicial && !canalInicial) {
            $('#CorretoraId').trigger('select2:select');
        }

        // Vincular evento select2:select no CanalId (já inicializado por .selected2)
        $('#CanalId').off('select2:select').on('select2:select', function (e) {
            const canalId = $(this).val();
            const $pa = $('#PontoAtendimentoId');
            var paPreSelected = $pa.val();
            $pa.empty();

            if (!canalId) {
                $pa.append('<option value="">Selecione...</option>');
                return;
            }

            gl.Get("ListarPontosAtendimentoPorCanal", "PontoAtendimento", { canalId: canalId }, false, function (r) {
                if (r && r.length > 0) {
                    $pa.append('<option value="">Selecione...</option>');
                    r.forEach(item => {
                        $pa.append(`<option value="${item.id}">${item.text}</option>`);
                    });
                } else {
                    $pa.append('<option value="">Nenhum ponto de atendimento encontrado</option>');
                }
                if (paPreSelected) {
                    $pa.val(paPreSelected).trigger('change');
                }
            });
        });

        // Forçar seleção do CanalId após todas as inicializações do select2
        var canalModelVal = $('#CanalId option[selected]').val();
        if (canalModelVal) {
            $('#CanalId').val(canalModelVal).trigger('change');
        }

        // Carregar tipos de solo conforme produtos da Cultura e Safra selecionadas
        function carregarTiposSoloPorCulturaSafra() {
            const culturaId = $("#CulturaId").val();
            const safraId = $("#SafraId").val();
            const $tipoSolo = $("select[name='TipoSolo'], #TipoSolo").first();
            if (!$tipoSolo.length) return;

            function atualizarSelect(opcoes, valoresSelecionados) {
                if ($tipoSolo.hasClass("select2-hidden-accessible")) {
                    try { $tipoSolo.select2("destroy"); } catch (e) { }
                }
                $tipoSolo.empty();
                opcoes.forEach(function (opt) {
                    $tipoSolo.append($("<option>", { value: opt.value, text: opt.text }));
                });
                if (valoresSelecionados && valoresSelecionados.length > 0) {
                    $tipoSolo.val(valoresSelecionados).trigger("change");
                }
                $tipoSolo.select2({ placeholder: "Selecione...", allowClear: true, width: "100%" });
            }

            if (!culturaId || !safraId) {
                atualizarSelect([{ value: "", text: "Selecione cultura e safra para carregar os tipos de solo dos produtos" }], null);
                return;
            }

            gl.Get("ObterTiposSoloPorCulturaSafra", "CotacaoAgricola", { culturaId: culturaId, safraId: safraId }, false, function (r) {
                var lista = (r && r.length) ? r : [];
                var opcoes = [{ value: "", text: lista.length ? "Selecione..." : "Nenhum tipo de solo cadastrado nos produtos" }].concat(
                    lista.map(function (item) {
                        var v = item.value !== undefined ? item.value : item.Value;
                        var t = item.text || item.Text || ("Tipo " + v);
                        return { value: String(v), text: t };
                    })
                );
                atualizarSelect(opcoes, null);
            });
        }

        // Carregar classificações de solo conforme produtos da Cultura e Safra selecionadas
        function carregarClassificacoesSoloPorCulturaSafra() {
            const culturaId = $("#CulturaId").val();
            const safraId = $("#SafraId").val();
            const $classificacaoSolo = $("select[name='ClassificacaoSolo'], #ClassificacaoSolo").first();
            if (!$classificacaoSolo.length) return;

            function atualizarSelect(opcoes, valoresSelecionados) {
                if ($classificacaoSolo.hasClass("select2-hidden-accessible")) {
                    try { $classificacaoSolo.select2("destroy"); } catch (e) { }
                }
                $classificacaoSolo.empty();
                opcoes.forEach(function (opt) {
                    $classificacaoSolo.append($("<option>", { value: opt.value, text: opt.text }));
                });
                if (valoresSelecionados && valoresSelecionados.length > 0) {
                    $classificacaoSolo.val(valoresSelecionados).trigger("change");
                }
                $classificacaoSolo.select2({ placeholder: "Selecione...", allowClear: true, width: "100%" });
            }

            if (!culturaId || !safraId) {
                atualizarSelect([{ value: "", text: "Selecione cultura e safra para carregar as classificações dos produtos" }], null);
                return;
            }

            gl.Get("ObterClassificacoesSoloPorCulturaSafra", "CotacaoAgricola", { culturaId: culturaId, safraId: safraId }, false, function (r) {
                var lista = (r && r.length) ? r : [];
                var opcoes = [{ value: "", text: lista.length ? "Selecione..." : "Nenhuma classificação cadastrada nos produtos" }].concat(
                    lista.map(function (item) {
                        var v = typeof item === "string" ? item : (item.value || item.Value || "");
                        return { value: v, text: v };
                    })
                );
                atualizarSelect(opcoes, null);
            });
        }

        function carregarTiposEClassificacoesSolo() {
            carregarTiposSoloPorCulturaSafra();
            carregarClassificacoesSoloPorCulturaSafra();
        }

        function carregarSeAmbosPreenchidos() {
            if ($("#CulturaId").val() && $("#SafraId").val()) {
                carregarTiposEClassificacoesSolo();
            }
        }

        $("#CulturaId, #SafraId").off("select2:select").on("select2:select", carregarSeAmbosPreenchidos);
        $("#CulturaId, #SafraId").off("select2:clear").on("select2:clear", function () {
            $("#TipoSolo").empty().append('<option value="">Selecione...</option>').trigger("change");
            $("#ClassificacaoSolo").empty().append('<option value="">Selecione...</option>').trigger("change");
        });

        // Carregar os selects se já houver Cultura/Safra selecionadas (edição)
        carregarSeAmbosPreenchidos();

        // Initialize temporary alerts
        initializeTemporaryAlerts();

        // Inicializar modal de clientes (desacoplado do init principal)

        // Esconde mensagens de validação do container ao alterar campos
        function hideValidationOnChange() {
            var campos = ["TipoSolo", "ClassificacaoSolo", "CulturaId", "SafraId",
                          "Estado", "Municipio", "CanalId", "PontoAtendimentoId",
                          "CorretoraId", "AreaTotal", "PrecoSaca", "ValorCusteio",
                          "IsModalidadeProdutividade"];
            campos.forEach(function (name) {
                var el = $("#" + name + ", select[name='" + name + "'], input[name='" + name + "']");
                el.off("change.hideErr").on("change.hideErr", function () {
                    var $container = $("#container_propostas");
                    if ($container.length) {
                        $container.find("span[data-field='" + name + "']").fadeOut(300);
                        $container.find("li:contains('" + name + ":')").fadeOut(300);
                    }
                });
            });
        }
        hideValidationOnChange();

        // Log ao enviar o formulário (diagnóstico) e guardar para copiar depois
        $("#formGerenciar").on("submit", function () {
            if (cotacaoAgricolaJS._submitting) { return false; }
            cotacaoAgricolaJS._submitting = true;
            var arr = $(this).serializeArray();
            var obj = {};
            arr.forEach(function (item) { obj[item.name] = item.value; });
            cotacaoAgricolaJS.ultimoSubmit = obj;
            var m = obj.Municipio || obj["Municipio"];
            console.log("[CotacaoAgricola] --- SUBMIT ---");
            console.log("[CotacaoAgricola] Município enviado no POST:", m || "(VAZIO - motor pode não gerar propostas)");
            console.log("[CotacaoAgricola] Cotação Id:", obj.Id);
            console.log("[CotacaoAgricola] Estado:", obj.Estado);
            console.log("[CotacaoAgricola] Dados completos (copiar): copy(JSON.stringify(cotacaoAgricolaJS.ultimoSubmit))");
        });
    },
    onSuccessCallback: function (r) {
        cotacaoAgricolaJS._submitting = false;
        cotacaoAgricolaJS.ultimaResposta = r;
        console.log("[CotacaoAgricola] --- RESPOSTA SERVIDOR ---");
        console.log("[CotacaoAgricola] success:", r.success);
        console.log("[CotacaoAgricola] message:", r.message);
        console.log("[CotacaoAgricola] cotacaoId:", r.cotacaoId);
        if (r.motorDiagnostico) {
            console.log("[CotacaoAgricola] --- MOTOR DIAGNÓSTICO (por que não há propostas) ---");
            console.log("[CotacaoAgricola] Qtd produtos elegíveis:", r.motorDiagnostico.qtdProdutosElegiveis);
            console.log("[CotacaoAgricola] Qtd propostas gravadas:", r.motorDiagnostico.qtdPropostasGravadas);
            console.log("[CotacaoAgricola] Estado:", r.motorDiagnostico.estado);
            console.log("[CotacaoAgricola] Município:", r.motorDiagnostico.municipio);
            console.log("[CotacaoAgricola] CulturaId:", r.motorDiagnostico.culturaId);
            console.log("[CotacaoAgricola] SafraId:", r.motorDiagnostico.safraId);
            console.log("[CotacaoAgricola] CanalId:", r.motorDiagnostico.canalId);
            console.log("[CotacaoAgricola] PontoAtendimentoId:", r.motorDiagnostico.pontoAtendimentoId);
            console.log("[CotacaoAgricola] AreaTotal:", r.motorDiagnostico.areaTotal);
            console.log("[CotacaoAgricola] Motivo sem propostas:", r.motorDiagnostico.motivoSemPropostas || "(não informado)");
            if (r.motorDiagnostico.produtoIdElegivel) {
                console.log("[CotacaoAgricola] Produto elegível (ID):", r.motorDiagnostico.produtoIdElegivel);
                console.log("[CotacaoAgricola] Produto elegível (nome):", r.motorDiagnostico.nomeProdutoElegivel || "(não informado)");
                console.log("[CotacaoAgricola] Detalhe taxas:", r.motorDiagnostico.detalheTaxas || "(não informado)");
            }
            console.log("[CotacaoAgricola] Objeto completo motorDiagnostico:", r.motorDiagnostico);
        }
        if (!r.success) console.error("[CotacaoAgricola] Resposta de erro:", r);
        console.log("[CotacaoAgricola] Copiar resposta: copy(JSON.stringify(cotacaoAgricolaJS.ultimaResposta))");
        if (r.success === true && r.cotacaoId) {
            notify.success("Cotação Salva", r.message);
            if (cotacaoAgricolaJS.debugSemRedirect) {
                console.warn("[CotacaoAgricola] Redirect desativado. Redirecionar: window.location.href = '/CotacaoAgricola/Gerenciar/" + r.cotacaoId + "';");
            } else if (!document.getElementById('container_propostas')) {
                window.location.href = '/CotacaoAgricola/Gerenciar/' + r.cotacaoId;
                return;
            }
            var motor = r.motorDiagnostico;
            if (motor && motor.qtdPropostasGravadas === 0) {
                if (typeof produtosReferencia !== 'undefined') {
                    produtosReferencia.atualizar('formGerenciar');
                }
                var diagnostico = "";
                if (motor.passoAPasso && motor.passoAPasso.length > 0) {
                    motor.passoAPasso.forEach(function (p) {
                        var icon = p.sucesso
                            ? '<i class="bi bi-check-circle text-success me-1"></i>'
                            : '<i class="bi bi-x-circle text-danger me-1"></i>';
                        diagnostico += '<li class="mb-2">' + icon + ' <strong>' + p.etapa + ':</strong> ' + p.mensagem;
                        if (p.dica) {
                            diagnostico += '<br/><span class="text-muted ms-4"><i class="bi bi-lightbulb text-warning me-1"></i>' + p.dica + '</span>';
                        }
                        if (p.produtosExcluidos && p.produtosExcluidos.length > 0) {
                            diagnostico += '<ul class="list-unstyled ms-4 mt-1 mb-0">';
                            p.produtosExcluidos.forEach(function (ex) {
                                diagnostico += '<li style="font-size:0.85rem"><i class="bi bi-dash text-danger me-1"></i><em>' + ex.nomeProduto + '</em> — ' + ex.motivo + '</li>';
                            });
                            diagnostico += '</ul>';
                        }
                        diagnostico += '</li>';
                    });
                } else if (motor.motivoSemPropostas) {
                    var linhas = motor.motivoSemPropostas.split(" | ");
                    linhas.forEach(function (d) {
                        diagnostico += '<li class="mb-1"><i class="bi bi-exclamation-triangle text-warning me-1"></i> ' + d + '</li>';
                    });
                }
                if (motor.detalheTaxas) {
                    diagnostico += '<li class="mb-1"><i class="bi bi-info-circle text-info me-1"></i> ' + motor.detalheTaxas + '</li>';
                }
                var container = document.getElementById('container_propostas');
                container.innerHTML = '<div class="card pt-3" style="border-radius:15px;border-color:var(--color-primary)">' +
                    '<div class="card-body pt-3" style="color:var(--color-primary)">' +
                    '<div class="row"><div class="col-12 text-center">' +
                    '<h5>Não há propostas disponíveis</h5>' +
                    (diagnostico ? '<div class="mt-3 text-start" style="max-width:700px;margin:0 auto">' +
                        '<p class="text-muted mb-2"><strong>Verificação passo a passo:</strong></p>' +
                        '<ul class="list-unstyled mb-0">' + diagnostico + '</ul>' +
                    '</div>' : '') +
                    '</div></div></div></div>';
                return;
            }
            console.log("[CotacaoAgricola] Renderizando resumo da cotação em tela...");
            gl.RenderGet("CotacaoAgricolaDadosPropostas", "CotacaoAgricola", { cotacaoId: r.cotacaoId, order: "segurada" },
                "#container_propostas",
                true,
                null,
                null
            );
            if (typeof produtosReferencia !== 'undefined') {
                produtosReferencia.atualizar('formGerenciar');
            }
        } else if (r.success === true) {
            notify.success("Cotação Salva", r.message || "Cotação processada.");
        } else {
            notify.error(r.message || "Erro ao processar cotação.");
        }
    },
    onSuccessCallbackProposta: function (r) {
        if (r.success == true) {
            var propostaId = r.propostaId;
            notify.success("Proposta Criada", r.message, function (d) {
                if (propostaId != null && propostaId != undefined)
                    window.location.href = "/Propostas/Cadastrar/" + propostaId;
            });

        } else
            notify.error(r.message);
    },

    // Funções para o modal de seleção de clientes
    cotacaoIdSelecionada: null,

    abrirModalSelecionarCliente: function (cotacaoId) {
        this.cotacaoIdSelecionada = cotacaoId;
        $('#modalNumeroCotacao').text('#' + cotacaoId.substring(0, 8));
        var modalEl = document.getElementById('modalSelecionarCliente');
        var modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
        this.buscarClientes();
    },

    buscarClientes: function () {
        const nome = $('#pesquisaNomeCliente').val();
        const cpf = $('#pesquisaCPFCliente').val();

        $.get('/Cliente/PesquisarClientes', { pesquisaNome: nome, pesquisaCPF: cpf }, function (data) {
            const tbody = $('#tblClientesModal tbody');
            tbody.empty();

            if (data.listaClientes && data.listaClientes.length > 0) {
                data.listaClientes.forEach(cliente => {
                    tbody.append(`
                        <tr>
                            <td class="text-center">
                                <input type="radio" name="clienteSelecionado" value="${cliente.id}">
                            </td>
                            <td>${cliente.nome || ''}</td>
                            <td>${cliente.cpf || ''}</td>
                            <td>${cliente.telefone || ''}</td>
                            <td>${cliente.email || ''}</td>
                        </tr>
                    `);
                });
            } else {
                tbody.append('<tr><td colspan="5" class="text-center">Nenhum cliente encontrado.</td></tr>');
            }

            // Evento para habilitar o botão ao selecionar um cliente
            $('input[name=\"clienteSelecionado\"]').on('change', function () {
                $('#btnConfirmarCliente').prop('disabled', !$('input[name=\"clienteSelecionado\"]:checked').length);
            });
        }).fail(function () {
            const tbody = $('#tblClientesModal tbody');
            tbody.empty();
            tbody.append('<tr><td colspan="5" class="text-center text-danger">Erro ao buscar clientes.</td></tr>');
        });
    },

    vincularCliente: function (clienteId) {
        if (!this.cotacaoIdSelecionada) {
            alert('Erro: ID da cotação não encontrado.');
            return;
        }

        var btn = document.getElementById('btnConfirmarCliente');
        var originalText = btn.innerHTML;
        btn.innerHTML = '<i class="bi bi-hourglass-split"></i> Vinculando...';
        btn.disabled = true;

        var params = new URLSearchParams();
        params.append('clienteId', clienteId);
        params.append('cotacaoId', this.cotacaoIdSelecionada);

        fetch('/Cliente/VincularClienteACotacao', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: params
        })
        .then(function (response) {
            if (!response.ok) throw new Error('HTTP ' + response.status);
            return response.json();
        })
        .then(function (data) {
            if (data.success) {
                var modalEl = document.getElementById('modalSelecionarCliente');
                var modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
                window.location.href = data.url;
            } else {
                alert('Erro: ' + data.message);
                btn.innerHTML = originalText;
                btn.disabled = false;
            }
        })
        .catch(function (error) {
            console.error('[Vincular] Erro:', error);
            alert('Erro ao vincular cliente: ' + error.message);
            btn.innerHTML = originalText;
            btn.disabled = false;
        });
    },

    initModalClientes: function () {
        // Usar delegação de eventos no document para garantir que funcione
        // mesmo se o modal for re-renderizado
        $(document).on('click', '#btnPesquisarClientes', function () {
            cotacaoAgricolaJS.buscarClientes();
        });

        $(document).on('keypress', '#pesquisaNomeCliente, #pesquisaCPFCliente', function (e) {
            if (e.which === 13) {
                cotacaoAgricolaJS.buscarClientes();
            }
        });

        $(document).on('click', '#btnConfirmarCliente', function () {
            var clienteId = $('input[name="clienteSelecionado"]:checked').val();
            if (!clienteId) {
                alert('Selecione um cliente para vincular.');
                return;
            }
            cotacaoAgricolaJS.vincularCliente(clienteId);
        });
    }
}
$(document).ready(function () {
    // SEMPRE inicializar modal de clientes, independente do init() principal
    cotacaoAgricolaJS.initModalClientes();

    try {
        cotacaoAgricolaJS.init();
    } catch (e) {
        console.error("[CotacaoAgricola] Erro no init:", e);
    }
});
