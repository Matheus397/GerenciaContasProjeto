namespace GerenciaContas.Application.Common;

public sealed class ConflitoException : Exception
{
    public ConflitoException(string message) : base(message) { }
}
