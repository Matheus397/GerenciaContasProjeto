using System.ComponentModel;

namespace GerenciaContas.Application.DTOs;

public sealed record AtualizarContaRequest(
    [property: DefaultValue("Maria Souza")] string? NomeTitular = null,
    [property: DefaultValue(false)] bool? Ativa = null);
