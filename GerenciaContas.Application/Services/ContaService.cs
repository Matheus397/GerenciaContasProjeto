using GerenciaContas.Application.Abstractions;
using GerenciaContas.Application.Common;
using GerenciaContas.Application.DTOs;
using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.Repositories;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Application.Services;

public sealed class ContaService : IContaService
{
    private readonly IContaRepository _repository;
    private readonly IDomainEventDispatcher _dispatcher;

    public ContaService(IContaRepository repository, IDomainEventDispatcher dispatcher)
    {
        _repository = repository;
        _dispatcher = dispatcher;
    }

    public async Task<ContaResponse> CriarAsync(CriarContaRequest request, CancellationToken ct = default)
    {
        var cpf = Cpf.Criar(request.Cpf);

        if (await _repository.ExisteCpfAsync(cpf, ct))
            throw new ConflitoException($"Já existe uma conta para o CPF {cpf.Formatado()}.");

        var conta = Conta.Criar(request.NomeTitular, cpf, request.Status);

        await _repository.AdicionarAsync(conta, ct);
        await PublicarEventosAsync(conta, ct);

        return ContaResponse.De(conta);
    }

    public async Task<ContaResponse?> ObterPorCpfAsync(string cpf, CancellationToken ct = default)
    {
        var conta = await _repository.ObterPorCpfAsync(Cpf.Criar(cpf), ct);
        return conta is null ? null : ContaResponse.De(conta);
    }

    public async Task<IReadOnlyList<ContaResponse>> ListarAsync(CancellationToken ct = default)
    {
        var contas = await _repository.ListarAsync(ct);
        return contas.Select(ContaResponse.De).ToList();
    }

    public async Task<ContaResponse?> AtualizarAsync(string cpf, AtualizarContaRequest request, CancellationToken ct = default)
    {
        var conta = await _repository.ObterPorCpfAsync(Cpf.Criar(cpf), ct);
        if (conta is null) return null;

        conta.Atualizar(request.NomeTitular, request.Status);

        await _repository.AtualizarAsync(conta, ct);
        await PublicarEventosAsync(conta, ct);

        return ContaResponse.De(conta);
    }

    public async Task<bool> RemoverAsync(string cpf, CancellationToken ct = default)
    {
        var conta = await _repository.ObterPorCpfAsync(Cpf.Criar(cpf), ct);
        if (conta is null) return false;

        conta.MarcarComoRemovida();

        await _repository.RemoverAsync(conta, ct);
        await PublicarEventosAsync(conta, ct);

        return true;
    }

    private async Task PublicarEventosAsync(Conta conta, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync(conta.DomainEvents, ct);
        conta.ClearDomainEvents();
    }
}
