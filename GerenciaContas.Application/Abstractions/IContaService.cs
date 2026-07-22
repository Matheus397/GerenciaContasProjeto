using GerenciaContas.Application.Common;
using GerenciaContas.Application.DTOs;

namespace GerenciaContas.Application.Abstractions;

public interface IContaService
{
    Task<Result<ContaResponse>> CriarAsync(CriarContaRequest request, CancellationToken ct = default);
    Task<ContaResponse?> ObterPorCpfAsync(string cpf, CancellationToken ct = default);
    Task<IReadOnlyList<ContaResponse>> ListarAsync(CancellationToken ct = default);
    Task<ContaResponse?> AtualizarAsync(string cpf, AtualizarContaRequest request, CancellationToken ct = default);
    Task<bool> RemoverAsync(string cpf, CancellationToken ct = default);
}
