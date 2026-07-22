using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.Repositories;
using GerenciaContas.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GerenciaContas.Infrastructure.Persistence;

public sealed class ContaRepository : IContaRepository
{
    private readonly AppDbContext _context;

    public ContaRepository(AppDbContext context) => _context = context;

    public async Task<Conta?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Contas.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Conta>> ListarAsync(CancellationToken ct = default) =>
        await _context.Contas.AsNoTracking().ToListAsync(ct);

    public async Task<bool> ExisteCpfAsync(Cpf cpf, CancellationToken ct = default) =>
        await _context.Contas.AnyAsync(c => c.Cpf == cpf, ct);

    public async Task AdicionarAsync(Conta conta, CancellationToken ct = default)
    {
        await _context.Contas.AddAsync(conta, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Conta conta, CancellationToken ct = default)
    {
        _context.Contas.Update(conta);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Conta conta, CancellationToken ct = default)
    {
        _context.Contas.Remove(conta);
        await _context.SaveChangesAsync(ct);
    }
}
