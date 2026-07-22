namespace GerenciaContas.Application.DTOs;

public sealed record AtualizarContaRequest(
    string? NomeTitular = null,
    bool? Ativa = null);
