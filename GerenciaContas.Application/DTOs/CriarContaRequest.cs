using System.ComponentModel.DataAnnotations;
using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Application.DTOs;

public sealed record CriarContaRequest(
    [property: Required] string NomeTitular,
    [property: Required]
    [property: RegularExpression(@"^(\d{11}|\d{3}\.\d{3}\.\d{3}-\d{2})$",
        ErrorMessage = "CPF deve ter 11 dígitos, com ou sem máscara (000.000.000-00).")]
    string Cpf,
    [property: EnumDataType(typeof(StatusConta))] StatusConta Status = StatusConta.Ativa);
