using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Enums;

namespace GerenciaContas.Domain.Events;

public sealed record ContaAtualizadaEvent(Guid ContaId, string NomeTitular, StatusConta Status)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
