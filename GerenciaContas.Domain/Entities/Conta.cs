using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Enums;
using GerenciaContas.Domain.Events;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Domain.Entities;

public sealed class Conta : Entity
{
    public string NomeTitular { get; private set; }
    public Cpf Cpf { get; private set; }
    public StatusConta Status { get; private set; }

    private Conta()
    {
        NomeTitular = string.Empty;
        Cpf = null!;
    }

    private Conta(string nomeTitular, Cpf cpf, StatusConta status)
    {
        NomeTitular = nomeTitular;
        Cpf = cpf;
        Status = status;
    }

    public static Conta Criar(string nomeTitular, Cpf cpf, StatusConta status = StatusConta.Ativa)
    {
        ValidarNome(nomeTitular);

        var conta = new Conta(nomeTitular.Trim(), cpf, status);
        conta.RaiseDomainEvent(new ContaCriadaEvent(conta.Id, conta.NomeTitular, cpf.Numero));
        return conta;
    }

    public void Atualizar(string nomeTitular, StatusConta status)
    {
        ValidarNome(nomeTitular);

        NomeTitular = nomeTitular.Trim();
        Status = status;
        RaiseDomainEvent(new ContaAtualizadaEvent(Id, NomeTitular, Status));
    }

    public void Inativar()
    {
        if (Status == StatusConta.Inativa) return;
        Status = StatusConta.Inativa;
        RaiseDomainEvent(new ContaAtualizadaEvent(Id, NomeTitular, Status));
    }

    public void MarcarComoRemovida() => RaiseDomainEvent(new ContaDeletadaEvent(Id));

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do titular é obrigatório.");

        if (nome.Trim().Length < 3)
            throw new DomainException("Nome do titular deve ter ao menos 3 caracteres.");
    }
}
