using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Application.DTOs;

public sealed record AtualizarContaRequest(string NomeTitular, StatusConta Status);
