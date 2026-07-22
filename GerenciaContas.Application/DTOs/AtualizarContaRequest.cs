using System.ComponentModel.DataAnnotations;
using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Application.DTOs;

public sealed record AtualizarContaRequest(
    [property: Required] string NomeTitular,
    [property: EnumDataType(typeof(StatusConta))] StatusConta Status);
