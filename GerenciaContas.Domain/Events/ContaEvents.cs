using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Domain.Events;

public sealed record ContaCriadaEvent(Guid ContaId, string NomeTitular, string Cpf) : DomainEvent;

public sealed record ContaAtualizadaEvent(Guid ContaId, string NomeTitular, StatusConta Status) : DomainEvent;

public sealed record ContaDeletadaEvent(Guid ContaId) : DomainEvent;
