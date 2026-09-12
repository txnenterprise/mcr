namespace MCR.API.Shared.Extensions;

public static class StringExtensions
{
    public static string OnlyNumbers(this string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return texto;

        return new string(texto.Where(char.IsDigit).ToArray());
    }

    public static string FormatCpfCnpj(this string cpfCnpj)
    {
        if (string.IsNullOrEmpty(cpfCnpj)) return cpfCnpj;
        var digits = cpfCnpj.OnlyNumbers();
        if (digits.Length == 11)
            return Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00");
        if (digits.Length == 14)
            return Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00");
        return cpfCnpj;
    }

    public static string FormatCelular(this string celular)
    {
        if (string.IsNullOrEmpty(celular)) return celular;
        var digits = celular.OnlyNumbers();
        if (digits.Length == 11)
            return $"({digits[..2]}) {digits[2..7]}-{digits[7..]}";
        if (digits.Length == 10)
            return $"({digits[..2]}) {digits[2..6]}-{digits[6..]}";
        return celular;
    }
}
