using GerenciaContas.Application.Common;
using GerenciaContas.Application.DTOs;
using GerenciaContas.Application.Services;
using GerenciaContas.Domain.Enums;
using GerenciaContas.Domain.Events;
using GerenciaContas.Tests.Fakes;

namespace GerenciaContas.Tests.Application;

public class ContaServiceTests
{
    private const string CpfValido = "52998224725";
    private const string OutroCpfValido = "11144477735";


    private readonly FakeContaRepository _repo = new();
    private readonly FakeEventDispatcher _dispatcher = new();
    private readonly ContaService _service;

    public ContaServiceTests()
    {
        _service = new ContaService(_repo, _dispatcher);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_PersisteEPublicaEvento()
    {
        var resposta = await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        Assert.NotEqual(Guid.Empty, resposta.Id);
        Assert.Equal(StatusConta.Ativa, resposta.Status);
        Assert.Contains(_dispatcher.EventosPublicados, e => e is ContaCriadaEvent);
    }

    [Fact]
    public async Task CriarAsync_ComCpfDuplicado_LancaConflito()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        await Assert.ThrowsAsync<ConflitoException>(() =>
            _service.CriarAsync(new CriarContaRequest("Outro Nome", CpfValido)));
    }

    [Fact]
    public async Task ObterPorIdAsync_QuandoNaoExiste_RetornaNull()
    {
        Assert.Null(await _service.ObterPorIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task AtualizarAsync_ContaExistente_AtualizaEPublicaEvento()
    {
        var criada = await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var atualizada = await _service.AtualizarAsync(
            criada.Id, new AtualizarContaRequest("João P. Silva", StatusConta.Inativa));

        Assert.NotNull(atualizada);
        Assert.Equal("João P. Silva", atualizada!.NomeTitular);
        Assert.Equal(StatusConta.Inativa, atualizada.Status);
        Assert.Contains(_dispatcher.EventosPublicados, e => e is ContaAtualizadaEvent);
    }

    [Fact]
    public async Task AtualizarAsync_ContaInexistente_RetornaNull()
    {
        var resultado = await _service.AtualizarAsync(
            Guid.NewGuid(), new AtualizarContaRequest("Nome", StatusConta.Ativa));

        Assert.Null(resultado);
    }

    [Fact]
    public async Task RemoverAsync_ContaExistente_RemoveEPublicaEvento()
    {
        var criada = await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var removida = await _service.RemoverAsync(criada.Id);

        Assert.True(removida);
        Assert.Null(await _service.ObterPorIdAsync(criada.Id));
        Assert.Contains(_dispatcher.EventosPublicados, e => e is ContaDeletadaEvent);
    }

    [Fact]
    public async Task RemoverAsync_ContaInexistente_RetornaFalse()
    {
        Assert.False(await _service.RemoverAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ListarAsync_RetornaTodasAsContas()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));
        await _service.CriarAsync(new CriarContaRequest("Maria Souza", OutroCpfValido));

        var contas = await _service.ListarAsync();

        Assert.Equal(2, contas.Count);
    }
}
