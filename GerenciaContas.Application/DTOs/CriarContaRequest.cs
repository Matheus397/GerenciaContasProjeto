using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Application.DTOs;

public sealed record CriarContaRequest(string NomeTitular, string Cpf, StatusConta Status = StatusConta.Ativa);
