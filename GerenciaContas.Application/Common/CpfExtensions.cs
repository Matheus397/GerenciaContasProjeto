namespace GerenciaContas.Application.Common;

public static class CpfExtensions
{
    public static string SomenteDigitos(this string cpf) =>
        new(cpf.Where(char.IsDigit).ToArray());
}
