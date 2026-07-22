using GerenciaContas.Application.Abstractions;
using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Events;
using Microsoft.Extensions.Logging;

namespace GerenciaContas.Infrastructure.Events.Handlers;

public sealed class CartoesHandler : IEventoContaHandler
{
    private readonly ILogger<CartoesHandler> _logger;

    public CartoesHandler(ILogger<CartoesHandler> logger) => _logger = logger;

    public Task HandleAsync(IDomainEvent evento, CancellationToken ct = default)
    {
        if (evento is ContaCriadaEvent e)
        {
            _logger.LogInformation(
                "[Cartões] Conta {ContaId} criada; avaliar emissão de cartão para {Titular}.",
                e.ContaId, e.NomeTitular);
        }

        return Task.CompletedTask;
    }
}
