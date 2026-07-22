using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.Repositories;
using GerenciaContas.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GerenciaContas.Infrastructure.Caching;

public sealed class CachedContaRepository : IContaRepository
{
    private const string PrefixoConta = "conta:";

    private readonly IContaRepository _inner;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedContaRepository> _logger;

    public CachedContaRepository(
        IContaRepository inner,
        IMemoryCache cache,
        ILogger<CachedContaRepository> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Conta?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var chave = ChaveDe(id);

        if (_cache.TryGetValue(chave, out Conta? emCache))
        {
            _logger.LogInformation("Cache HIT para conta {ContaId} (consulta ao banco evitada).", id);
            return emCache;
        }

        _logger.LogInformation("Cache MISS para conta {ContaId} (consultando o banco).", id);
        var conta = await _inner.ObterPorIdAsync(id, ct);

        if (conta is not null)
            _cache.Set(chave, conta, OpcoesExpiracaoFimDoDia());

        return conta;
    }

    public Task<IReadOnlyList<Conta>> ListarAsync(CancellationToken ct = default) =>
        _inner.ListarAsync(ct);

    public Task<bool> ExisteCpfAsync(Cpf cpf, CancellationToken ct = default) =>
        _inner.ExisteCpfAsync(cpf, ct);

    public async Task AdicionarAsync(Conta conta, CancellationToken ct = default)
    {
        await _inner.AdicionarAsync(conta, ct);
        _cache.Set(ChaveDe(conta.Id), conta, OpcoesExpiracaoFimDoDia());
    }

    public async Task AtualizarAsync(Conta conta, CancellationToken ct = default)
    {
        await _inner.AtualizarAsync(conta, ct);
        _cache.Set(ChaveDe(conta.Id), conta, OpcoesExpiracaoFimDoDia());
    }

    public async Task RemoverAsync(Conta conta, CancellationToken ct = default)
    {
        await _inner.RemoverAsync(conta, ct);
        _cache.Remove(ChaveDe(conta.Id));
    }

    private static string ChaveDe(Guid id) => PrefixoConta + id;

    private static MemoryCacheEntryOptions OpcoesExpiracaoFimDoDia()
    {
        var fimDoDia = DateTimeOffset.UtcNow.Date.AddDays(1);
        return new MemoryCacheEntryOptions { AbsoluteExpiration = fimDoDia };
    }
}
