using System.Text.RegularExpressions;
using GerenciaContas.Domain.Common;

namespace GerenciaContas.Domain.ValueObjects;

public sealed partial class Cpf : IEquatable<Cpf>
{
    public string Numero { get; }

    private Cpf(string numero) => Numero = numero;

    public static Cpf Criar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("CPF é obrigatório.");

        var digitos = SomenteDigitos().Replace(valor, string.Empty);

        if (digitos.Length != 11)
            throw new DomainException("CPF deve conter 11 dígitos.");

        if (!EhValido(digitos))
            throw new DomainException("CPF inválido.");

        return new Cpf(digitos);
    }

    public string Formatado() =>
        Convert.ToUInt64(Numero).ToString(@"000\.000\.000\-00");

    private static bool EhValido(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false;

        return DigitoVerificador(cpf, 9) == cpf[9]
            && DigitoVerificador(cpf, 10) == cpf[10];
    }

    private static char DigitoVerificador(string cpf, int posicao)
    {
        var soma = 0;
        var peso = posicao + 1;

        for (var i = 0; i < posicao; i++)
            soma += (cpf[i] - '0') * peso--;

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;
        return (char)('0' + digito);
    }

    public bool Equals(Cpf? other) => other is not null && Numero == other.Numero;
    public override bool Equals(object? obj) => Equals(obj as Cpf);
    public override int GetHashCode() => Numero.GetHashCode();
    public override string ToString() => Numero;

    public static bool operator ==(Cpf? left, Cpf? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(Cpf? left, Cpf? right) => !(left == right);

    [GeneratedRegex(@"\D")]
    private static partial Regex SomenteDigitos();
}
