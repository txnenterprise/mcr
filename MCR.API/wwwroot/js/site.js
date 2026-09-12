$(document).ready(function () {

    $('#open-sidebar').click(() => {

        // add class active on #sidebar
        $('#sidebar').addClass('active');

        // show sidebar overlay
        $('#sidebar-overlay').removeClass('d-none');

    });


    $('#sidebar-overlay').click(function () {

        // add class active on #sidebar
        $('#sidebar').removeClass('active');

        // show sidebar overlay
        $(this).addClass('d-none');

    });

    $("#CNPJ_Corretora").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, "");
        value = value.substring(0, 14);
        value = value.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})?$/, "$1.$2.$3/$4-$5");
        $(this).val(value);
    });

    //Consultar endereço pelo CEP-------------------------------------------------
    var mostrouAlerta = false;
    $("#CEP").blur(function () {
        var cep = $(this).val().replace(/\D/g, '');
        if (cep != "") {
            var validacep = /^[0-9]{8}$/;

            if (validacep.test(cep)) {
                $("#Endereco").val("...");
                $("#Bairro").val("...");
                $("#Cidade").val("...");
                $("#Estado").val("...");
                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {
                    if (!("erro" in dados)) {
                        $("#Endereco").val(dados.logradouro);
                        $("#Bairro").val(dados.bairro);
                        $("#Cidade").val(dados.localidade);
                        $("#Estado").val(dados.uf);
                        mostrouAlerta = false;
                    }
                    else {
                        limpa_formulário_cep();
                        if (!mostrouAlerta) {
                            alert("CEP não encontrado.");
                            mostrouAlerta = true;
                        }
                    }
                });
            }
            else {
                if (!mostrouAlerta) {
                    alert("Formato de CEP inválido.");
                    mostrouAlerta = true;
                }
            }
        }
        else {
            limpa_formulário_cep();
        }
    });
});
function limpa_formulário_cep() {
    // Limpa valores do formulário de cep.
    $("#Endereco").val("");
    $("#Bairro").val("");
    $("#Cidade").val("");
    $("#Estado").val("");
}

function validarCPF(cpf) {
    // Remove todos os caracteres que não sejam dígitos
    cpf = cpf.replace(/\D/g, '');

    // Verifica se o CPF possui 11 dígitos
    if (cpf.length !== 11) {
        return false;
    }

    // Verifica se todos os dígitos são iguais, o que não é válido para um CPF
    var digits = [...cpf].map(Number);
    if (digits.every(digit => digit === digits[0])) {
        return false;
    }

    // Calcula o primeiro dígito verificador
    var sum = 0;
    for (var i = 0; i < 9; i++) {
        sum += digits[i] * (10 - i);
    }
    var firstDigit = (sum * 10) % 11;
    if (firstDigit === 10) {
        firstDigit = 0;
    }

    // Calcula o segundo dígito verificador
    sum = 0;
    for (var i = 0; i < 10; i++) {
        sum += digits[i] * (11 - i);
    }
    var secondDigit = (sum * 10) % 11;
    if (secondDigit === 10) {
        secondDigit = 0;
    }

    // Verifica se os dígitos verificadores calculados são iguais aos informados
    if (firstDigit !== digits[9] || secondDigit !== digits[10]) {
        return false;
    }

    return true; // CPF válido
}

function validarCNPJ(cnpj) {
    // Remove caracteres não numéricos
    cnpj = cnpj.replace(/[^\d]+/g, '');

    // Verifica se o CNPJ tem 14 dígitos
    if (cnpj.length !== 14) {
        return false;
    }

    // Verifica se todos os dígitos são iguais (ex.: 00000000000000)
    if (/^(\d)\1{13}$/.test(cnpj)) {
        return false;
    }

    // Calcula o primeiro dígito verificador
    let soma = 0;
    let peso = 5;
    for (let i = 0; i < 12; i++) {
        soma += cnpj.charAt(i) * peso;
        peso = peso === 2 ? 9 : peso - 1;
    }
    let resto = soma % 11;
    let digito1 = resto < 2 ? 0 : 11 - resto;

    // Verifica o primeiro dígito verificador
    if (cnpj.charAt(12) != digito1) {
        return false;
    }

    // Calcula o segundo dígito verificador
    soma = 0;
    peso = 6;
    for (let i = 0; i < 13; i++) {
        soma += cnpj.charAt(i) * peso;
        peso = peso === 2 ? 9 : peso - 1;
    }
    resto = soma % 11;
    let digito2 = resto < 2 ? 0 : 11 - resto;

    // Verifica o segundo dígito verificador
    if (cnpj.charAt(13) != digito2) {
        return false;
    }

    return true;
}
function printDiv(divId) {
    var printContents = document.getElementById(divId).innerHTML;
    var originalContents = document.body.innerHTML;

    document.body.innerHTML = printContents;

    window.print();

    document.body.innerHTML = originalContents;
}


async function carregarMunicipios(estado) {
    try {
        const response = await fetch(
            `https://servicodados.ibge.gov.br/api/v1/localidades/estados/${estado}/municipios`
        );

        if (!response.ok) {
            throw new Error('Erro ao carregar municípios');
        }

        const data = await response.json();
        for (let i = 0; i < data.length; i++) {
            data[i].nome = data[i].nome.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
        }
        return data.sort((a, b) => a.nome.localeCompare(b.nome));
    } catch (error) {
        console.error('Erro:', error);
        throw error;
    }
}


// Reusable function for temporary alerts
function initializeTemporaryAlerts(timeout = 5000) {
    $('.temporary-alert').each(function () {
        const $alert = $(this);
        setTimeout(function () {
            $alert.fadeOut(500, function () {
                $(this).remove();
            });
        }, timeout);
    });
}
