using GerenciaContas.Application.Abstractions;
using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Events;
using Microsoft.Extensions.Logging;

namespace GerenciaContas.Infrastructure.Events.Handlers;

public sealed class PrevencaoFraudeHandler : IEventoContaHandler
{
    private readonly ILogger<PrevencaoFraudeHandler> _logger;

    public PrevencaoFraudeHandler(ILogger<PrevencaoFraudeHandler> logger) => _logger = logger;

    public Task HandleAsync(IDomainEvent evento, CancellationToken ct = default)
    {
        switch (evento)
        {
            case ContaCriadaEvent e:
                _logger.LogInformation(
                    "[Fraude] Conta {ContaId} liberada para transacionar (titular {Titular}).",
                    e.ContaId, e.NomeTitular);
                break;
            case ContaDeletadaEvent e:
                _logger.LogInformation(
                    "[Fraude] Conta {ContaId} removida; bloquear novas transações.", e.ContaId);
                break;
        }

        return Task.CompletedTask;
    }
}
