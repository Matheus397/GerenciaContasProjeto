using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.Events;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Tests.Domain;

public class ContaTests
{
    private static Cpf CpfValido() => Cpf.Criar("52998224725");

    [Fact]
    public void Criar_ContaValida_NasceAtivaERegistraEvento()
    {
        var conta = Conta.Criar("Maria Souza", CpfValido());

        Assert.True(conta.Ativa);
        Assert.Equal("Maria Souza", conta.NomeTitular);
        Assert.Single(conta.DomainEvents);
        Assert.IsType<ContaCriadaEvent>(conta.DomainEvents.First());
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Criar_ComNomeInvalido_LancaDomainException(string nome)
    {
        Assert.Throws<DomainException>(() => Conta.Criar(nome, CpfValido()));
    }

    [Fact]
    public void Atualizar_AlteraDadosERegistraEvento()
    {
        var conta = Conta.Criar("Maria Souza", CpfValido());
        conta.ClearDomainEvents();

        conta.Atualizar("Maria S. Souza", false);

        Assert.Equal("Maria S. Souza", conta.NomeTitular);
        Assert.False(conta.Ativa);
        Assert.IsType<ContaAtualizadaEvent>(conta.DomainEvents.Single());
    }

    [Fact]
    public void Atualizar_SomenteStatus_MantemNome()
    {
        var conta = Conta.Criar("Maria Souza", CpfValido());
        conta.ClearDomainEvents();

        conta.Atualizar(null, false);

        Assert.Equal("Maria Souza", conta.NomeTitular);
        Assert.False(conta.Ativa);
    }

    [Fact]
    public void MarcarComoRemovida_RegistraEventoDeDelecao()
    {
        var conta = Conta.Criar("Maria Souza", CpfValido());
        conta.ClearDomainEvents();

        conta.MarcarComoRemovida();

        Assert.IsType<ContaDeletadaEvent>(conta.DomainEvents.Single());
    }
}
