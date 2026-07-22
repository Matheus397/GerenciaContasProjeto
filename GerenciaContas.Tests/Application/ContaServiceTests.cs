using GerenciaContas.Application.Common;
using GerenciaContas.Application.DTOs;
using GerenciaContas.Application.Services;
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

        Assert.True(resposta.Sucedido);
        Assert.NotEqual(Guid.Empty, resposta.Valor!.Id);
        Assert.True(resposta.Valor.Ativa);
        Assert.Contains(_dispatcher.EventosPublicados, e => e is ContaCriadaEvent);
    }

    [Fact]
    public async Task CriarAsync_ComCpfDuplicado_RetornaConflito()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var resultado = await _service.CriarAsync(new CriarContaRequest("Outro Nome", CpfValido));

        Assert.Equal(ResultStatus.Conflito, resultado.Status);
        Assert.Null(resultado.Valor);
    }

    [Fact]
    public async Task ObterPorCpfAsync_QuandoNaoExiste_RetornaNull()
    {
        Assert.Null(await _service.ObterPorCpfAsync(OutroCpfValido));
    }

    [Fact]
    public async Task ObterPorCpfAsync_QuandoExiste_RetornaConta()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var conta = await _service.ObterPorCpfAsync(CpfValido);

        Assert.NotNull(conta);
        Assert.Equal("João Silva", conta!.NomeTitular);
    }

    [Fact]
    public async Task AtualizarAsync_ContaExistente_AtualizaEPublicaEvento()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var atualizada = await _service.AtualizarAsync(
            CpfValido, new AtualizarContaRequest("João P. Silva", false));

        Assert.NotNull(atualizada);
        Assert.Equal("João P. Silva", atualizada!.NomeTitular);
        Assert.False(atualizada.Ativa);
        Assert.Contains(_dispatcher.EventosPublicados, e => e is ContaAtualizadaEvent);
    }

    [Fact]
    public async Task AtualizarAsync_SomenteStatus_MantemNomeEInativa()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var atualizada = await _service.AtualizarAsync(
            CpfValido, new AtualizarContaRequest(Ativa: false));

        Assert.NotNull(atualizada);
        Assert.Equal("João Silva", atualizada!.NomeTitular);
        Assert.False(atualizada.Ativa);
    }

    [Fact]
    public async Task AtualizarAsync_ContaInexistente_RetornaNull()
    {
        var resultado = await _service.AtualizarAsync(
            OutroCpfValido, new AtualizarContaRequest("Nome", true));

        Assert.Null(resultado);
    }

    [Fact]
    public async Task RemoverAsync_ContaExistente_RemoveEPublicaEvento()
    {
        await _service.CriarAsync(new CriarContaRequest("João Silva", CpfValido));

        var removida = await _service.RemoverAsync(CpfValido);

        Assert.True(removida);
        Assert.Null(await _service.ObterPorCpfAsync(CpfValido));
        Assert.Contains(_dispatcher.EventosPublicados, e => e is ContaDeletadaEvent);
    }

    [Fact]
    public async Task RemoverAsync_ContaInexistente_RetornaFalse()
    {
        Assert.False(await _service.RemoverAsync(OutroCpfValido));
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
