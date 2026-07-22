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

    public async Task<Conta?> ObterPorCpfAsync(Cpf cpf, CancellationToken ct = default)
    {
        var chave = ChaveDe(cpf);

        if (_cache.TryGetValue(chave, out Conta? emCache))
        {
            _logger.LogInformation("Cache HIT para CPF {Cpf} (consulta ao banco evitada).", cpf.Numero);
            return emCache;
        }

        _logger.LogInformation("Cache MISS para CPF {Cpf} (consultando o banco).", cpf.Numero);
        var conta = await _inner.ObterPorCpfAsync(cpf, ct);

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
        _cache.Set(ChaveDe(conta.Cpf), conta, OpcoesExpiracaoFimDoDia());
    }

    public async Task AtualizarAsync(Conta conta, CancellationToken ct = default)
    {
        await _inner.AtualizarAsync(conta, ct);
        _cache.Set(ChaveDe(conta.Cpf), conta, OpcoesExpiracaoFimDoDia());
    }

    public async Task RemoverAsync(Conta conta, CancellationToken ct = default)
    {
        await _inner.RemoverAsync(conta, ct);
        _cache.Remove(ChaveDe(conta.Cpf));
    }

    private static string ChaveDe(Cpf cpf) => PrefixoConta + cpf.Numero;

    private static MemoryCacheEntryOptions OpcoesExpiracaoFimDoDia()
    {
        var fimDoDia = DateTimeOffset.UtcNow.Date.AddDays(1);
        return new MemoryCacheEntryOptions { AbsoluteExpiration = fimDoDia };
    }
}
