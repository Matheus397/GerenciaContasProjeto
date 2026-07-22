using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.Repositories;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Tests.Fakes;

public sealed class FakeContaRepository : IContaRepository
{
    private readonly Dictionary<Guid, Conta> _contas = new();

    public Task<Conta?> ObterPorCpfAsync(Cpf cpf, CancellationToken ct = default) =>
        Task.FromResult(_contas.Values.FirstOrDefault(c => c.Cpf == cpf));

    public Task<IReadOnlyList<Conta>> ListarAsync(CancellationToken ct = default) =>
        Task.FromResult((IReadOnlyList<Conta>)_contas.Values.ToList());

    public Task<bool> ExisteCpfAsync(Cpf cpf, CancellationToken ct = default) =>
        Task.FromResult(_contas.Values.Any(c => c.Cpf == cpf));

    public Task AdicionarAsync(Conta conta, CancellationToken ct = default)
    {
        _contas[conta.Id] = conta;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Conta conta, CancellationToken ct = default)
    {
        _contas[conta.Id] = conta;
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Conta conta, CancellationToken ct = default)
    {
        _contas.Remove(conta.Id);
        return Task.CompletedTask;
    }
}
