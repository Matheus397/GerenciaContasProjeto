using GerenciaContas.Application.DTOs;

namespace GerenciaContas.Application.Abstractions;

public interface IContaService
{
    Task<ContaResponse> CriarAsync(CriarContaRequest request, CancellationToken ct = default);
    Task<ContaResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ContaResponse>> ListarAsync(CancellationToken ct = default);
    Task<ContaResponse?> AtualizarAsync(Guid id, AtualizarContaRequest request, CancellationToken ct = default);
    Task<bool> RemoverAsync(Guid id, CancellationToken ct = default);
}
