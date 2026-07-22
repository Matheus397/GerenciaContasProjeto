using System.ComponentModel.DataAnnotations;

namespace GerenciaContas.Application.DTOs;

public sealed record CriarContaRequest(
    [Required] string NomeTitular,
    [Required]
    [RegularExpression(@"^(\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$",
        ErrorMessage = "CPF deve ter 11 dígitos, com ou sem máscara (000.000.000-00).")]
    string Cpf,
    bool Ativa = true);
