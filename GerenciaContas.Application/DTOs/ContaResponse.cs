using GerenciaContas.Domain.Entities;

namespace GerenciaContas.Application.DTOs;

public sealed record ContaResponse(Guid Id, string NomeTitular, string Cpf, bool Ativa)
{
    public static ContaResponse De(Conta conta) =>
        new(conta.Id, conta.NomeTitular, conta.Cpf.Formatado(), conta.Ativa);
}
