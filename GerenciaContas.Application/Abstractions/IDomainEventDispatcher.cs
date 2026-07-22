using GerenciaContas.Domain.Common;

namespace GerenciaContas.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> eventos, CancellationToken ct = default);
}
