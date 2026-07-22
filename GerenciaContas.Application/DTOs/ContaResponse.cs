using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Application.DTOs;

public sealed record ContaResponse(Guid Id, string NomeTitular, string Cpf, StatusConta Status)
{
    public static ContaResponse De(Conta conta) =>
        new(conta.Id, conta.NomeTitular, conta.Cpf.Formatado(), conta.Status);
}
