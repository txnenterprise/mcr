// Dashboard JavaScript
/** Área agregada já vem em hectares (AreaTotal da proposta); apenas formatar para exibição. */
function formatAreaHaDisplay(value) {
    const n = Number(value);
    if (Number.isNaN(n)) return (0).toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    return n.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
}

class DashboardManager {
    constructor() {
        this.currentUserLevel = null;
        this.currentFilters = {};
        this.dashboardData = {};
        this.charts = {};
        this.chartsInitialized = false;
        this.currentChartType = 'pie'; // Tipo padrão
        
        this.init();
    }

    async init() {
        try {
            console.log('=== INICIANDO DASHBOARD ===');
            
            // Carregar dados iniciais do servidor se disponíveis
            this.loadServerData();
            
            console.log('Após loadServerData - dashboardData:', this.dashboardData);
            
            await this.loadUserLevel();
            console.log('Após loadUserLevel - dashboardData:', this.dashboardData);
            
            await this.loadInitialData();
            console.log('Após loadInitialData - dashboardData:', this.dashboardData);
            
            this.setupEventListeners();
            this.initializeSelect2();
            
            // Garantir que a visibilidade dos filtros seja atualizada após tudo estar pronto
            setTimeout(() => {
                this.updateFilterVisibility();
            }, 200);
            
            // NUNCA carregar dados do dashboard na inicialização
            // Sempre manter valores do ViewBag na tela inicial
            console.log('Inicialização concluída, mantendo valores do ViewBag');
            
            console.log('=== DASHBOARD INICIALIZADO ===');
        } catch (error) {
            console.error('Erro ao inicializar dashboard:', error);
            this.showError('Erro ao carregar o dashboard');
        }
    }

    loadServerData() {
        // Verificar se há dados do servidor disponíveis
        if (window.serverData) {
            this.dashboardData = window.serverData.dashboardData;
            this.currentUserLevel = window.serverData.userLevel;
            this.updateUserLevelBadge();
            
            // Aguardar um pouco para garantir que o DOM esteja pronto
            setTimeout(() => {
                this.updateFilterVisibility();
            }, 50);
            
            // NÃO atualizar summary aqui - manter valores do ViewBag
            // this.updateSummary();
            
            // NÃO atualizar tabela aqui - manter dados do ViewBag
            // this.updateTable(this.dashboardData);
            
            // Atualizar gráficos com dados reais do servidor
            if (this.dashboardData) {
                this.updateCharts(this.dashboardData);
            }
        }
    }

    async loadUserLevel() {
        try {
            const response = await fetch('/api/dashboard/user-level');
            const data = await response.json();
            this.currentUserLevel = data.userLevel;
            this.updateUserLevelBadge();
            
            // Aguardar um pouco para garantir que o DOM esteja pronto
            setTimeout(() => {
                this.updateFilterVisibility();
            }, 50);
        } catch (error) {
            console.error('Erro ao carregar nível do usuário:', error);
            this.currentUserLevel = 'consultor'; // fallback
            // Ainda assim, tentar atualizar a visibilidade dos filtros
            setTimeout(() => {
                this.updateFilterVisibility();
            }, 50);
        }
    }

    updateUserLevelBadge() {
        const badge = document.getElementById('currentUserLevel');
        const levelNames = {
            'admin': 'Administrador',
            'corretor': 'Corretor',
            'gestor': 'Gestor de Canal',
            'consultor': 'Consultor'
        };
        badge.textContent = levelNames[this.currentUserLevel] || 'Usuário';
        badge.className = `badge badge-${this.getLevelBadgeClass()}`;
    }

    getLevelBadgeClass() {
        const classes = {
            'admin': 'danger',
            'corretor': 'warning',
            'gestor': 'info',
            'consultor': 'success'
        };
        return classes[this.currentUserLevel] || 'secondary';
    }

    updateFilterVisibility() {
        // Mostrar/ocultar filtros baseado no nível do usuário
        const corretoraFilter = document.getElementById('corretoraFilter');
        const canalFilter = document.getElementById('canalFilter');
        const paFilter = document.getElementById('paFilter');

        // Verificar se os elementos existem antes de tentar alterá-los
        if (!corretoraFilter || !canalFilter || !paFilter) {
            console.warn('Elementos de filtro não encontrados. Tentando novamente...', {
                corretoraFilter: !!corretoraFilter,
                canalFilter: !!canalFilter,
                paFilter: !!paFilter
            });
            // Tentar novamente após um pequeno delay
            setTimeout(() => this.updateFilterVisibility(), 100);
            return;
        }

        console.log('Atualizando visibilidade dos filtros para nível:', this.currentUserLevel);
        console.log('Estado atual dos filtros:', {
            corretora: corretoraFilter.style.display,
            canal: canalFilter.style.display,
            pa: paFilter.style.display
        });

        switch (this.currentUserLevel) {
            case 'admin':
                corretoraFilter.style.display = 'block';
                canalFilter.style.display = 'block';
                paFilter.style.display = 'block';
                console.log('Filtros configurados para ADMIN - todos visíveis');
                break;
            case 'corretor':
                corretoraFilter.style.display = 'none';
                canalFilter.style.display = 'block';
                paFilter.style.display = 'block';
                console.log('Filtros configurados para CORRETOR - canal e PA visíveis');
                break;
            case 'gestor':
                corretoraFilter.style.display = 'none';
                canalFilter.style.display = 'none';
                paFilter.style.display = 'block';
                console.log('Filtros configurados para GESTOR - apenas PA visível');
                break;
            case 'consultor':
                corretoraFilter.style.display = 'none';
                canalFilter.style.display = 'none';
                paFilter.style.display = 'block';
                console.log('Filtros configurados para CONSULTOR - apenas PA visível');
                break;
            default:
                // Fallback: mostrar todos os filtros se o nível não for reconhecido
                console.warn('Nível de usuário não reconhecido:', this.currentUserLevel);
                corretoraFilter.style.display = 'block';
                canalFilter.style.display = 'block';
                paFilter.style.display = 'block';
                break;
        }

        // Verificar se a alteração foi aplicada
        console.log('Estado final dos filtros:', {
            corretora: corretoraFilter.style.display,
            canal: canalFilter.style.display,
            pa: paFilter.style.display
        });
    }

    async loadInitialData() {
        try {
            console.log('Carregando dados iniciais...');
            const response = await fetch('/api/dashboard/initial-data');
            const data = await response.json();
            
            console.log('Dados iniciais carregados:', data);
            
            this.populateSelectOptions('corretoraSelect', data.corretoras);
            this.populateSelectOptions('canalSelect', data.canais);
            this.populateSelectOptions('paSelect', data.pontosAtendimento);
            this.populateSelectOptions('culturaSelect', data.culturas);
            this.populateSelectOptions('safraSelect', data.safras);
            this.populateSelectOptions('seguradoraSelect', data.seguradoras);
            this.populateStatusOptions();
            
            console.log('Dados iniciais populados com sucesso');
        } catch (error) {
            console.error('Erro ao carregar dados iniciais:', error);
        }
    }

    populateSelectOptions(selectId, options) {
        const select = document.getElementById(selectId);
        if (!select) {
            console.error(`Elemento ${selectId} não encontrado`);
            return;
        }
        
        select.innerHTML = '<option value="">Selecione...</option>';
        
        if (!options || options.length === 0) {
            console.log(`Nenhuma opção encontrada para ${selectId}`);
            return;
        }
        
        console.log(`Populando ${selectId} com ${options.length} opções:`, options);
        
        options.forEach(option => {
            const optionElement = document.createElement('option');
            optionElement.value = option.id;
            optionElement.textContent = option.nome;
            select.appendChild(optionElement);
        });
    }

    populateStatusOptions() {
        const statusInicialSelect = document.getElementById('statusInicialSelect');
        const statusFinalSelect = document.getElementById('statusFinalSelect');
        
        // Status de contratação
        const statusOptions = [
            { value: 'aguardando_transmissao', text: 'Aguardando Transmissão' },
            { value: 'cotacao_negociacao', text: 'Cotação em Negociação' },
            { value: 'cotacao_realizada_sucesso', text: 'Cotação Realizada com Sucesso' },
            { value: 'proposta_andamento', text: 'Proposta em Andamento' },
            { value: 'proposta_negociacao', text: 'Proposta em Negociação' },
            { value: 'proposta_pendencia', text: 'Proposta com Pendência' },
            { value: 'proposta_transmitida', text: 'Proposta Transmitida (Em Análise)' },
            { value: 'proposta_aceita', text: 'Proposta Aceita → Devolutiva da Seguradora' },
            { value: 'apolice_emitida', text: 'Apólice Emitida' },
            { value: 'solicitar_endosso', text: 'Solicitar Endosso' },
            { value: 'endosso_pendencia', text: 'Endosso com Pendência' },
            { value: 'endosso_transmitido', text: 'Endosso Transmitido' },
            { value: 'endosso_emitido', text: 'Endosso Emitido' },
            { value: 'proposta_recusada', text: 'Proposta Recusada → Devolutiva da Seguradora' },
            { value: 'proposta_cancelada', text: 'Proposta Cancelada' },
            { value: 'apolice_cancelada', text: 'Apólice Cancelada' },
            { value: 'cotacao_encerrada', text: 'Cotação Encerrada sem Sucesso' }
        ];

        statusOptions.forEach(option => {
            const optionElement1 = document.createElement('option');
            optionElement1.value = option.value;
            optionElement1.textContent = option.text;
            statusInicialSelect.appendChild(optionElement1);

            const optionElement2 = document.createElement('option');
            optionElement2.value = option.value;
            optionElement2.textContent = option.text;
            statusFinalSelect.appendChild(optionElement2);
        });
    }

    initializeSelect2() {
        $('.filter-select').select2({
            placeholder: 'Selecione...',
            allowClear: true,
            width: '100%'
        });
    }

    setupEventListeners() {
        // Aplicar filtros
        document.getElementById('applyFilters').addEventListener('click', () => {
            this.applyFilters();
        });

        // Limpar filtros
        document.getElementById('clearFilters').addEventListener('click', () => {
            this.clearFilters();
        });

        // Exportar dados
        document.getElementById('exportData').addEventListener('click', () => {
            this.exportData();
        });

        // Controles de gráficos
        document.querySelectorAll('.chart-type-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const chart = e.target.dataset.chart;
                const type = e.target.dataset.type;
                this.changeChartType(chart, type);
            });
        });

        // Filtros dependentes
        document.getElementById('canalSelect').addEventListener('change', () => {
            this.updatePAFilter();
        });

        // Event delegation para células de status clicáveis (renderizadas no servidor e dinamicamente)
        const dashboardTable = document.getElementById('dashboardTable');
        if (dashboardTable) {
            dashboardTable.addEventListener('click', (e) => {
                const statusCell = e.target.closest('.status-clickable');
                if (statusCell) {
                    const statusUrl = statusCell.getAttribute('data-status-url');
                    if (statusUrl) {
                        window.location.href = statusUrl;
                    }
                }
            });
        }
    }

    async updatePAFilter() {
        const canalIds = $('#canalSelect').val();
        if (!canalIds || canalIds.length === 0) {
            $('#paSelect').empty().append('<option value="">Selecione...</option>');
            return;
        }

        try {
            const response = await fetch('/api/dashboard/pontos-atendimento', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ canalIds })
            });
            const data = await response.json();
            this.populateSelectOptions('paSelect', data);
        } catch (error) {
            console.error('Erro ao carregar PAs:', error);
        }
    }

    async applyFilters() {
        this.showLoading();
        
        try {
            const filters = this.getCurrentFilters();
            const response = await fetch('/api/dashboard/data', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(filters)
            });
            
            if (response.ok) {
                const data = await response.json();
                this.dashboardData = data;
                console.log('Dados do dashboard carregados:', this.dashboardData);
                console.log('StatusData recebido:', data.StatusData);
                console.log('Quantidade de registros StatusData:', Object.keys(data.StatusData || {}).length);
                
                // Só atualizar summary se tivermos dados válidos
                if (data && data.StatusData) {
                    this.updateSummary();
                } else {
                    console.log('Dados inválidos, mantendo valores do ViewBag');
                }
                
                // Atualizar tabela e subtotal via Partial Views
                await this.updateTablePartial(filters);
                await this.updateSubtotalPartial(filters);
                
                // Atualizar gráficos
                this.updateCharts(data);
            } else {
                console.error('Erro ao carregar dados do dashboard');
                this.showError('Erro ao carregar dados do dashboard');
            }
        } catch (error) {
            console.error('Erro ao aplicar filtros:', error);
            this.showError('Erro ao carregar dados');
        } finally {
            this.hideLoading();
        }
    }

    getCurrentFilters() {
        return {
            userLevel: this.currentUserLevel,
            corretoraIds: $('#corretoraSelect').val() || [],
            canalIds: $('#canalSelect').val() || [],
            paIds: $('#paSelect').val() || [],
            culturaIds: $('#culturaSelect').val() || [],
            safraIds: $('#safraSelect').val() || [],
            statusInicial: $('#statusInicialSelect').val() || [],
            statusFinal: $('#statusFinalSelect').val() || [],
            seguradoraIds: $('#seguradoraSelect').val() || []
        };
    }

    getStatusUrl(status) {
        const statusLower = status.toLowerCase();
        
        if (statusLower.includes('sinistro')) {
            return '/Sinistro';
        } else if (statusLower.includes('apólice') || statusLower.includes('apolice')) {
            return '/Apolices';
        } else if (statusLower.includes('endosso')) {
            return '/Endosso';
        } else {
            // Outros ou proposta
            return '/Propostas';
        }
    }

    updateTable(data) {
        const tbody = document.getElementById('dashboardTableContainer');
        if (!tbody || !data || !data.StatusData) return;

        // Limpar completamente o tbody
        tbody.innerHTML = '';

        // Usar todos os dados de status (não há mais subtotal misturado)
        const normalData = Object.entries(data.StatusData);
        const subtotalData = data.Subtotal;

        // Adicionar linhas normais
        normalData.forEach(([status, statusData]) => {
            const tr = document.createElement('tr');
            const statusUrl = this.getStatusUrl(status);
            tr.innerHTML = `
                <td class="status-clickable" data-status-url="${statusUrl}" style="cursor: pointer;">${status}</td>
                <td>${formatAreaHaDisplay(statusData.Area)}</td>
                <td>${statusData.Qtd.toLocaleString('pt-BR')}</td>
                <td>${statusData.LMI.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</td>
                <td>${statusData.Premio.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</td>
                <td>${statusData.Produtividade.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                <td>${statusData.Taxa.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}%</td>
            `;
            
            // Adicionar evento de clique na célula de status
            const statusCell = tr.querySelector('.status-clickable');
            if (statusCell) {
                statusCell.addEventListener('click', () => {
                    window.location.href = statusUrl;
                });
            }
            
            tbody.appendChild(tr);
        });

        // Atualizar subtotais na tabela se existirem
        if (subtotalData) {
            this.updateSubtotals(subtotalData);
        }
    }


    updateSubtotals(subtotalData) {
        // Atualizar elementos de subtotal na tabela
        const subtotalArea = document.getElementById('subtotalArea');
        const subtotalQtd = document.getElementById('subtotalQtd');
        const subtotalLMI = document.getElementById('subtotalLMI');
        const subtotalPremio = document.getElementById('subtotalPremio');
        const subtotalProdutividade = document.getElementById('subtotalProdutividade');
        const subtotalTaxa = document.getElementById('subtotalTaxa');

        if (subtotalArea) {
            subtotalArea.textContent = formatAreaHaDisplay(subtotalData.Area);
        }
        if (subtotalQtd) {
            subtotalQtd.textContent = subtotalData.Qtd.toLocaleString('pt-BR');
        }
        if (subtotalLMI) {
            subtotalLMI.textContent = subtotalData.LMI.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        }
        if (subtotalPremio) {
            subtotalPremio.textContent = subtotalData.Premio.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        }
        if (subtotalProdutividade) {
            subtotalProdutividade.textContent = subtotalData.Produtividade.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        }
        if (subtotalTaxa) {
            subtotalTaxa.textContent = subtotalData.Taxa.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '%';
        }
    }

    updateCharts(data) {
        // Destruir gráficos existentes
        this.destroyCharts();
        
        // Gerar dados dinâmicos baseados no StatusData
        const chartData = this.generateChartData(data);
        
        // Criar gráficos com dados dinâmicos
        this.createAreaChart(chartData.area);
        this.createSeguradoraChart(chartData.seguradora);
        this.createStatusChart(chartData.status);
        
        // Configurar botões de controle
        this.setupChartControls();
        
        // Atualizar status de inicialização
        this.chartsInitialized = true;
        
        console.log('Gráficos atualizados com sucesso');
    }

    // Gerar dados para gráficos baseados no StatusData
    generateChartData(data) {
        console.log('Gerando dados de gráficos...');
        console.log('Dados recebidos:', data);
        
        // Verificar se temos StatusData
        const statusData = data?.StatusData || data?.statusData || {};
        console.log('StatusData encontrado:', statusData);
        
        if (!statusData || Object.keys(statusData).length === 0) {
            console.log('Nenhum StatusData encontrado, usando dados vazios');
            return this.getEmptyChartData();
        }
        
        const statusEntries = Object.entries(statusData);
        console.log('StatusEntries:', statusEntries);
        
        return {
            area: this.generateAreaChartData(statusEntries),
            seguradora: this.generateSeguradoraChartData(statusEntries),
            status: this.generateStatusChartData(statusEntries)
        };
    }

    // Gerar dados do gráfico de área baseado no StatusData
    generateAreaChartData(statusEntries) {
        console.log('Gerando dados de área...');
        console.log('StatusEntries recebidos:', statusEntries);
        
        if (statusEntries.length === 0) {
            return {
                labels: ['Nenhum dado'],
                values: [0],
                title: 'Área por Status (ha)'
            };
        }

        const labels = statusEntries.map(([status, _]) => status);
        const values = statusEntries.map(([_, statusData]) => {
            const raw = statusData.Area ?? statusData.area ?? 0;
            return Number(raw);
        });
        
        console.log('Área - Labels:', labels);
        console.log('Área - Values:', values);
        
        return {
            labels: labels,
            values: values,
            title: 'Área Segurada por Status (ha)'
        };
    }

    // Gerar dados do gráfico de quantidade baseado no StatusData
    generateSeguradoraChartData(statusEntries) {
        console.log('Gerando dados de quantidade...');
        console.log('StatusEntries recebidos:', statusEntries);
        
        if (statusEntries.length === 0) {
            return {
                labels: ['Nenhum dado'],
                values: [0],
                title: 'Propostas por Status'
            };
        }

        const labels = statusEntries.map(([status, _]) => status);
        const values = statusEntries.map(([_, statusData]) => statusData.qtd || 0);
        
        console.log('Quantidade - Labels:', labels);
        console.log('Quantidade - Values:', values);
        
        return {
            labels: labels,
            values: values,
            title: 'Quantidade de Propostas por Status'
        };
    }

    // Gerar dados do gráfico de prêmio baseado no StatusData
    generateStatusChartData(statusEntries) {
        console.log('Gerando dados de prêmio...');
        console.log('StatusEntries recebidos:', statusEntries);
        
        if (statusEntries.length === 0) {
            return {
                labels: ['Nenhum dado'],
                values: [0],
                title: 'Prêmio Total por Status (R$)'
            };
        }

        const labels = statusEntries.map(([status, _]) => status);
        const values = statusEntries.map(([_, statusData]) => statusData.premio || 0);
        
        console.log('Prêmio - Labels:', labels);
        console.log('Prêmio - Values:', values);
        
        return {
            labels: labels,
            values: values,
            title: 'Prêmio Total por Status (R$)'
        };
    }

    // Dados vazios para gráficos
    getEmptyChartData() {
        return {
            area: {
                labels: ['Nenhum dado'],
                values: [0],
                title: 'Área Segurada por Status (ha)'
            },
            seguradora: {
                labels: ['Nenhum dado'],
                values: [0],
                title: 'Quantidade de Propostas por Status'
            },
            status: {
                labels: ['Nenhum dado'],
                values: [0],
                title: 'Prêmio Total por Status (R$)'
            }
        };
    }


    updateSummary() {
        console.log('updateSummary() chamado - Stack trace:', new Error().stack);
        
        // Só atualizar se tivermos dados válidos do dashboard
        if (!this.dashboardData || !this.dashboardData.StatusData) {
            console.log('Dados do dashboard não disponíveis, mantendo valores do ViewBag');
            return;
        }

        // Verificar se StatusData tem dados válidos
        const statusEntries = Object.entries(this.dashboardData.StatusData);
        if (statusEntries.length === 0) {
            console.log('StatusData vazio, mantendo valores do ViewBag');
            return;
        }

        // Usar subtotal separado se disponível, senão calcular dos dados de status
        let totalPropostas = 0;
        let areaTotal = 0;
        let premioTotal = 0;
        let lmiTotal = 0;

        if (this.dashboardData.Subtotal) {
            // Usar subtotal separado
            totalPropostas = this.dashboardData.Subtotal.Qtd || 0;
            areaTotal = this.dashboardData.Subtotal.Area || 0;
            premioTotal = this.dashboardData.Subtotal.Premio || 0;
            lmiTotal = this.dashboardData.Subtotal.LMI || 0;
        } else {
            // Calcular dos dados de status (fallback)
            statusEntries.forEach(([key, status]) => {
                if (status && typeof status === 'object') {
                    totalPropostas += (status.Qtd || 0);
                    areaTotal += (status.Area || 0);
                    premioTotal += (status.Premio || 0);
                    lmiTotal += (status.LMI || 0);
                }
            });
        }

        // Só atualizar se os valores calculados forem maiores que zero
        if (totalPropostas === 0 && areaTotal === 0 && premioTotal === 0 && lmiTotal === 0) {
            console.log('Valores calculados são zero, mantendo valores do ViewBag');
            return;
        }

        console.log('Atualizando summary com valores calculados:', { totalPropostas, areaTotal, premioTotal, lmiTotal });

        // Atualizar elementos do resumo se existirem
        const totalPropostasEl = document.getElementById('totalPropostas');
        if (totalPropostasEl) {
            totalPropostasEl.textContent = totalPropostas.toLocaleString('pt-BR');
        }

        const areaTotalEl = document.getElementById('areaTotal');
        if (areaTotalEl) {
            areaTotalEl.textContent = formatAreaHaDisplay(areaTotal);
        }

        const premioTotalEl = document.getElementById('premioTotal');
        if (premioTotalEl) {
            premioTotalEl.textContent = premioTotal.toLocaleString('pt-BR', { 
                style: 'currency', 
                currency: 'BRL' 
            });
        }

        const lmiTotalEl = document.getElementById('lmiTotal');
        if (lmiTotalEl) {
            lmiTotalEl.textContent = lmiTotal.toLocaleString('pt-BR', { 
                style: 'currency', 
                currency: 'BRL' 
            });
        }
    }

    initializeCharts() {
        // Verificar se Chart.js está disponível
        if (typeof Chart === 'undefined') {
            console.error('Chart.js não está carregado');
            return false;
        }

        // Verificar se os canvas existem
        const areaCanvas = document.getElementById('areaChart');
        const seguradoraCanvas = document.getElementById('seguradoraChart');
        const statusCanvas = document.getElementById('statusChart');

        if (!areaCanvas || !seguradoraCanvas || !statusCanvas) {
            console.error('Canvas dos gráficos não encontrados');
            return false;
        }

        // Destruir gráficos existentes se houver
        this.destroyCharts();

        console.log('Inicializando gráficos...');
        
        try {
            // Usar dados do dashboard se disponível, senão usar dados de exemplo
            const chartData = this.dashboardData ? {
                Area: this.dashboardData.Area || {
                    labels: ['Soja', 'Milho', 'Algodão', 'Café'],
                    values: [1200, 800, 600, 400]
                },
                Seguradora: this.dashboardData.Seguradora || {
                    labels: ['Seguradora A', 'Seguradora B', 'Seguradora C'],
                    values: [1500, 1000, 500]
                },
                Status: this.dashboardData.Status || {
                    labels: ['Aguardando Transmissão', 'Proposta Aceita', 'Apólice Emitida'],
                    values: [800, 1200, 1000]
                }
            } : {
                Area: {
                    labels: ['Soja', 'Milho', 'Algodão', 'Café'],
                    values: [1200, 800, 600, 400]
                },
                Seguradora: {
                    labels: ['Seguradora A', 'Seguradora B', 'Seguradora C'],
                    values: [1500, 1000, 500]
                },
                Status: {
                    labels: ['Aguardando Transmissão', 'Proposta Aceita', 'Apólice Emitida'],
                    values: [800, 1200, 1000]
                }
            };

            console.log('Dados dos gráficos:', chartData);

            // Criar gráficos
            this.createAreaChart(chartData.Area);
            this.createSeguradoraChart(chartData.Seguradora);
            this.createStatusChart(chartData.Status);

            // Configurar botões de controle
            this.setupChartControls();

            this.chartsInitialized = true;
            console.log('Gráficos inicializados com sucesso');
            return true;
        } catch (error) {
            console.error('Erro ao inicializar gráficos:', error);
            this.chartsInitialized = false;
            return false;
        }
    }

    destroyCharts() {
        // Destruir todos os gráficos existentes
        Object.keys(this.charts).forEach(key => {
            if (this.charts[key]) {
                this.charts[key].destroy();
                this.charts[key] = null;
            }
        });
        this.charts = {};
        this.chartsInitialized = false;
    }

    setupChartControls() {
        document.querySelectorAll('.chart-type-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const chartName = e.target.dataset.chart;
                const chartType = e.target.dataset.type;
                this.changeChartType(chartName, chartType);
            });
        });
    }

    createAreaChart(data) {
        console.log('Criando gráfico de área:', data);
        const canvas = document.getElementById('areaChart');
        if (!canvas) {
            console.error('Canvas areaChart não encontrado');
            return false;
        }
        
        // Verificar se os dados existem
        if (!data || !data.labels || !data.values) {
            console.error('Dados do gráfico de área inválidos:', data);
            return false;
        }
        
        const ctx = canvas.getContext('2d');
        
        try {
            this.charts.area = new Chart(ctx, {
                type: this.currentChartType || 'pie',
                data: {
                    labels: data.labels,
                    datasets: [{
                        label: data.title,
                        data: data.values,
                        backgroundColor: this.getChartColors(data.labels.length),
                        borderColor: this.getChartColors(data.labels.length, true),
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        title: {
                            display: true,
                            text: data.title
                        },
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                padding: 20,
                                generateLabels: function(chart) {
                                    const data = chart.data;
                                    if (data.labels.length && data.datasets.length) {
                                        return data.labels.map((label, i) => ({
                                            text: label,
                                            fillStyle: data.datasets[0].backgroundColor[i],
                                            strokeStyle: data.datasets[0].borderColor[i],
                                            lineWidth: data.datasets[0].borderWidth,
                                            pointStyle: 'circle',
                                            hidden: false,
                                            index: i
                                        }));
                                    }
                                    return [];
                                }
                            }
                        }
                    }
                }
            });
            
            console.log('Gráfico de área criado com sucesso');
            return true;
        } catch (error) {
            console.error('Erro ao criar gráfico de área:', error);
            return false;
        }
    }

    createSeguradoraChart(data) {
        console.log('Criando gráfico de seguradora:', data);
        const canvas = document.getElementById('seguradoraChart');
        if (!canvas) {
            console.error('Canvas seguradoraChart não encontrado');
            return false;
        }
        
        // Verificar se os dados existem
        if (!data || !data.labels || !data.values) {
            console.error('Dados do gráfico de seguradora inválidos:', data);
            return false;
        }
        
        const ctx = canvas.getContext('2d');
        
        try {
            this.charts.seguradora = new Chart(ctx, {
                type: this.currentChartType || 'pie',
                data: {
                    labels: data.labels,
                    datasets: [{
                        label: data.title,
                        data: data.values,
                        backgroundColor: this.getChartColors(data.labels.length),
                        borderColor: this.getChartColors(data.labels.length, true),
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        title: {
                            display: true,
                            text: data.title
                        },
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                padding: 20,
                                generateLabels: function(chart) {
                                    const data = chart.data;
                                    if (data.labels.length && data.datasets.length) {
                                        return data.labels.map((label, i) => ({
                                            text: label,
                                            fillStyle: data.datasets[0].backgroundColor[i],
                                            strokeStyle: data.datasets[0].borderColor[i],
                                            lineWidth: data.datasets[0].borderWidth,
                                            pointStyle: 'circle',
                                            hidden: false,
                                            index: i
                                        }));
                                    }
                                    return [];
                                }
                            }
                        }
                    },
                    scales: this.currentChartType === 'pie' ? {} : {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
            
            console.log('Gráfico de seguradora criado com sucesso');
            return true;
        } catch (error) {
            console.error('Erro ao criar gráfico de seguradora:', error);
            return false;
        }
    }

    createStatusChart(data) {
        console.log('Criando gráfico de status:', data);
        const canvas = document.getElementById('statusChart');
        if (!canvas) {
            console.error('Canvas statusChart não encontrado');
            return false;
        }
        
        // Verificar se os dados existem
        if (!data || !data.labels || !data.values) {
            console.error('Dados do gráfico de status inválidos:', data);
            return false;
        }
        
        const ctx = canvas.getContext('2d');
        
        try {
            this.charts.status = new Chart(ctx, {
                type: this.currentChartType || 'pie',
                data: {
                    labels: data.labels,
                    datasets: [{
                        label: data.title,
                        data: data.values,
                        backgroundColor: this.getChartColors(data.labels.length),
                        borderColor: this.getChartColors(data.labels.length, true),
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        title: {
                            display: true,
                            text: data.title
                        },
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                padding: 20,
                                generateLabels: function(chart) {
                                    const data = chart.data;
                                    if (data.labels.length && data.datasets.length) {
                                        return data.labels.map((label, i) => ({
                                            text: label,
                                            fillStyle: data.datasets[0].backgroundColor[i],
                                            strokeStyle: data.datasets[0].borderColor[i],
                                            lineWidth: data.datasets[0].borderWidth,
                                            pointStyle: 'circle',
                                            hidden: false,
                                            index: i
                                        }));
                                    }
                                    return [];
                                }
                            }
                        }
                    },
                    scales: this.currentChartType === 'pie' ? {} : {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
            
            console.log('Gráfico de status criado com sucesso');
            return true;
        } catch (error) {
            console.error('Erro ao criar gráfico de status:', error);
            return false;
        }
    }

    changeChartType(chartName, type) {
        console.log(`Alterando gráfico ${chartName} para tipo ${type}`);
        
        // Atualizar tipo atual
        this.currentChartType = type;

        // Atualizar botões ativos
        document.querySelectorAll(`[data-chart]`).forEach(btn => {
            btn.classList.remove('active');
        });
        document.querySelectorAll(`[data-chart="${type}"]`).forEach(btn => {
            btn.classList.add('active');
        });

        // Recriar todos os gráficos com o novo tipo
        if (this.dashboardData) {
            const chartData = this.generateChartData(this.dashboardData);
            
            // Destruir gráficos existentes
            this.destroyCharts();
            
            // Recriar com novo tipo
            this.createAreaChart(chartData.area);
            this.createSeguradoraChart(chartData.seguradora);
            this.createStatusChart(chartData.status);
        }
    }

    getChartColors(count, isBorder = false) {
        const colors = [
            '#FF6384', // Vermelho
            '#36A2EB', // Azul
            '#FFCE56', // Amarelo
            '#4BC0C0', // Turquesa
            '#9966FF', // Roxo
            '#FF9F40', // Laranja
            '#FF6B6B', // Rosa
            '#4ECDC4', // Verde água
            '#45B7D1', // Azul claro
            '#96CEB4', // Verde claro
            '#FFEAA7', // Amarelo claro
            '#DDA0DD'  // Ameixa
        ];
        
        const borderColors = [
            '#E55656', // Vermelho escuro
            '#2E8BC0', // Azul escuro
            '#E6B800', // Amarelo escuro
            '#3BA3A3', // Turquesa escuro
            '#8A4FCC', // Roxo escuro
            '#E68A00', // Laranja escuro
            '#E55A5A', // Rosa escuro
            '#3BB5AD', // Verde água escuro
            '#3A9BC1', // Azul claro escuro
            '#85B89A', // Verde claro escuro
            '#E6D177', // Amarelo claro escuro
            '#C77BC7'  // Ameixa escuro
        ];
        
        return isBorder ? borderColors.slice(0, count) : colors.slice(0, count);
    }

    clearFilters() {
        $('.filter-select').val(null).trigger('change');
        this.currentFilters = {};
        // Sempre aplicar filtros vazios para recarregar dados
        this.applyFilters();
    }

    async loadDashboardData() {
        // Só carregar dados se não tivermos dados do servidor
        if (window.serverData && window.serverData.dashboardData) {
            console.log('Dados do servidor já disponíveis, não carregando via API');
            return;
        }
        
        // Verificar se já temos dados válidos
        if (this.dashboardData && this.dashboardData.StatusData) {
            console.log('Dados do dashboard já disponíveis, não recarregando');
            return;
        }
        
        console.log('Carregando dados via API...');
        this.showLoading();
        try {
            await this.applyFilters();
        } finally {
            this.hideLoading();
        }
    }

    exportData() {
        // Implementar exportação de dados
        console.log('Exportar dados:', this.dashboardData);
    }

    showLoading() {
        document.getElementById('loadingOverlay').style.display = 'flex';
    }

    hideLoading() {
        document.getElementById('loadingOverlay').style.display = 'none';
    }

    showError(message) {
        // Implementar exibição de erro
        console.error(message);
    }

    formatNumber(value) {
        return new Intl.NumberFormat('pt-BR').format(value);
    }

    formatCurrency(value) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    }

    // Novas funções para Partial Views
    async updateTablePartial(filters) {
        try {
            const response = await fetch('/dashboard/GetDashboardTable', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(filters)
            });
            
            if (response.ok) {
                const html = await response.text();
                document.getElementById('dashboardTableContainer').innerHTML = html;
            }
        } catch (error) {
            console.error('Erro ao atualizar tabela:', error);
        }
    }

    async updateSubtotalPartial(filters) {
        try {
            const response = await fetch('/dashboard/GetDashboardSubtotal', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(filters)
            });
            
            if (response.ok) {
                const html = await response.text();
                document.getElementById('dashboardSubtotalContainer').innerHTML = html;
            }
        } catch (error) {
            console.error('Erro ao atualizar subtotal:', error);
        }
    }

    async updateSummaryAndCharts(filters) {
        try {
            const response = await fetch('/api/dashboard/data', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(filters)
            });
            
            if (response.ok) {
                const data = await response.json();
                this.dashboardData = data;
                
                // Atualizar summary
                if (data && data.StatusData) {
                    this.updateSummary();
                }
                
                // Atualizar gráficos
                this.updateCharts(data);
            }
        } catch (error) {
            console.error('Erro ao atualizar summary e gráficos:', error);
        }
    }

    formatPercentage(value) {
        return new Intl.NumberFormat('pt-BR', {
            style: 'percent',
            minimumFractionDigits: 2
        }).format(value / 100);
    }
}

// Inicializar dashboard quando a página carregar
document.addEventListener('DOMContentLoaded', () => {
    window.dashboardManager = new DashboardManager();
});
