using GerenciaContas.Domain.Common;

namespace GerenciaContas.Domain.Events;

public sealed record ContaDeletadaEvent(Guid ContaId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
