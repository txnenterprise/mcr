namespace MCR.API.Shared.Extensions;

public static class NumericExtensions
{
    public static string FormatAreaHectaresVisualizacao(this decimal value)
    {
        return value.ToString("N2");
    }

    public static string FormatarMoeda(this decimal value)
    {
        return value.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
    }
}
