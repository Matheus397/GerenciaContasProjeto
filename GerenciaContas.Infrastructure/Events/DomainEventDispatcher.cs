using GerenciaContas.Application.Abstractions;
using GerenciaContas.Domain.Common;

namespace GerenciaContas.Infrastructure.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IEnumerable<IEventoContaHandler> _handlers;

    public DomainEventDispatcher(IEnumerable<IEventoContaHandler> handlers) => _handlers = handlers;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> eventos, CancellationToken ct = default)
    {
        foreach (var evento in eventos)
            foreach (var handler in _handlers)
                await handler.HandleAsync(evento, ct);
    }
}
