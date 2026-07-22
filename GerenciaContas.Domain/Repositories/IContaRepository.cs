using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Domain.Repositories;

public interface IContaRepository
{
    Task<Conta?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Conta>> ListarAsync(CancellationToken ct = default);
    Task<bool> ExisteCpfAsync(Cpf cpf, CancellationToken ct = default);
    Task AdicionarAsync(Conta conta, CancellationToken ct = default);
    Task AtualizarAsync(Conta conta, CancellationToken ct = default);
    Task RemoverAsync(Conta conta, CancellationToken ct = default);
}
