using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Events;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Domain.Entities;

public sealed class Conta : Entity
{
    public string NomeTitular { get; private set; }
    public Cpf Cpf { get; private set; }
    public bool Ativa { get; private set; }

    private Conta()
    {
        NomeTitular = string.Empty;
        Cpf = null!;
    }

    private Conta(string nomeTitular, Cpf cpf, bool ativa)
    {
        NomeTitular = nomeTitular;
        Cpf = cpf;
        Ativa = ativa;
    }

    public static Conta Criar(string nomeTitular, Cpf cpf, bool ativa = true)
    {
        ValidarNome(nomeTitular);

        var conta = new Conta(nomeTitular.Trim(), cpf, ativa);
        conta.RaiseDomainEvent(new ContaCriadaEvent(conta.Id, conta.NomeTitular, cpf.Numero));
        return conta;
    }

    public void Atualizar(string? nomeTitular, bool? ativa)
    {
        if (!string.IsNullOrWhiteSpace(nomeTitular))
        {
            ValidarNome(nomeTitular);
            NomeTitular = nomeTitular.Trim();
        }

        if (ativa.HasValue)
            Ativa = ativa.Value;

        RaiseDomainEvent(new ContaAtualizadaEvent(Id, NomeTitular, Ativa));
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
