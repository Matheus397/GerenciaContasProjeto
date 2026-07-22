using GerenciaContas.Application.Abstractions;
using GerenciaContas.Application.Services;
using GerenciaContas.Domain.Repositories;
using GerenciaContas.Infrastructure.Caching;
using GerenciaContas.Infrastructure.Events;
using GerenciaContas.Infrastructure.Events.Handlers;
using GerenciaContas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GerenciaContas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGerenciaContas(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("GerenciaContasDb"));

        services.AddMemoryCache();

        services.AddScoped<ContaRepository>();
        services.AddScoped<IContaRepository>(sp => new CachedContaRepository(
            sp.GetRequiredService<ContaRepository>(),
            sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>(),
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CachedContaRepository>>()));

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventoContaHandler, PrevencaoFraudeHandler>());
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IEventoContaHandler, CartoesHandler>());

        services.AddScoped<IContaService, ContaService>();

        return services;
    }
}
