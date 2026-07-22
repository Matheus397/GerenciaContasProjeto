namespace GerenciaContas.Application.Common;

public enum ResultStatus
{
    Sucesso,
    Conflito
}

public sealed class Result<T>
{
    public ResultStatus Status { get; }
    public T? Valor { get; }
    public string? Erro { get; }

    private Result(ResultStatus status, T? valor, string? erro)
    {
        Status = status;
        Valor = valor;
        Erro = erro;
    }

    public bool Sucedido => Status == ResultStatus.Sucesso;

    public static Result<T> Ok(T valor) => new(ResultStatus.Sucesso, valor, null);
    public static Result<T> Conflito(string erro) => new(ResultStatus.Conflito, default, erro);
}
