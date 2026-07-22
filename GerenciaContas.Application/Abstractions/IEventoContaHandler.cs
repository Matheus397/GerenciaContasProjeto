using GerenciaContas.Domain.Common;

namespace GerenciaContas.Application.Abstractions;

public interface IEventoContaHandler
{
    Task HandleAsync(IDomainEvent evento, CancellationToken ct = default);
}
