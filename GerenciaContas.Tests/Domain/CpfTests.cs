using GerenciaContas.Domain.Common;
using GerenciaContas.Domain.ValueObjects;

namespace GerenciaContas.Tests.Domain;

public class CpfTests
{
    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("52998224725", "52998224725")]
    public void Criar_ComCpfValido_RemoveMascaraEArmazenaDigitos(string entrada, string esperado)
    {
        var cpf = Cpf.Criar(entrada);
        Assert.Equal(esperado, cpf.Numero);
    }

    [Fact]
    public void Formatado_RetornaComMascara()
    {
        var cpf = Cpf.Criar("52998224725");
        Assert.Equal("529.982.247-25", cpf.Formatado());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("11111111111")]
    [InlineData("52998224724")]
    public void Criar_ComCpfInvalido_LancaDomainException(string? entrada)
    {
        Assert.Throws<DomainException>(() => Cpf.Criar(entrada));
    }

    [Fact]
    public void Equals_ComMesmoNumero_SaoIguais()
    {
        Assert.Equal(Cpf.Criar("52998224725"), Cpf.Criar("529.982.247-25"));
    }
}
