using GerenciaContas.Application.Abstractions;
using GerenciaContas.Domain.Common;

namespace GerenciaContas.Tests.Fakes;

public sealed class FakeEventDispatcher : IDomainEventDispatcher
{
    public List<IDomainEvent> EventosPublicados { get; } = new();

    public Task DispatchAsync(IEnumerable<IDomainEvent> eventos, CancellationToken ct = default)
    {
        EventosPublicados.AddRange(eventos);
        return Task.CompletedTask;
    }
}
