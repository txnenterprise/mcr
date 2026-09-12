$(document).ready(function () {
    $("#CNPJ").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, "");
        value = value.substring(0, 14);
        value = value.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})?$/, "$1.$2.$3/$4-$5");
        $(this).val(value);
    });
    $("#CPF").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, ""); // Remove caracteres n�o num�ricos
        value = value.substring(0, 11);

        // Formata��o do CPF
        value = value.replace(/(\d{3})(\d{3})(\d{1,3})/, "$1.$2.$3");
        value = value.replace(/(\d{3})(\d{1,3})$/, "$1-$2");

        // Atualiza o valor no campo de entrada
        $(this).val(value);

        // Realiza a valida��o apenas se o CPF tiver 11 d�gitos
        if (value.length === 14 && !validarCPF(value)) {
            alert("CPF inv�lido. Por favor, insira um CPF v�lido.");
            $("#CPF").val("");
        }
    });
    $("#Telefone").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, "");
        value = value.substring(0, 11);
        if (value.length <= 11) {
            value = value.replace(/^(\d{2})(\d{5})(\d{4})$/, "($1) $2-$3");
        }

        $(this).val(value);
    });
    $("#TelefoneVinculo").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, "");
        value = value.substring(0, 10);
        if (value.length <= 10) {
            value = value.replace(/^(\d{2})(\d{4})(\d{4})$/, "($1) $2-$3");
        }

        $(this).val(value);
    });
    $("#Celular").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, "");
        value = value.substring(0, 11);
        value = value.replace(/^(\d{2})(\d{5})(\d{4})$/, "($1) $2-$3");
        $(this).val(value);
    });
    $("#CelularVinculo").on("input", function () {
        let value = $(this).val();
        value = value.replace(/\D/g, "");
        value = value.substring(0, 11);
        value = value.replace(/^(\d{2})(\d{5})(\d{4})$/, "($1) $2-$3");
        $(this).val(value);
    });
});