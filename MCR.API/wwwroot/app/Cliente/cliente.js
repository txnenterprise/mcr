// Cliente JavaScript Module
var clienteJS = {
    onSuccessCallback: function (r) {
        if (r.success == true) {
            var clienteId = r.clienteId;
            if (r.url == null || r.url == undefined) {
                // Determinar se é cadastro ou alteração baseado na URL atual
                const isEditing = window.location.pathname.includes('/Gerenciar');
                const title = isEditing ? "Cliente Alterado" : "Cliente Salvo";

                notify.success(title, r.message, function (d) {
                    if (clienteId != null && clienteId != undefined) {
                        window.location.href = "/Cliente/Index";
                    } else {
                        window.location.href = "/Cliente/Index";
                    }
                });
            } else
                window.location.href = r.url;
        } else {
            notify.error("Erro ao Salvar", r.message || "Ocorreu um erro ao salvar o cliente.");
        }
    },

    // Inicialização quando o DOM estiver carregado
    init: function () {
        this.initMascarasContato();
        this.initImageUploads();
        this.initVinculosFamiliares();
        this.initPropriedades();
        this.initModais();
        this.initFormValidation();
        this.initModalGerenciarPropriedade();
        this.initBancoAutocomplete();
    },

    // Máscaras: Step Contato (telefone/celular) e Step Vínculo familiares (celular/telefone)
    initMascarasContato: function () {
        if (typeof Basics !== 'undefined' && Basics.maskPhone) {
            Basics.maskPhone("#Telefone");
            Basics.maskPhone("#Celular");
            Basics.maskPhone("#TelefoneVinculo");
            Basics.maskPhone("#CelularVinculo");
        }
    },

    // Select2 no campo Banco (Dados bancários) - filtro por autocomplete
    initBancoAutocomplete: function () {
        if ($('#Banco').length && typeof $.fn.select2 !== 'undefined') {
            $('#Banco').select2({
                placeholder: 'Digite ou selecione o banco',
                allowClear: true,
                width: '100%'
            });
        }
    },

    // Variáveis para armazenar base64 de imagens/PDFs (evita hidden fields gigantes)
    _imagemCPFBase64: null,
    _imagemRGBase64: null,

    // Configuração de upload de imagens
    initImageUploads: function () {
        const self = this;

        // Upload de imagem do CPF
        const imageUploadCPF = document.getElementById('imageUploadCPF');
        if (imageUploadCPF) {
            imageUploadCPF.addEventListener('change', function () {
                const fileCPF = this.files[0];
                if (fileCPF) {
                    const readerCPF = new FileReader();
                    readerCPF.onload = function (event) {
                        const base64StringCPF = event.target.result;
                        self._imagemCPFBase64 = base64StringCPF;
                        document.getElementById('ObjectImagemCPF').value = base64StringCPF;

                        // Atualizar preview se existir
                        const imagePreviewCPF = document.getElementById('imagePreviewCPF');
                        if (imagePreviewCPF) {
                            imagePreviewCPF.src = base64StringCPF;
                        }

                        // Atualizar nome do arquivo se existir (para Gerenciar)
                        const fileNameCPF = document.getElementById('fileNameCPF');
                        if (fileNameCPF) {
                            const fileName = fileCPF.name.length > 50 ? fileCPF.name.substring(0, 47) + '...' : fileCPF.name;
                            fileNameCPF.textContent = fileName;
                            fileNameCPF.title = fileCPF.name;
                        }
                    };
                    readerCPF.readAsDataURL(fileCPF);
                }
            });
        }

        // Upload de imagem do RG
        const imageUploadRG = document.getElementById('imageUploadRG');
        if (imageUploadRG) {
            imageUploadRG.addEventListener('change', function () {
                const fileRG = this.files[0];
                if (fileRG) {
                    const readerRG = new FileReader();
                    readerRG.onload = function (event) {
                        const base64StringRG = event.target.result;
                        self._imagemRGBase64 = base64StringRG;
                        document.getElementById('ObjectImagemRG').value = base64StringRG;

                        // Atualizar preview se existir
                        const imagePreviewRG = document.getElementById('imagePreviewRG');
                        if (imagePreviewRG) {
                            imagePreviewRG.src = base64StringRG;
                        }

                        // Atualizar nome do arquivo se existir (para Gerenciar)
                        const fileNameRG = document.getElementById('fileNameRG');
                        if (fileNameRG) {
                            const fileName = fileRG.name.length > 50 ? fileRG.name.substring(0, 47) + '...' : fileRG.name;
                            fileNameRG.textContent = fileName;
                            fileNameRG.title = fileRG.name;
                        }
                    };
                    readerRG.readAsDataURL(fileRG);
                }
            });
        }
    },

    // Configuração de vínculos familiares
    initVinculosFamiliares: function () {
        const addVinculoBtn = document.getElementById("addVinculoBtn");
        if (addVinculoBtn) {
            addVinculoBtn.addEventListener("click", function () {
                const cpf = document.getElementById("CpfVinculo").value;
                const nome = document.getElementById("NomeVinculo").value;
                const telefone = document.getElementById("TelefoneVinculo").value;
                const celular = document.getElementById("CelularVinculo").value;
                const email = document.getElementById("EmailVinculo").value;
                const relacaoParental = document.getElementById("RelacaoParental").value;

                if (!nome || !relacaoParental) {
                    alert("Preencha os campos obrigatórios (Nome e Vínculo Familiar).");
                    return;
                }

                const vinculo = {
                    Id: crypto.randomUUID(),
                    Nome: nome,
                    Cpf: cpf,
                    Telefone: telefone,
                    Celular: celular,
                    Email: email,
                    RelacaoParental: relacaoParental,
                    Ativo: true,
                    Excluido: false,
                };

                const jsonField = document.getElementById("JsonVinculosFamiliar");
                const vinculos = jsonField.value ? JSON.parse(jsonField.value) : [];
                vinculos.push(vinculo);
                jsonField.value = JSON.stringify(vinculos);

                clienteJS.atualizarTabelaVinculos(vinculos);

                document.getElementById("CpfVinculo").value = "";
                document.getElementById("NomeVinculo").value = "";
                document.getElementById("TelefoneVinculo").value = "";
                document.getElementById("CelularVinculo").value = "";
                document.getElementById("EmailVinculo").value = "";
                document.getElementById("RelacaoParental").value = "";
            });
        }

        // Repopula a tabela de vínculos ao carregar a página
        const jsonField = document.getElementById("JsonVinculosFamiliar");
        if (jsonField && jsonField.value) {
            const vinculos = JSON.parse(jsonField.value);
            this.atualizarTabelaVinculos(vinculos);
        }
    },

    atualizarTabelaVinculos: function (vinculos) {
        const tbody = document.getElementById("vinculosTableBody");
        if (!tbody) {
            console.error("Tabela de vínculos não encontrada.");
            return;
        }

        tbody.innerHTML = ""; // Limpa a tabela

        vinculos.forEach((vinculo, index) => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${vinculo.Cpf || ""}</td>
                <td>${vinculo.Nome || ""}</td>
                <td>${vinculo.Telefone || ""}</td>
                <td>${vinculo.Celular || ""}</td>
                <td>${vinculo.Email || ""}</td>
                <td>${vinculo.RelacaoParental || ""}</td>
                <td>
                    <button class="btn btn-danger btn-sm" onclick="clienteJS.removerVinculo(${index})">Excluir</button>
                </td>
            `;
            tbody.appendChild(row);
        });
    },

    removerVinculo: function (index) {
        const jsonField = document.getElementById("JsonVinculosFamiliar");
        let vinculos = jsonField.value ? JSON.parse(jsonField.value) : [];
        vinculos.splice(index, 1);
        jsonField.value = JSON.stringify(vinculos);
        this.atualizarTabelaVinculos(vinculos);
    },

    // Configuração de propriedades
    initPropriedades: function () {
        window.selectedItems = new Set(); // Conjunto para armazenar IDs selecionados
        window.hiddenField = document.getElementById("selectedItems");

        // Carregar todas as propriedades ao iniciar (sem filtros)
        function carregarPropriedades() {
            const tableBody = document.querySelector("#tblCadastroPropriedade tbody");
            if (!tableBody) return;
            fetch("/Cliente/PesquisarPropriedades?pesquisaNome=&pesquisaEstado=&pesquisaCidade=&pesquisaAtivoInativo=")
                .then(response => response.json())
                .then(data => {
                    tableBody.innerHTML = "";
                    (data.listaPropriedades || []).forEach(item => {
                        const row = document.createElement("tr");
                        row.dataset.id = item.id;
                        row.dataset.nome = item.nome;
                        row.dataset.cidade = item.cidade;
                        row.dataset.estado = item.estado;
                        row.dataset.endereco = item.endereco || "";
                        row.dataset.areaTotal = item.somaAreaTotalTalhao || "";
                        row.innerHTML = `
                            <td>${item.nome}</td>
                            <td>${item.cidade}/${item.estado}</td>
                            <td>${item.ativo ? "Sim" : "Não"}</td>
                            <td><button type="button" class="btn btn-info btn-sm" onclick="clienteJS.abrirModalGerenciarPropriedade('${item.id}')" title="Gerenciar Propriedade"><i class="bi bi-gear"></i> Gerenciar</button>
                            <button type="button" class="btn btn-primary btn-sm add-to-list">Adicionar</button></td>
                        `;
                        tableBody.appendChild(row);
                    });
                    clienteJS.configureAddButtons();
                });
        }
        carregarPropriedades();

        // Preencher tabela ao recarregar a página
        const preloadedItems = JSON.parse(window.hiddenField.value || "[]");
        preloadedItems.forEach(item => {
            // Tratar tanto item.id quanto item.Id (com I maiúsculo)
            if (item.id !== undefined) {
                window.selectedItems.add(item.id);

                const newRow = document.createElement("tr");
                newRow.dataset.id = item.id;
                newRow.dataset.nome = item.nome;
                newRow.dataset.cidade = item.cidade;
                newRow.dataset.estado = item.estado;
                newRow.dataset.endereco = item.endereco || "";

                newRow.innerHTML = `
                    <td>${item.nome}</td>
                    <td>${item.cidade}/${item.estado}</td>
                    <td>${item.areaTotal ? new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2 }).format(item.areaTotal) : '-'}</td>
                    <td>
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-info btn-sm" onclick="clienteJS.abrirModalGerenciarPropriedade('${item.id}')" title="Gerenciar Propriedade">
                                <i class="bi bi-gear"></i> Gerenciar
                            </button>
                            <button type="button" class="btn btn-danger btn-sm remove-from-list">Remover</button>
                        </div>
                    </td>
                `;
                document.querySelector("#tblPropriedadesSelecionadas tbody").appendChild(newRow);
            } else if (item.Id !== undefined) {
                window.selectedItems.add(item.Id);

                const newRow = document.createElement("tr");
                newRow.dataset.id = item.Id;
                newRow.dataset.nome = item.Nome;
                newRow.dataset.cidade = item.Cidade;
                newRow.dataset.estado = item.Estado;
                newRow.dataset.endereco = item.Endereco || "";

                newRow.innerHTML = `
                    <td>${item.Nome}</td>
                    <td>${item.Cidade}/${item.Estado}</td>
                    <td>${item.areaTotal ? new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2 }).format(item.areaTotal) : '-'}</td>
                    <td>
                        <div class="btn-group" role="group">
                            <button type="button" class="btn btn-info btn-sm" onclick="clienteJS.abrirModalGerenciarPropriedade('${item.Id}')" title="Gerenciar Propriedade">
                                <i class="bi bi-gear"></i> Gerenciar
                            </button>
                            <button type="button" class="btn btn-danger btn-sm remove-from-list">Remover</button>
                        </div>
                    </td>
                `;
                document.querySelector("#tblPropriedadesSelecionadas tbody").appendChild(newRow);
            }
        });

        this.configureRemoveButtons();
    },

    configureAddButtons: function () {
        document.querySelectorAll(".add-to-list").forEach(button => {
            button.addEventListener("click", function () {
                const row = this.closest("tr");
                const id = row.dataset.id;
                const nome = row.dataset.nome;
                const cidade = row.dataset.cidade;
                const estado = row.dataset.estado;

                if (!window.selectedItems.has(id)) {
                    window.selectedItems.add(id);
                    clienteJS.updateHiddenField();

                    const newRow = document.createElement("tr");
                    newRow.dataset.id = id;
                    newRow.dataset.nome = nome;
                    newRow.dataset.cidade = cidade;
                    newRow.dataset.estado = estado;
                    newRow.dataset.endereco = "";

                    newRow.innerHTML = `
                        <td>${nome}</td>
                        <td>${cidade}/${estado}</td>
                        <td>${row.dataset.areaTotal ? new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2 }).format(parseFloat(row.dataset.areaTotal)) : '-'}</td>
                        <td>
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-info btn-sm" onclick="clienteJS.abrirModalGerenciarPropriedade('${id}')" title="Gerenciar Propriedade">
                                    <i class="bi bi-gear"></i> Gerenciar
                                </button>
                                <button type="button" class="btn btn-danger btn-sm remove-from-list">Remover</button>
                            </div>
                        </td>
                    `;
                    document.querySelector("#tblPropriedadesSelecionadas tbody").appendChild(newRow);

                    clienteJS.configureRemoveButtons();
                }
            });
        });
    },

    configureRemoveButtons: function () {
        document.querySelectorAll(".remove-from-list").forEach(button => {
            button.addEventListener("click", function () {
                const row = this.closest("tr");
                const id = row.dataset.id;

                if (window.selectedItems.has(id)) {
                    window.selectedItems.delete(id);
                    clienteJS.updateHiddenField();
                    row.remove(); // Remover a linha da tabela
                }
            });
        });
    },

    updateHiddenField: function () {
        const itemsArray = Array.from(window.selectedItems).map(id => {
            const row = document.querySelector(`#tblPropriedadesSelecionadas tbody tr[data-id="${id}"]`);
            return {
                id: id,
                nome: row ? row.dataset.nome : "",
                endereco: row ? row.dataset.endereco : "",
                cidade: row ? row.dataset.cidade : "",
                estado: row ? row.dataset.estado : ""
            };
        });
        window.hiddenField.value = JSON.stringify(itemsArray); // Atualizar o campo oculto
    },

    // Configuração de validação e envio do formulário via FormData (suporta PDFs grandes)
    initFormValidation: function () {
        const form = document.getElementById("formCadastro");
        const submitButton = document.querySelector("#formCadastro button[type='submit']");

        if (form && submitButton) {
            // Destruir jQuery Validate neste form para não interceptar o submit
            if (typeof $ !== 'undefined') {
                var validator = $.data(form, 'validator');
                if (validator) {
                    validator.destroy();
                }
            }

            // Capture phase: roda ANTES do jQuery Validate e do browser
            form.addEventListener("submit", function (event) {
                event.preventDefault();
                event.stopImmediatePropagation();

                if (submitButton.disabled) return;
                submitButton.disabled = true;
                submitButton.innerText = "Enviando...";

                gl.BeginAjax.call(form);

                const formData = new FormData(form);

                // Injetar imagens/PDFs via variáveis JS (não depende de hidden fields no HTML)
                if (clienteJS._imagemCPFBase64) {
                    formData.set("ObjectImagemCPF", clienteJS._imagemCPFBase64);
                }
                if (clienteJS._imagemRGBase64) {
                    formData.set("ObjectImagemRG", clienteJS._imagemRGBase64);
                }

                fetch(form.getAttribute("action") || form.action, {
                    method: "POST",
                    body: formData
                })
                .then(response => {
                    if (!response.ok) {
                        throw new Error("HTTP " + response.status);
                    }
                    return response.json();
                })
                .then(data => {
                    gl.CompleteAjax.call(form);
                    gl.onSuccess.call(form, data);
                })
                .catch(error => {
                    gl.CompleteAjax.call(form);
                    console.error("Erro no envio:", error);
                    gl.onFailed({ status: 0, responseText: error.message || "Erro de rede" });
                })
                .finally(() => {
                    submitButton.disabled = false;
                    submitButton.innerText = "Salvar";
                });
            }, true); // capture phase
        }
    },

    // Variáveis globais para o modal de talhão
    modalMap: null,
    modalDrawingManager: null,
    modalSelectedShape: null,
    modalAddressMarker: null,
    propriedadeIdCriada: null,
    propriedadeIdParaReabrir: null,
    googleMapsLoaded: false,

    // Aguardar o carregamento do Google Maps
    waitForGoogleMaps: function () {
        return new Promise((resolve) => {
            if (typeof google !== 'undefined' && google.maps) {
                this.googleMapsLoaded = true;
                resolve();
            } else {
                const checkGoogleMaps = setInterval(() => {
                    if (typeof google !== 'undefined' && google.maps) {
                        this.googleMapsLoaded = true;
                        clearInterval(checkGoogleMaps);
                        resolve();
                    }
                }, 100);
            }
        });
    },

    // Inicialização dos modais
    initModais: async function () {
        await this.waitForGoogleMaps();

        const btnAdicionarPropriedade = document.getElementById("btnAdicionarPropriedade");
        const modalElement = document.getElementById("modalCadastrarPropriedade");
        const btnSalvarPropriedade = document.getElementById("btnSalvarPropriedade");
        const btnAdicionarTalhao = document.getElementById("btnAdicionarTalhao");

        // Modal de talhão
        const modalTalhaoElement = document.getElementById("modalCadastrarTalhao");
        const btnSalvarTalhao = document.getElementById("btnSalvarTalhao");

        if (!modalElement) {
            console.error("Modal modalCadastrarPropriedade não encontrado!");
            return;
        }

        // Criar instâncias dos modais
        let modalCadastrarPropriedade;
        let modalCadastrarTalhao;

        // Tentar diferentes formas de inicializar os modais
        if (typeof $ !== 'undefined' && $.fn.modal) {
            modalCadastrarPropriedade = $(modalElement);
            modalCadastrarTalhao = $(modalTalhaoElement);
        } else if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
            modalCadastrarPropriedade = new bootstrap.Modal(modalElement);
            modalCadastrarTalhao = new bootstrap.Modal(modalTalhaoElement);
        } else {
            console.error("Bootstrap não encontrado!");
            return;
        }

        // Abrir modal de propriedade
        if (btnAdicionarPropriedade) {
            btnAdicionarPropriedade.addEventListener("click", function (e) {
                e.preventDefault();
                try {
                    if (typeof $ !== 'undefined' && $.fn.modal) {
                        modalCadastrarPropriedade.modal('show');
                    } else {
                        modalCadastrarPropriedade.show();
                    }
                } catch (error) {
                    console.error("Erro ao abrir modal:", error);
                }
            });
        }
        if (btnAdicionarTalhao) {
            btnAdicionarTalhao.addEventListener("click", async function (e) {
                e.preventDefault();

                // Fechar modal de propriedade
                clienteJS.fecharModalPropriedade();

                // Abrir modal de talhão
                try {
                    if (typeof $ !== 'undefined' && $.fn.modal) {
                        modalCadastrarTalhao.modal('show');
                    } else {
                        modalCadastrarTalhao.show();
                    }

                    // Aguardar um pouco para o modal abrir completamente
                    await new Promise(resolve => setTimeout(resolve, 500));

                    // Inicializar mapa do talhão
                    await clienteJS.initializeModalMap();

                } catch (error) {
                    console.error("Erro ao abrir modal de talhão:", error);
                }
            });
        }

        // Carregar municípios ao mudar estado no modal de propriedade
        const modalEstadoSelect = document.getElementById("modalEstado");
        const modalCidadeSelect = document.getElementById("modalCidade");
        if (modalEstadoSelect && modalCidadeSelect && typeof carregarMunicipios === "function") {
            modalEstadoSelect.addEventListener("change", async function () {
                const estado = this.value;
                if (!estado) {
                    modalCidadeSelect.innerHTML = '<option value="">Selecione primeiro um estado</option>';
                    return;
                }
                try {
                    modalCidadeSelect.innerHTML = '<option value="">Carregando...</option>';
                    const municipios = await carregarMunicipios(estado);
                    modalCidadeSelect.innerHTML = '<option value="">Selecione o município</option>';
                    municipios.forEach(function (m) {
                        const opt = document.createElement("option");
                        opt.value = m.nome;
                        opt.textContent = m.nome;
                        modalCidadeSelect.appendChild(opt);
                    });
                } catch (err) {
                    console.error("Erro ao carregar municípios:", err);
                    modalCidadeSelect.innerHTML = '<option value="">Erro ao carregar municípios</option>';
                }
            });
        }

        // Consulta de endereço por CEP no modal de propriedade
        const modalCEP = document.getElementById("modalCEP");
        if (modalCEP) {
            modalCEP.addEventListener("blur", function () {
                const cep = this.value.replace(/\D/g, "");
                if (cep.length !== 8) return;
                const modalEndereco = document.getElementById("modalEndereco");
                const modalBairro = document.getElementById("modalBairro");
                const modalCidade = document.getElementById("modalCidade");
                const modalEstado = document.getElementById("modalEstado");
                if (modalEndereco) modalEndereco.value = "...";
                if (modalBairro) modalBairro.value = "...";
                if (modalCidade) modalCidade.innerHTML = '<option value="">Carregando...</option>';
                fetch("https://viacep.com.br/ws/" + cep + "/json/")
                    .then(function (r) { return r.json(); })
                    .then(async function (dados) {
                        if (dados && !dados.erro) {
                            if (modalEndereco) modalEndereco.value = dados.logradouro || "";
                            if (modalBairro) modalBairro.value = dados.bairro || "";
                            if (modalEstado) modalEstado.value = dados.uf || "";
                            if (modalCidade && dados.uf && typeof carregarMunicipios === "function") {
                                try {
                                    const municipios = await carregarMunicipios(dados.uf);
                                    modalCidade.innerHTML = '<option value="">Selecione o município</option>';
                                    const localidade = (dados.localidade || "").trim();
                                    const normalizar = function (s) { return (s || "").normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase(); };
                                    municipios.forEach(function (m) {
                                        const opt = document.createElement("option");
                                        opt.value = m.nome;
                                        opt.textContent = m.nome;
                                        if (localidade && (m.nome === localidade || normalizar(m.nome) === normalizar(localidade))) {
                                            opt.selected = true;
                                        }
                                        modalCidade.appendChild(opt);
                                    });
                                } catch (err) {
                                    modalCidade.innerHTML = '<option value="">Erro ao carregar municípios</option>';
                                }
                            }
                        } else {
                            if (modalEndereco) modalEndereco.value = "";
                            if (modalBairro) modalBairro.value = "";
                            if (modalEstado) modalEstado.value = "";
                            if (modalCidade) modalCidade.innerHTML = '<option value="">Selecione primeiro um estado</option>';
                            alert("CEP não encontrado.");
                        }
                    })
                    .catch(function () {
                        if (modalEndereco) modalEndereco.value = "";
                        if (modalBairro) modalBairro.value = "";
                        if (modalEstado) modalEstado.value = "";
                        if (modalCidade) modalCidade.innerHTML = '<option value="">Selecione primeiro um estado</option>';
                        alert("Erro ao consultar CEP.");
                    });
            });
        }

        // Salvar propriedade
        if (btnSalvarPropriedade) {
            btnSalvarPropriedade.addEventListener("click", function () {
                clienteJS.salvarPropriedade();
            });
        }

        // Salvar talhão
        if (btnSalvarTalhao) {
            btnSalvarTalhao.addEventListener("click", function () {
                clienteJS.salvarTalhao();
            });
        }

        // Limpar formulários quando os modais forem fechados
        modalElement.addEventListener("hidden.bs.modal", function () {
            clienteJS.limparFormularioModal();
        });

        modalTalhaoElement.addEventListener("hidden.bs.modal", function () {
            clienteJS.limparFormularioModalTalhao();
        });

        // Botões de fechar dos modais
        const btnClosePropriedade = modalElement.querySelector('.btn-close');
        if (btnClosePropriedade) {
            btnClosePropriedade.addEventListener("click", function () {
                clienteJS.fecharModalPropriedade();
            });
        }

        const btnCloseTalhao = modalTalhaoElement.querySelector('.btn-close');
        if (btnCloseTalhao) {
            btnCloseTalhao.addEventListener("click", function () {
                clienteJS.fecharModalTalhao();
            });
        }

        // Botões cancelar dos modais
        const btnCancelarPropriedade = modalElement.querySelector('[data-bs-dismiss="modal"]');
        if (btnCancelarPropriedade) {
            btnCancelarPropriedade.addEventListener("click", function () {
                clienteJS.fecharModalPropriedade();
            });
        }

        const btnCancelarTalhao = modalTalhaoElement.querySelector('[data-bs-dismiss="modal"]');
        if (btnCancelarTalhao) {
            btnCancelarTalhao.addEventListener("click", function () {
                clienteJS.fecharModalTalhao();
            });
        }

        // Configurar campos de análise do solo no modal de talhão
        this.configurarAnaliseSolo();
    },

    // Funções para fechar modais
    fecharModalPropriedade: function () {
        const modalElement = document.getElementById("modalCadastrarPropriedade");
        try {
            if (typeof $ !== 'undefined' && $.fn.modal) {
                $(modalElement).modal('hide');
            } else if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                const modal = bootstrap.Modal.getInstance(modalElement);
                if (modal) modal.hide();
            }
        } catch (error) {
            modalElement.style.display = 'none';
            modalElement.classList.remove('show');
            document.body.classList.remove('modal-open');

            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
                backdrop.remove();
            }
        }
    },

    fecharModalTalhao: function () {
        const modalTalhaoElement = document.getElementById("modalCadastrarTalhao");
        try {
            if (typeof $ !== 'undefined' && $.fn.modal) {
                $(modalTalhaoElement).modal('hide');
            } else if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                const modal = bootstrap.Modal.getInstance(modalTalhaoElement);
                if (modal) modal.hide();
            }
        } catch (error) {
            modalTalhaoElement.style.display = 'none';
            modalTalhaoElement.classList.remove('show');
            document.body.classList.remove('modal-open');

            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
                backdrop.remove();
            }
        }
    },

    limparFormularioModal: function () {
        const form = document.getElementById("formCadastrarPropriedade");
        if (form) {
            form.reset();
        }
        const modalCidade = document.getElementById("modalCidade");
        if (modalCidade) {
            modalCidade.innerHTML = '<option value="">Selecione primeiro um estado</option>';
        }
        const modalImagePreview = document.getElementById("modalImagePreview");
        if (modalImagePreview) { modalImagePreview.src = ""; modalImagePreview.style.display = "none"; }
        const modalPdfPreview = document.getElementById("modalPdfPreview");
        if (modalPdfPreview) modalPdfPreview.style.display = "none";
    },

    limparFormularioModalTalhao: function () {
        const form = document.getElementById("formCadastrarTalhao");
        if (form) {
            form.reset();
        }
        const modalClasseAD = document.getElementById("modalClasseAD");
        if (modalClasseAD) {
            modalClasseAD.textContent = "";
        }
        const modalMapScreenshot = document.getElementById("modalMapScreenshot");
        if (modalMapScreenshot) {
            modalMapScreenshot.style.display = "none";
        }
        const modalKmlOutput = document.getElementById("modalKmlOutput");
        if (modalKmlOutput) {
            modalKmlOutput.style.display = "none";
        }

        // Limpar mapa
        this.modalMap = null;
        this.modalSelectedShape = null;
    },

    montarEnderecoCompletoGerenciar: function () {
        const val = (id) => {
            const el = document.getElementById(id);
            return el && el.value ? String(el.value).trim() : '';
        };
        const partes = [
            val('modalPropriedadeEndereco'),
            val('modalPropriedadeNumero'),
            val('modalPropriedadeBairro'),
            val('modalPropriedadeCidade'),
            val('modalPropriedadeEstado'),
            val('modalPropriedadeCEP'),
        ].filter(Boolean);
        return partes.join(', ');
    },

    montarEnderecoCompletoCadastroPropriedade: function () {
        const val = (id) => {
            const el = document.getElementById(id);
            return el && el.value ? String(el.value).trim() : '';
        };
        const partes = [
            val('modalEndereco'),
            val('modalNumero'),
            val('modalBairro'),
            val('modalCidade'),
            val('modalEstado'),
            val('modalCEP'),
        ].filter(Boolean);
        return partes.join(', ');
    },

    // --- Validação de polígono no município (mesma regra que Views/Talhao/Cadastrar.cshtml) ---
    normalizarTextoModal: function (str) {
        if (!str) return "";
        return str.toLowerCase().trim()
            .normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    },

    obterMunicipioDeAddressComponentsModal: function (components) {
        let locality = "";
        let admin2 = "";
        for (let i = 0; i < components.length; i++) {
            const types = components[i].types;
            if (types.indexOf("locality") !== -1) locality = components[i].long_name;
            if (types.indexOf("administrative_area_level_2") !== -1) admin2 = components[i].long_name;
        }
        return locality || admin2 || "";
    },

    validarPoligonoNoMunicipioModal: function (polygon, callback) {
        const cidadePropriedade = (window.propriedadeCidade || "").trim();
        if (!cidadePropriedade) {
            callback(false, "", "Selecione uma propriedade antes de desenhar o polígono para validar o município.");
            return;
        }

        const path = polygon.getPath();
        const n = path.getLength();
        if (n < 3) {
            callback(false, "", "Polígono inválido.");
            return;
        }

        let totalLat = 0;
        let totalLng = 0;
        path.forEach((latLng) => {
            totalLat += latLng.lat();
            totalLng += latLng.lng();
        });
        const centerLat = totalLat / n;
        const centerLng = totalLng / n;

        const pontos = [
            new google.maps.LatLng(centerLat, centerLng),
            path.getAt(0),
            path.getAt(Math.min(Math.floor(n / 3), n - 1)),
            path.getAt(Math.min(Math.floor(2 * n / 3), n - 1)),
        ];

        const self = this;
        const normalizadoProp = self.normalizarTextoModal(cidadePropriedade);
        let pendentes = pontos.length;
        const municipiosEncontrados = [];
        let primeiroMunicipioDiferente = "";
        let algumPontoSemMunicipio = false;
        let qtdFalhasGeocode = 0;
        const geocoder = new google.maps.Geocoder();

        const processarResultado = (latLng, results, status) => {
            let municipio = "";
            if (status !== google.maps.GeocoderStatus.OK) {
                qtdFalhasGeocode++;
            } else if (results && results.length > 0) {
                municipio = self.obterMunicipioDeAddressComponentsModal(results[0].address_components);
            }
            municipiosEncontrados.push(municipio);
            const norm = self.normalizarTextoModal(municipio);
            if (!municipio || !norm) {
                algumPontoSemMunicipio = true;
            } else if (normalizadoProp !== norm) {
                primeiroMunicipioDiferente = primeiroMunicipioDiferente || municipio;
            }
            pendentes--;
            if (pendentes === 0) {
                if (qtdFalhasGeocode === pontos.length) {
                    callback(true, "", "");
                    return;
                }
                const algumDentro = municipiosEncontrados.some((m) => self.normalizarTextoModal(m) === normalizadoProp);
                const valido = !algumPontoSemMunicipio && !primeiroMunicipioDiferente && algumDentro;
                const msg = valido ? "" : (algumPontoSemMunicipio
                    ? "Não foi possível verificar o município em todos os pontos. Tente desenhar mais próximo da área da propriedade."
                    : "O polígono está fora do município da propriedade.");
                callback(valido, primeiroMunicipioDiferente || municipiosEncontrados[0] || "", msg);
            }
        };

        pontos.forEach((latLng) => {
            geocoder.geocode({ location: latLng, region: "br" }, (results, status) => {
                processarResultado(latLng, results, status);
            });
        });
    },

    /** Mesmo fluxo da tela Talhão/Cadastrar: busca a propriedade e monta o texto para geocode (endereço + CEP). */
    carregarPropriedadeParaModalTalhao: function (propriedadeId) {
        const self = this;
        const id = propriedadeId && String(propriedadeId).trim();
        if (!id || id === '00000000-0000-0000-0000-000000000000') {
            self.centralizarMapaModalNoEndereco();
            return;
        }
        fetch(`/api/propriedades/${id}`)
            .then((r) => r.json())
            .then((response) => {
                const ok = response.success === true;
                if (ok && response.data) {
                    const prop = response.data;
                    const cep = (prop.cep || prop.CEP || '').trim();
                    const endereco = (prop.endereco || prop.Endereco || '').trim();
                    window.propriedadeCidade = (prop.cidade || prop.Cidade || '').trim();
                    window.propriedadeEstado = (prop.estado || prop.Estado || '').trim();
                    const queryEnderecoCompleto = endereco + (cep ? ', ' + cep : '');
                    const el = document.getElementById('modalPropertyAddress');
                    if (el && queryEnderecoCompleto) {
                        el.value = queryEnderecoCompleto;
                    }
                }
                self.centralizarMapaModalNoEndereco();
            })
            .catch(() => {
                self.centralizarMapaModalNoEndereco();
            });
    },

    // Inicializar mapa do modal de talhão
    initializeModalMap: async function () {
        console.log("Inicializando mapa do modal...");

        if (!this.googleMapsLoaded) {
            console.error("Google Maps não está carregado");
            return;
        }

        const mapElement = document.getElementById("modalMap");
        if (!mapElement) {
            console.error("Elemento modalMap não encontrado");
            return;
        }

        console.log("Criando mapa...");
        const mapOptions = {
            zoom: 5,
            center: new google.maps.LatLng(-14.235004, -51.92528),
            mapTypeId: "satellite",
            mapTypeControl: true,
            mapTypeControlOptions: {
                style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
                position: google.maps.ControlPosition.TOP_RIGHT,
            },
        };

        this.modalMap = new google.maps.Map(mapElement, mapOptions);
        this.modalAddressMarker = null;

        this.modalDrawingManager = new google.maps.drawing.DrawingManager({
            drawingMode: google.maps.drawing.OverlayType.POLYGON,
            drawingControl: true,
            drawingControlOptions: {
                position: google.maps.ControlPosition.TOP_CENTER,
                drawingModes: ["polygon"],
            },
            polygonOptions: {
                editable: true,
                draggable: false,
            },
        });
        this.modalDrawingManager.setMap(this.modalMap);

        const self = this;
        google.maps.event.addListener(this.modalDrawingManager, "overlaycomplete", function (event) {
            if (self.modalSelectedShape) {
                self.modalSelectedShape.setMap(null);
            }

            self.modalSelectedShape = event.overlay;
            self.modalDrawingManager.setDrawingMode(null);

            self.validarPoligonoNoMunicipioModal(self.modalSelectedShape, function (valido, municipioEncontrado, mensagemErro) {
                if (!valido) {
                    self.modalSelectedShape.setMap(null);
                    self.modalSelectedShape = null;
                    let msg = mensagemErro || "O polígono está fora do município da propriedade.";
                    if (municipioEncontrado) {
                        msg += "\n\nMunicípio da propriedade: " + (window.propriedadeCidade || "") + "\nÁrea desenhada está em: " + municipioEncontrado;
                    }
                    msg += "\n\nDesenhe o polígono apenas dentro dos limites do município onde o cliente está cadastrado.";
                    alert(msg);
                    return;
                }
                self.processModalPolygon(self.modalSelectedShape);
            });
        });

        google.maps.event.addListener(this.modalDrawingManager, "overlaycomplete", function (event) {
            google.maps.event.addListener(event.overlay.getPath(), "set_at", () => self.processModalPolygon(self.modalSelectedShape));
            google.maps.event.addListener(event.overlay.getPath(), "insert_at", () => self.processModalPolygon(self.modalSelectedShape));
        });

        const runCentralizar = () => {
            if (self.modalMap) {
                google.maps.event.trigger(self.modalMap, "resize");
            }
            const pidEl = document.getElementById("modalPropriedadeId");
            const pid = pidEl && pidEl.value ? pidEl.value.trim() : "";
            if (pid) {
                self.carregarPropriedadeParaModalTalhao(pid);
            } else {
                self.centralizarMapaModalNoEndereco();
            }
        };
        requestAnimationFrame(() => requestAnimationFrame(runCentralizar));
    },

    obterEnderecoPropriedadeParaModal: function () {
        const modalPropertyAddressInput = document.getElementById("modalPropertyAddress");
        if (modalPropertyAddressInput && modalPropertyAddressInput.value && modalPropertyAddressInput.value.trim()) {
            return modalPropertyAddressInput.value.trim();
        }

        const fullGerenciar = this.montarEnderecoCompletoGerenciar();
        if (fullGerenciar) {
            return fullGerenciar;
        }

        const fullCadastro = this.montarEnderecoCompletoCadastroPropriedade();
        if (fullCadastro) {
            return fullCadastro;
        }

        const gerenciarEnderecoInput = document.getElementById("modalPropriedadeEndereco");
        if (gerenciarEnderecoInput && gerenciarEnderecoInput.value && gerenciarEnderecoInput.value.trim()) {
            return gerenciarEnderecoInput.value.trim();
        }

        const cadastrarEnderecoInput = document.getElementById("modalEndereco");
        if (cadastrarEnderecoInput && cadastrarEnderecoInput.value && cadastrarEnderecoInput.value.trim()) {
            return cadastrarEnderecoInput.value.trim();
        }

        const propriedadeId = document.getElementById("modalPropriedadeId") ? document.getElementById("modalPropriedadeId").value : "";
        if (propriedadeId) {
            const selectedRow = document.querySelector(`#tblPropriedadesSelecionadas tbody tr[data-id="${propriedadeId}"]`);
            if (selectedRow) {
                const rua = (selectedRow.dataset.endereco || "").trim();
                const cidade = (selectedRow.dataset.cidade || "").trim();
                const estado = (selectedRow.dataset.estado || "").trim();
                const composto = [rua, cidade, estado].filter(Boolean).join(", ");
                if (composto) {
                    return composto;
                }
            }
        }

        return "";
    },

    centralizarMapaModalNoEndereco: function () {
        if (!this.modalMap || !this.googleMapsLoaded) {
            return;
        }

        const address = this.obterEnderecoPropriedadeParaModal();
        if (!address) {
            return;
        }

        const modalPropertyAddressInput = document.getElementById("modalPropertyAddress");
        if (modalPropertyAddressInput) {
            modalPropertyAddressInput.value = address;
        }

        const geocoder = new google.maps.Geocoder();
        geocoder.geocode({ address: address, region: "br" }, (results, status) => {
            if (status !== google.maps.GeocoderStatus.OK || !results || !results.length) {
                return;
            }

            const result = results[0];
            const location = result.geometry.location;

            if (this.modalAddressMarker) {
                this.modalAddressMarker.setMap(null);
            }

            this.modalAddressMarker = new google.maps.Marker({
                position: location,
                map: this.modalMap,
                title: `Local: ${address}`,
                icon: {
                    url: "http://maps.google.com/mapfiles/ms/icons/red-dot.png"
                }
            });

            let zoomLevel = 15;
            const locationType = result.geometry.location_type || "";
            if (locationType === "ROOFTOP" || locationType === "RANGE_INTERPOLATED") zoomLevel = 17;
            else if (locationType === "GEOMETRIC_CENTER") zoomLevel = 14;
            else if (locationType === "APPROXIMATE") zoomLevel = 13;

            this.modalMap.setCenter(location);
            this.modalMap.setZoom(zoomLevel);
            google.maps.event.trigger(this.modalMap, "resize");

            if (result.geometry.viewport) {
                this.modalMap.fitBounds(result.geometry.viewport);
                google.maps.event.addListenerOnce(this.modalMap, "idle", () => {
                    const z = this.modalMap.getZoom();
                    if (z < 13) {
                        this.modalMap.setCenter(location);
                        this.modalMap.setZoom(zoomLevel);
                    } else if (z > 18) {
                        this.modalMap.setZoom(18);
                    }
                });
            }
        });
    },

    processModalPolygon: function (polygon) {
        const path = polygon.getPath();
        const bounds = new google.maps.LatLngBounds();
        let totalLat = 0;
        let totalLng = 0;

        path.forEach(latLng => {
            bounds.extend(latLng);
            totalLat += latLng.lat();
            totalLng += latLng.lng();
        });

        // Calcula a área em hectares
        const area = google.maps.geometry.spherical.computeArea(path) / 10000;
        document.getElementById("modalArea").value = area.toFixed(2);

        // Calcula a latitude e longitude médias
        const centerLat = totalLat / path.getLength();
        const centerLng = totalLng / path.getLength();
        document.getElementById("modalLatitude").value = centerLat.toFixed(6);
        document.getElementById("modalLongitude").value = centerLng.toFixed(6);

        // Ajusta o mapa para o polígono
        this.modalMap.fitBounds(bounds);

        // Gera o roteiro de acesso
        this.generateModalRoute(centerLat, centerLng);
    },

    generateModalRoute: function (lat, lng) {
        const address = document.getElementById("modalPropertyAddress") ?
            document.getElementById("modalPropertyAddress").value.trim() : "";

        if (!address) {
            document.getElementById("modalRoteiroAcesso").value = "Endereço de origem não fornecido.";
            // Mesmo sem endereço, gerar KML e imagem
            setTimeout(() => {
                this.capturarModalImagemMapa();
                this.gerarModalKmlMapa();
            }, 1000);
            return;
        }

        const directionsService = new google.maps.DirectionsService();
        const self = this;
        directionsService.route(
            {
                origin: address,
                destination: { lat, lng },
                travelMode: google.maps.TravelMode.DRIVING,
            },
            function (response, status) {
                if (status === google.maps.DirectionsStatus.OK) {
                    const steps = response.routes[0].legs[0].steps.map(step => {
                        const instruction = self.stripHtml(step.instructions);
                        return instruction.endsWith('.') || instruction.endsWith(',')
                            ? instruction
                            : instruction + '.';
                    });

                    document.getElementById("modalRoteiroAcesso").value = steps.join("\n");
                } else {
                    document.getElementById("modalRoteiroAcesso").value = "Não foi possível gerar o roteiro de acesso.";
                }
            }
        );

        setTimeout(() => {
            this.capturarModalImagemMapa();
            this.gerarModalKmlMapa();
        }, 1000);
    },

    stripHtml: function (html) {
        const div = document.createElement("div");
        div.innerHTML = html;
        return div.textContent || div.innerText || "";
    },

    capturarModalImagemMapa: function () {
        if (!this.modalMap || !this.modalSelectedShape) {
            return;
        }

        const bounds = new google.maps.LatLngBounds();
        const path = this.modalSelectedShape.getPath();

        path.forEach(latLng => {
            bounds.extend(latLng);
        });

        const paddingDegrees = 0.002;
        const northEast = bounds.getNorthEast();
        const southWest = bounds.getSouthWest();

        const paddedBounds = new google.maps.LatLngBounds(
            new google.maps.LatLng(southWest.lat() - paddingDegrees, southWest.lng() - paddingDegrees),
            new google.maps.LatLng(northEast.lat() + paddingDegrees, northEast.lng() + paddingDegrees)
        );

        this.modalMap.fitBounds(paddedBounds);

        const self = this;
        html2canvas(document.getElementById("modalMap"), {
            useCORS: true,
            scale: 2,
        }).then(function (canvas) {
            const image = canvas.toDataURL("image/png");
            const screenshot = document.getElementById("modalMapScreenshot");
            screenshot.src = image;
            screenshot.style.display = "block";
        }).catch(function (error) {
            console.error("Erro ao capturar a imagem:", error);
        });
    },

    gerarModalKmlMapa: function () {
        console.log("Gerando KML do modal...");
        if (!this.modalSelectedShape) {
            console.log("Nenhum polígono selecionado");
            return;
        }

        const path = this.modalSelectedShape.getPath();
        let kml = `<?xml version="1.0" encoding="UTF-8"?>\n`;
        kml += `<kml xmlns="http://www.opengis.net/kml/2.2">\n`;
        kml += `  <Document>\n`;
        kml += `    <Placemark>\n`;
        kml += `      <Polygon>\n`;
        kml += `        <outerBoundaryIs>\n`;
        kml += `          <LinearRing>\n`;
        kml += `            <coordinates>\n`;

        path.forEach(function (latLng) {
            kml += `              ${latLng.lng()},${latLng.lat()},0\n`;
        });

        kml += `            </coordinates>\n`;
        kml += `          </LinearRing>\n`;
        kml += `        </outerBoundaryIs>\n`;
        kml += `      </Polygon>\n`;
        kml += `    </Placemark>\n`;
        kml += `  </Document>\n`;
        kml += `</kml>`;

        const kmlOutput = document.getElementById("modalKmlOutput");
        kmlOutput.style.display = "block";
        kmlOutput.value = kml;
    },

    configurarAnaliseSolo: function () {
        const modalCheckbox = document.getElementById("modalPossuiAnaliseFisicaSolo");
        const modalCamposPercentual = [
            document.getElementById("modalPercentualAreia"),
            document.getElementById("modalPercentualSilte"),
            document.getElementById("modalPercentualArgila")
        ];

        if (modalCheckbox) {
            const alternarModalCampos = () => {
                const habilitar = modalCheckbox.checked;
                modalCamposPercentual.forEach(campo => {
                    if (campo) campo.disabled = !habilitar;
                });
            };

            modalCheckbox.addEventListener("change", alternarModalCampos);
            alternarModalCampos();
        }

        // Cálculo de AD no modal de talhão
        const modalAreiaInput = document.getElementById("modalPercentualAreia");
        const modalSilteInput = document.getElementById("modalPercentualSilte");
        const modalArgilaInput = document.getElementById("modalPercentualArgila");
        const modalClasseADElement = document.getElementById("modalClasseAD");
        const modalTipoSoloSelect = document.getElementById("modalTipoSolo");
        const modalClassificacaoSoloADSelect = document.getElementById("modalClassificacaoSoloAD");

        const rangesAD = [
            { min: 0, max: 0.33, classe: "AD 1" },
            { min: 0.34, max: 0.46, classe: "AD 2" },
            { min: 0.46, max: 0.61, classe: "AD 3" },
            { min: 0.61, max: 0.80, classe: "AD 4" },
            { min: 0.80, max: 1.06, classe: "AD 5" },
            { min: 1.06, max: Infinity, classe: "AD 6" }
        ];

        const self = this;
        const calcularModalAD = () => {
            const areia = parseFloat(modalAreiaInput.value.replace(",", ".")) || 0;
            const silte = parseFloat(modalSilteInput.value.replace(",", ".")) || 0;
            const argila = parseFloat(modalArgilaInput.value.replace(",", ".")) || 0;

            const somaTotal = areia + silte + argila;
            if (somaTotal !== 100) {
                modalClasseADElement.textContent = "A soma dos percentuais deve ser 100%.";
                modalClasseADElement.className = "text-danger";
                modalClassificacaoSoloADSelect.value = "";
                modalTipoSoloSelect.value = "";
                return;
            }

            const part1 = (0.3591 * (
                (-0.02128887 * areia) +
                (-0.01005814 * silte) +
                (-0.01901894 * argila) +
                (0.0001171219 * areia * silte) +
                (0.0002073924 * areia * argila) +
                (0.00006118707 * silte * argila) +
                (-0.000006373789 * areia * silte * argila)
            ));
            const part2 = 1 + part1;
            const part3 = part2 ** 2.78474;
            const ad = (part3 * 10).toFixed(2);

            const classificacao = rangesAD.find(range => ad >= range.min && ad < range.max);
            modalClassificacaoSoloADSelect.value = classificacao ? classificacao.classe : "";

            const diferencaAreiaArgila = areia - argila;
            if (argila >= 10 && argila < 15 && diferencaAreiaArgila >= 50) {
                modalTipoSoloSelect.value = "Tipo 1";
            } else if (argila >= 15 && argila < 35 && diferencaAreiaArgila < 50) {
                modalTipoSoloSelect.value = "Tipo 2";
            } else if (argila >= 35) {
                modalTipoSoloSelect.value = "Tipo 3";
            } else {
                modalTipoSoloSelect.value = "";
            }

            modalClasseADElement.textContent = `AD: ${ad}`;
        };

        [modalAreiaInput, modalSilteInput, modalArgilaInput].forEach(input => {
            if (input) {
                input.addEventListener("input", calcularModalAD);
                input.addEventListener("blur", calcularModalAD);
            }
        });
    },

    salvarPropriedade: function () {

        const btnSalvarPropriedade = document.getElementById("btnSalvarPropriedade");

        const nome = document.getElementById("modalNome").value.trim();
        const area = document.getElementById("modalSomaAreaTotalTalhao").value.trim();
        const estado = document.getElementById("modalEstado").value;
        const cidade = document.getElementById("modalCidade").value;

        if (!nome) { Swal.fire({ title: 'Atenção!', text: 'Preencha o campo Nome.', icon: 'warning' }); return; }
        if (!area) { Swal.fire({ title: 'Atenção!', text: 'Preencha o campo Área total da propriedade.', icon: 'warning' }); return; }
        if (!estado) { Swal.fire({ title: 'Atenção!', text: 'Selecione o Estado.', icon: 'warning' }); return; }
        if (!cidade) { Swal.fire({ title: 'Atenção!', text: 'Selecione a Cidade.', icon: 'warning' }); return; }

        const formData = new FormData();

        // Dados da propriedade
        formData.append("Nome", document.getElementById("modalNome").value);
        formData.append("SomaAreaTotalTalhao", document.getElementById("modalSomaAreaTotalTalhao").value);
        formData.append("MatriculaLote", document.getElementById("modalMatriculaLote").value);
        formData.append("CadastroAmbientalRural", document.getElementById("modalCadastroAmbientalRural").value);
        formData.append("CEP", document.getElementById("modalCEP").value);
        formData.append("Endereco", document.getElementById("modalEndereco").value);
        formData.append("Bairro", document.getElementById("modalBairro").value);
        formData.append("Numero", document.getElementById("modalNumero").value);
        formData.append("Cidade", document.getElementById("modalCidade").value);
        formData.append("Estado", document.getElementById("modalEstado").value);

        // Imagem geral dos talhões é opcional (campo removido do modal)
        const modalImageUpload = document.getElementById("modalImageUpload");
        if (modalImageUpload && modalImageUpload.files && modalImageUpload.files[0]) {
            formData.append("ImagemGeralTodosTalhoes", modalImageUpload.files[0]);
        }

        // Desabilitar botão durante o envio
        btnSalvarPropriedade.disabled = true;
        btnSalvarPropriedade.textContent = "Salvando...";

        const self = this;
        fetch("/Cliente/CadastrarPropriedade", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    self.propriedadeIdCriada = data.propriedadeId;

                    // ADICIONAR ESTA LINHA: Definir o ID da propriedade no modal de talhão
                    document.getElementById("modalPropriedadeId").value = data.propriedadeId;
                    var modalPropertyAddressInput = document.getElementById("modalPropertyAddress");
                    if (modalPropertyAddressInput) {
                        modalPropertyAddressInput.value = self.montarEnderecoCompletoCadastroPropriedade() ||
                            (document.getElementById("modalEndereco").value || "");
                    }

                    // Adicionar a nova propriedade à lista de propriedades selecionadas
                    const novaPropriedade = {
                        id: data.propriedadeId,
                        nome: document.getElementById("modalNome").value,
                        endereco: document.getElementById("modalEndereco").value,
                        cidade: document.getElementById("modalCidade").value,
                        estado: document.getElementById("modalEstado").value
                    };

                    // Adicionar à tabela de propriedades selecionadas
                    const selectedItems = new Set(JSON.parse(document.getElementById("selectedItems").value || "[]").map(item => item.id));
                    if (!selectedItems.has(novaPropriedade.id)) {
                        selectedItems.add(novaPropriedade.id);

                        const newRow = document.createElement("tr");
                        newRow.dataset.id = novaPropriedade.id;
                        newRow.dataset.nome = novaPropriedade.nome;
                        newRow.dataset.cidade = novaPropriedade.cidade;
                        newRow.dataset.estado = novaPropriedade.estado;
                        newRow.dataset.endereco = novaPropriedade.endereco;

                        newRow.innerHTML = `
                        <td>${novaPropriedade.nome}</td>
                        <td>${novaPropriedade.cidade}/${novaPropriedade.estado}</td>
                        <td>
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-info btn-sm" onclick="clienteJS.abrirModalGerenciarPropriedade('${novaPropriedade.id}')" title="Gerenciar Propriedade">
                                    <i class="bi bi-gear"></i> Gerenciar
                                </button>
                                <button type="button" class="btn btn-danger btn-sm remove-from-list">Remover</button>
                            </div>
                        </td>
                    `;
                        document.querySelector("#tblPropriedadesSelecionadas tbody").appendChild(newRow);

                        // Atualizar o campo oculto
                        const itemsArray = Array.from(selectedItems).map(id => {
                            const row = document.querySelector(`#tblPropriedadesSelecionadas tbody tr[data-id="${id}"]`);
                            return {
                                id: id,
                                nome: row ? row.dataset.nome : "",
                                endereco: row ? row.dataset.endereco : "",
                                cidade: row ? row.dataset.cidade : "",
                                estado: row ? row.dataset.estado : ""
                            };
                        });
                        document.getElementById("selectedItems").value = JSON.stringify(itemsArray);

                        // Configurar botões de remoção
                        self.configureRemoveButtons();
                    }
                    notify.success("Propriedade Cadastrada", data.message || "Propriedade cadastrada com sucesso!", function (d) {
                        self.fecharModalPropriedade();

                        setTimeout(async () => {
                            const modalTalhaoElement = document.getElementById("modalCadastrarTalhao");
                            if (typeof $ !== 'undefined' && $.fn.modal) {
                                $(modalTalhaoElement).modal('show');
                            } else if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
                                const modal = new bootstrap.Modal(modalTalhaoElement);
                                modal.show();
                            }

                            await new Promise(resolve => setTimeout(resolve, 500));

                            await self.initializeModalMap();
                        }, 300);
                    });
                } else {
                    notify.error("Erro ao Cadastrar Propriedade", data.message || "Ocorreu um erro ao cadastrar a propriedade.");
                }
            })
            .catch(error => {
                console.error("Erro:", error);
                notify.error("Erro ao Cadastrar Propriedade", "Erro ao cadastrar propriedade. Tente novamente.");
            })
            .finally(() => {
                // Reabilitar botão
                btnSalvarPropriedade.disabled = false;
                btnSalvarPropriedade.textContent = "Salvar Propriedade";
            });
    },

    salvarTalhao: function () {

        const btnSalvarTalhao = document.getElementById("btnSalvarTalhao");

        // Obter o ID da propriedade do campo hidden
        const propriedadeId = document.getElementById("modalPropriedadeId").value;

        if (!propriedadeId) {
            notify.error("Erro", "ID da propriedade não encontrado.");
            return;
        }

        const formData = new FormData();

        // Dados do talhão
        formData.append("Nome", document.getElementById("modalTalhaoNome").value);
        formData.append("PropriedadeId", propriedadeId);
        formData.append("PossuiAnaliseFisicaSolo", document.getElementById("modalPossuiAnaliseFisicaSolo").checked);
        formData.append("PercentualAreia", document.getElementById("modalPercentualAreia").value);
        formData.append("PercentualSilte", document.getElementById("modalPercentualSilte").value);
        formData.append("PercentualArgila", document.getElementById("modalPercentualArgila").value);
        formData.append("TipoSolo", document.getElementById("modalTipoSolo").value);
        formData.append("ClassificacaoSolo", document.getElementById("modalClassificacaoSoloAD").value);
        formData.append("Latitude", document.getElementById("modalLatitude").value);
        formData.append("Longitude", document.getElementById("modalLongitude").value);
        formData.append("Area", document.getElementById("modalArea").value);
        formData.append("RoteiroAcesso", document.getElementById("modalRoteiroAcesso").value);
        formData.append("ImagemTalhao", document.getElementById("modalMapScreenshot").src);
        formData.append("KmlTalhao", document.getElementById("modalKmlOutput").value);

        // Desabilitar botão durante o envio
        btnSalvarTalhao.disabled = true;
        btnSalvarTalhao.textContent = "Salvando...";

        const self = this;
        fetch("/Cliente/CadastrarTalhao", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    notify.success("Talhão Cadastrado", data.message || "Talhão cadastrado com sucesso!", function (d) {
                        self.fecharModalTalhao();
                        self.limparFormularioModalTalhao();
                    });
                } else {
                    notify.error("Erro ao Cadastrar Talhão", data.message || "Ocorreu um erro ao cadastrar o talhão.");
                }
            })
            .catch(error => {
                console.error("Erro:", error);
                notify.error("Erro ao Cadastrar Talhão", "Erro ao cadastrar talhão. Tente novamente.");
            })
            .finally(() => {
                // Reabilitar botão
                btnSalvarTalhao.disabled = false;
                btnSalvarTalhao.textContent = "Salvar Talhão";
            });
    },

    // Funções para o modal de gerenciar propriedades
    abrirModalGerenciarPropriedade: function (propriedadeId) {
        // Carregar dados da propriedade via AJAX
        fetch(`/Cliente/ObterPropriedadeParaGerenciar?id=${propriedadeId}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.preencherModalPropriedade(data.propriedade);
                    this.carregarTalhoesPropriedade(propriedadeId);

                    // Abrir modal
                    const modal = new bootstrap.Modal(document.getElementById('modalGerenciarPropriedade'));
                    modal.show();
                } else {
                    notify.error("Erro", data.message || "Erro ao carregar dados da propriedade.");
                }
            })
            .catch(error => {
                console.error("Erro:", error);
                notify.error("Erro", "Erro ao carregar dados da propriedade.");
            });
    },

    preencherModalPropriedade: function (propriedade) {
        // Preencher campos do formulário
        document.getElementById('modalPropriedadeId').value = propriedade.id;
        document.getElementById('modalPropriedadeNome').value = propriedade.nome || '';
        document.getElementById('modalPropriedadeSomaAreaTotalTalhao').value = propriedade.somaAreaTotalTalhao || '';
        document.getElementById('modalPropriedadeMatriculaLote').value = propriedade.matriculaLote || '';
        document.getElementById('modalPropriedadeCadastroAmbientalRural').value = propriedade.cadastroAmbientalRural || '';
        document.getElementById('modalPropriedadeCEP').value = propriedade.cep || '';
        document.getElementById('modalPropriedadeEndereco').value = propriedade.endereco || '';
        document.getElementById('modalPropriedadeBairro').value = propriedade.bairro || '';
        document.getElementById('modalPropriedadeNumero').value = propriedade.numero || '';
        document.getElementById('modalPropriedadeCidade').value = propriedade.cidade || '';
        document.getElementById('modalPropriedadeEstado').value = propriedade.estado || '';
        document.getElementById('modalPropriedadeAtivo').value = propriedade.ativo ? "true" : "false";
        document.getElementById('modalPropriedadeExcluido').value = propriedade.excluido ? "true" : "false";
        var modalPropertyAddressInput = document.getElementById("modalPropertyAddress");
        if (modalPropertyAddressInput) {
            modalPropertyAddressInput.value = this.montarEnderecoCompletoGerenciar() || (propriedade.endereco || "");
        }

        // Configurar imagem ou PDF
        if (propriedade.imagemGeralTodosTalhoes) {
            const imagePreview = document.getElementById('modalPropriedadeImagePreview');
            const pdfPreview = document.getElementById('modalPropriedadePdfPreview');
            const imageInfo = document.getElementById('modalPropriedadeImageInfo');
            const removeButton = document.getElementById('modalPropriedadeRemoveButton');
            const imageLabel = document.getElementById('modalPropriedadeImageLabel');

            let imageSrc = propriedade.imagemGeralTodosTalhoes;
            const isPdf = imageSrc.startsWith('data:application/pdf');
            if (!isPdf && !imageSrc.startsWith('data:image/')) {
                imageSrc = 'data:image/jpeg;base64,' + imageSrc;
            }

            document.getElementById('modalPropriedadeObjectImagem').value = imageSrc;
            imageInfo.style.display = 'block';
            removeButton.style.display = 'block';
            imageLabel.textContent = isPdf ? 'Documento atual da propriedade:' : 'Imagem atual da propriedade:';
            if (isPdf) {
                if (imagePreview) { imagePreview.src = ''; imagePreview.style.display = 'none'; }
                if (pdfPreview) pdfPreview.style.display = 'block';
            } else {
                if (pdfPreview) pdfPreview.style.display = 'none';
                if (imagePreview) { imagePreview.src = imageSrc; imagePreview.style.display = 'block'; }
            }
        } else {
            document.getElementById('modalPropriedadeImagePreview').style.display = 'none';
            const mpPdf = document.getElementById('modalPropriedadePdfPreview');
            if (mpPdf) mpPdf.style.display = 'none';
            document.getElementById('modalPropriedadeImageInfo').style.display = 'none';
            document.getElementById('modalPropriedadeRemoveButton').style.display = 'none';
            document.getElementById('modalPropriedadeImageLabel').textContent = 'Selecione uma imagem ou PDF:';
            document.getElementById('modalPropriedadeObjectImagem').value = '';
        }
    },

    carregarTalhoesPropriedade: function (propriedadeId) {
        fetch(`/Cliente/ObterTalhoesPropriedade?id=${propriedadeId}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.exibirTalhoesPropriedade(data.talhoes);
                    this.configurarControlesStatus(data.talhoes.length);
                } else {
                    console.error("Erro ao carregar talhões:", data.message);
                    this.exibirTalhoesPropriedade([]);
                    this.configurarControlesStatus(0);
                }
            })
            .catch(error => {
                console.error("Erro:", error);
                this.exibirTalhoesPropriedade([]);
                this.configurarControlesStatus(0);
            });
    },

    exibirTalhoesPropriedade: function (talhoes) {
        const container = document.getElementById('modalPropriedadeTalhoesContainer');

        if (!talhoes || talhoes.length === 0) {
            container.innerHTML = `
                <div class="text-center py-4">
                    <i class="bi bi-geo-alt" style="font-size: 3rem; color: var(--color-primary);"></i>
                    <h5 class="mt-3" style="color: var(--color-primary);">Nenhum Talhão Vinculado</h5>
                    <p class="text-muted">Esta propriedade ainda não possui talhões cadastrados.</p>
                    <div class="mt-3">
                        <button type="button" 
                                class="btn btn-success" 
                                onclick="clienteJS.abrirModalCadastrarTalhaoPropriedade('${document.getElementById('modalPropriedadeId').value}')"
                                title="Adicionar Novo Talhão">
                            <i class="bi bi-plus-circle"></i> Adicionar Novo Talhão
                        </button>
                    </div>
                </div>
            `;
            return;
        }

        const talhoesAtivos = talhoes.filter(t => !t.excluido);

        if (talhoesAtivos.length === 0) {
            container.innerHTML = `
                <div class="text-center py-4">
                    <i class="bi bi-geo-alt" style="font-size: 3rem; color: var(--color-primary);"></i>
                    <h5 class="mt-3" style="color: var(--color-primary);">Nenhum Talhão Ativo</h5>
                    <p class="text-muted">Todos os talhões desta propriedade estão inativos.</p>
                    <div class="mt-3">
                        <button type="button" 
                                class="btn btn-success" 
                                onclick="clienteJS.abrirModalCadastrarTalhaoPropriedade('${document.getElementById('modalPropriedadeId').value}')"
                                title="Adicionar Novo Talhão">
                            <i class="bi bi-plus-circle"></i> Adicionar Novo Talhão
                        </button>
                    </div>
                </div>
            `;
            return;
        }

        let html = `
            <div class="table-responsive">
                <table class="table table-striped table-hover">
                    <thead class="table-primary">
                        <tr>
                            <th>Nome</th>
                            <th>Área (ha)</th>
                            <th>Tipo de Solo</th>
                            <th>Classificação</th>
                            <th>Análise Física</th>
                            <th>Status</th>
                            <th>Ações</th>
                        </tr>
                    </thead>
                    <tbody>
        `;

        talhoesAtivos.forEach(talhao => {
            html += `
                <tr>
                    <td>
                        <strong>${talhao.nome}</strong>
                        ${talhao.roteiroAcesso ? `<br><small class="text-muted"><i class="bi bi-geo-alt"></i> ${talhao.roteiroAcesso}</small>` : ''}
                    </td>
                    <td>
                        <span class="badge bg-info">${parseFloat(talhao.area).toFixed(2)}</span>
                    </td>
                    <td>
                        ${talhao.tipoSolo ? `<span class="badge bg-secondary">${talhao.tipoSolo}</span>` : '<span class="text-muted">-</span>'}
                    </td>
                    <td>
                        ${talhao.classificacaoSolo ? `<span class="badge bg-warning text-dark">${talhao.classificacaoSolo}</span>` : '<span class="text-muted">-</span>'}
                    </td>
                    <td>
                        ${talhao.possuiAnaliseFisicaSolo ?
                    `<span class="badge bg-success"><i class="bi bi-check-circle"></i> Sim</span><br><small class="text-muted">Areia: ${talhao.percentualAreia}% | Silte: ${talhao.percentualSilte}% | Argila: ${talhao.percentualArgila}%</small>` :
                    '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> Não</span>'}
                    </td>
                    <td>
                        ${talhao.ativo ?
                    '<span class="badge bg-success"><i class="bi bi-check-circle"></i> Ativo</span>' :
                    '<span class="badge bg-danger"><i class="bi bi-x-circle"></i> Inativo</span>'}
                    </td>
                    <td>
                        <div class="btn-group" role="group">
                            <a href="/Talhao/Gerenciar/${talhao.id}" class="btn btn-sm btn-outline-primary" title="Gerenciar Talhão" target="_blank">
                                <i class="bi bi-gear"></i>
                            </a>
                            ${talhao.imagemTalhao ?
                    `<button type="button" class="btn btn-sm btn-outline-info" onclick="clienteJS.visualizarImagemTalhaoPropriedade('${talhao.imagemTalhao}', '${talhao.nome}')" title="Visualizar Imagem">
                                    <i class="bi bi-image"></i>
                                </button>` : ''}
                        </div>
                    </td>
                </tr>
            `;
        });

        html += `
                    </tbody>
                </table>
            </div>
            
            <div class="row mt-3">
                <div class="col-md-6">
                    <div class="alert alert-info">
                        <i class="bi bi-info-circle"></i>
                        <strong>Total de Talhões:</strong> ${talhoesAtivos.length}
                    </div>
                </div>
                <div class="col-md-6 text-end">
                    <button type="button" 
                            class="btn btn-sm btn-success" 
                            onclick="clienteJS.abrirModalCadastrarTalhaoPropriedade('${document.getElementById('modalPropriedadeId').value}')"
                            title="Adicionar Novo Talhão">
                        <i class="bi bi-plus"></i> Adicionar Novo Talhão
                    </button>
                </div>
            </div>
        `;

        container.innerHTML = html;
    },

    configurarControlesStatus: function (quantidadeTalhoes) {
        const container = document.getElementById('modalPropriedadeStatusContainer');
        const ativo = document.getElementById('modalPropriedadeAtivo').value === 'true';
        const excluido = document.getElementById('modalPropriedadeExcluido').value === 'true';

        if (quantidadeTalhoes > 0) {
            // Se há talhões vinculados, usar campos hidden para preservar os valores
            container.innerHTML = `
                <input type="hidden" id="modalPropriedadeAtivoHidden" value="${ativo}" />
                <input type="hidden" id="modalPropriedadeExcluidoHidden" value="${excluido}" />
            `;
        } else {
            // Se não há talhões, mostrar os switches de controle
            container.innerHTML = `
                <div class="form-check form-switch">
                    <input class="form-check-input" type="checkbox" id="modalPropriedadeExcluidoSwitch" ${excluido ? 'checked' : ''}>
                    <label class="form-check-label" for="modalPropriedadeExcluidoSwitch">
                        Marque essa opção para <span class="badge bg-danger">excluir</span> a propriedade.
                    </label>
                </div>
                <div class="form-check form-switch">
                    <input class="form-check-input" type="checkbox" id="modalPropriedadeAtivoSwitch" ${ativo ? 'checked' : ''}>
                    <label class="form-check-label" for="modalPropriedadeAtivoSwitch">
                        Desmarque essa opção para <span class="badge bg-warning">inativar</span> a propriedade.
                    </label>
                </div>
            `;
        }
    },

    visualizarImagemTalhaoPropriedade: function (imagemBase64, nomeTalhao) {
        if (imagemBase64 && imagemBase64.length > 0) {
            let imagemSrc = imagemBase64;
            if (!imagemBase64.startsWith('data:image/')) {
                imagemSrc = 'data:image/jpeg;base64,' + imagemBase64;
            }

            document.getElementById('imagemTalhaoPropriedadeModal').src = imagemSrc;
            document.getElementById('modalImagemTalhaoPropriedadeLabel').textContent = 'Imagem do Talhão: ' + nomeTalhao;

            const modal = new bootstrap.Modal(document.getElementById('modalImagemTalhaoPropriedade'));
            modal.show();
        } else {
            alert('Este talhão não possui imagem cadastrada.');
        }
    },

    salvarPropriedadeModal: function () {
        const btnSalvar = document.getElementById('btnSalvarPropriedadeModal');
        const propriedadeId = document.getElementById('modalPropriedadeId').value;

        if (!propriedadeId) {
            notify.error("Erro", "ID da propriedade não encontrado.");
            return;
        }

        const formData = new FormData();

        // Dados da propriedade
        formData.append("Id", propriedadeId);
        formData.append("Nome", document.getElementById("modalPropriedadeNome").value);
        formData.append("SomaAreaTotalTalhao", document.getElementById("modalPropriedadeSomaAreaTotalTalhao").value);
        formData.append("MatriculaLote", document.getElementById("modalPropriedadeMatriculaLote").value);
        formData.append("CadastroAmbientalRural", document.getElementById("modalPropriedadeCadastroAmbientalRural").value);
        formData.append("CEP", document.getElementById("modalPropriedadeCEP").value);
        formData.append("Endereco", document.getElementById("modalPropriedadeEndereco").value);
        formData.append("Bairro", document.getElementById("modalPropriedadeBairro").value);
        formData.append("Numero", document.getElementById("modalPropriedadeNumero").value);
        formData.append("Cidade", document.getElementById("modalPropriedadeCidade").value);
        formData.append("Estado", document.getElementById("modalPropriedadeEstado").value);
        formData.append("ImagemGeralTodosTalhoes", document.getElementById("modalPropriedadeObjectImagem").value);

        // Verificar se há talhões vinculados para determinar os valores de status
        const quantidadeTalhoes = document.querySelectorAll('#modalPropriedadeTalhoesContainer tbody tr').length;

        if (quantidadeTalhoes > 0) {
            // Se há talhões, usar os valores dos campos hidden
            formData.append("Ativo", document.getElementById("modalPropriedadeAtivoHidden").value);
            formData.append("Excluido", document.getElementById("modalPropriedadeExcluidoHidden").value);
        } else {
            // Se não há talhões, usar os valores dos switches
            formData.append("Ativo", document.getElementById("modalPropriedadeAtivoSwitch").checked ? "true" : "false");
            formData.append("Excluido", document.getElementById("modalPropriedadeExcluidoSwitch").checked ? "true" : "false");
        }

        // Desabilitar botão durante o envio
        btnSalvar.disabled = true;
        btnSalvar.textContent = "Salvando...";

        fetch("/Cliente/AtualizarPropriedade", {
            method: "POST",
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    notify.success("Propriedade Atualizada", data.message || "Propriedade atualizada com sucesso!", function (d) {
                        // Fechar modal
                        bootstrap.Modal.getInstance(document.getElementById('modalGerenciarPropriedade')).hide();
                    });
                } else {
                    notify.error("Erro ao Atualizar Propriedade", data.message || "Ocorreu um erro ao atualizar a propriedade.");
                }
            })
            .catch(error => {
                console.error("Erro:", error);
                notify.error("Erro ao Atualizar Propriedade", "Erro ao atualizar propriedade. Tente novamente.");
            })
            .finally(() => {
                // Reabilitar botão
                btnSalvar.disabled = false;
                btnSalvar.textContent = "Salvar Alterações";
            });
    },

    removerImagemPropriedade: function () {
        if (confirm('Tem certeza que deseja remover a imagem/documento atual?')) {
            document.getElementById('modalPropriedadeImagePreview').src = '';
            document.getElementById('modalPropriedadeImagePreview').style.display = 'none';
            const mpPdf = document.getElementById('modalPropriedadePdfPreview');
            if (mpPdf) mpPdf.style.display = 'none';
            document.getElementById('modalPropriedadeObjectImagem').value = '';
            document.getElementById('modalPropriedadeImageUpload').value = '';
            document.getElementById('modalPropriedadeImageInfo').style.display = 'none';
            document.getElementById('modalPropriedadeRemoveButton').style.display = 'none';
            document.getElementById('modalPropriedadeImageLabel').textContent = 'Selecione uma imagem ou PDF:';
        }
    },

    abrirModalCadastrarTalhaoPropriedade: function (propriedadeId) {
        // Limpar formulário
        document.getElementById('formCadastrarTalhao').reset();

        // Definir a propriedade selecionada
        document.getElementById('modalPropriedadeId').value = propriedadeId;

        // Propagar cidade/estado para validação do polígono
        window.propriedadeCidade = (document.getElementById('modalPropriedadeCidade')?.value || '').trim();
        window.propriedadeEstado = (document.getElementById('modalPropriedadeEstado')?.value || '').trim();

        var modalPropertyAddressInput = document.getElementById("modalPropertyAddress");
        if (modalPropertyAddressInput) {
            const completo = this.montarEnderecoCompletoGerenciar();
            const fallback = document.getElementById("modalPropriedadeEndereco");
            modalPropertyAddressInput.value = completo || (fallback && fallback.value ? fallback.value.trim() : "");
        }

        // Armazenar o ID da propriedade para reabrir o modal depois
        this.propriedadeIdParaReabrir = propriedadeId;

        // Fechar o modal de gerenciar propriedade primeiro
        bootstrap.Modal.getInstance(document.getElementById('modalGerenciarPropriedade')).hide();

        // Aguardar um pouco e abrir o modal de talhão
        setTimeout(() => {
            var modal = new bootstrap.Modal(document.getElementById('modalCadastrarTalhao'));
            modal.show();

            // Aguardar o modal estar completamente aberto antes de inicializar o mapa
            document.getElementById('modalCadastrarTalhao').addEventListener('shown.bs.modal', function () {
                clienteJS.initializeModalMap();
            }, { once: true }); // Executar apenas uma vez
        }, 300);
    },

    reabrirModalPropriedade: function () {
        if (this.propriedadeIdParaReabrir) {
            setTimeout(() => {
                this.abrirModalGerenciarPropriedade(this.propriedadeIdParaReabrir);
                this.propriedadeIdParaReabrir = null;
            }, 500);
        }
    },

    initModalGerenciarPropriedade: function () {
        // Event listener para o botão salvar do modal
        const btnSalvarPropriedadeModal = document.getElementById('btnSalvarPropriedadeModal');
        if (btnSalvarPropriedadeModal) {
            btnSalvarPropriedadeModal.addEventListener('click', function () {
                clienteJS.salvarPropriedadeModal();
            });
        }

        // Event listener para upload de imagem ou PDF no modal
        const modalPropriedadeImageUpload = document.getElementById('modalPropriedadeImageUpload');
        if (modalPropriedadeImageUpload) {
            modalPropriedadeImageUpload.addEventListener('change', function () {
                const file = this.files[0];
                if (file) {
                    const reader = new FileReader();
                    reader.onload = function (event) {
                        const dataUrl = event.target.result;
                        document.getElementById('modalPropriedadeObjectImagem').value = dataUrl;
                        document.getElementById('modalPropriedadeImageInfo').style.display = 'none';
                        document.getElementById('modalPropriedadeRemoveButton').style.display = 'block';
                        const imgEl = document.getElementById('modalPropriedadeImagePreview');
                        const pdfEl = document.getElementById('modalPropriedadePdfPreview');
                        if (file.type === 'application/pdf') {
                            if (imgEl) { imgEl.src = ''; imgEl.style.display = 'none'; }
                            if (pdfEl) pdfEl.style.display = 'block';
                            document.getElementById('modalPropriedadeImageLabel').textContent = 'Novo PDF selecionado:';
                        } else {
                            if (pdfEl) pdfEl.style.display = 'none';
                            if (imgEl) { imgEl.src = dataUrl; imgEl.style.display = 'block'; }
                            document.getElementById('modalPropriedadeImageLabel').textContent = 'Nova imagem selecionada:';
                        }
                    };
                    reader.readAsDataURL(file);
                }
            });
        }
    }
};

// Inicializar quando o DOM estiver carregado
document.addEventListener("DOMContentLoaded", function () {
    clienteJS.init();
}); 