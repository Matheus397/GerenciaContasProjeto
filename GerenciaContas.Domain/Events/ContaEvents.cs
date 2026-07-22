using GerenciaContas.Domain.Common;

namespace GerenciaContas.Domain.Events;

public sealed record ContaCriadaEvent(Guid ContaId, string NomeTitular, string Cpf) : DomainEvent;

public sealed record ContaAtualizadaEvent(Guid ContaId, string NomeTitular, bool Ativa) : DomainEvent;

public sealed record ContaDeletadaEvent(Guid ContaId) : DomainEvent;
