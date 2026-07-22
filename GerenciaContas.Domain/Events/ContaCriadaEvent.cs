using GerenciaContas.Domain.Common;

namespace GerenciaContas.Domain.Events;

public sealed record ContaCriadaEvent(Guid ContaId, string NomeTitular, string Cpf)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
