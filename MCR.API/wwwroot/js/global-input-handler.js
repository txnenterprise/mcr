(function ($) {
    // Primeiro, vamos garantir que o validator aceite números no formato brasileiro
    // Isso precisa ser feito ANTES de qualquer validação
    if ($.validator) {
        $.validator.addMethod('decimal', function (value, element) {
            if (this.optional(element)) return true;

            // Remove pontos de milhar e converte vírgula para ponto
            value = value.replace(/\./g, '').replace(',', '.');
            return !isNaN(value) && isFinite(value);
        }, 'Por favor, insira um número válido.');

        // Sobrescreve o método number padrão
        $.validator.methods.number = function (value, element) {
            return this.optional(element) ||
                !isNaN(value.replace(/\./g, '').replace(',', '.'));
        };

        // Sobrescreve o método range
        $.validator.methods.range = function (value, element, param) {
            value = value.replace(/\./g, '').replace(',', '.');
            return this.optional(element) || (value >= param[0] && value <= param[1]);
        };
    }

    const InputHandler = {
        init: function () {
            this.initInputMasks();
            this.initFormSubmitHandler();
        },

        initInputMasks: function () {
            const decimalConfig = {
                prefix: '',
                allowNegative: false,
                thousands: '.',
                decimal: ',',
                precision: 2,
                affixesStay: false,
                allowZero: true
            };

            // Aplica máscaras baseado no data-input-type
            $('[data-input-type]').each(function () {
                const $input = $(this);
                const inputType = $input.data('input-type');

                switch (inputType) {
                    case 'decimal':
                        const precision = $input.data('precision') || 2;
                        $input.maskMoney({ ...decimalConfig, precision: precision });
                        // Adiciona a classe para validação personalizada
                        $input.addClass('decimal');
                        break;
                }
            });
        },

        initFormSubmitHandler: function () {
            $('form').on('submit', function () {
                $('[data-input-type="decimal"]').each(function () {
                    const $input = $(this);
                    let value = $input.val();

                    if (value) {
                        // Remove separadores de milhar e converte vírgula para ponto
                        value = value.replace(/\./g, '').replace(',', '.');
                        $input.val(value);
                    }
                });
                return true;
            });
        }
    };

    // Inicializa quando o documento estiver pronto
    $(document).ready(function () {
        InputHandler.init();
    });

})(jQuery); 