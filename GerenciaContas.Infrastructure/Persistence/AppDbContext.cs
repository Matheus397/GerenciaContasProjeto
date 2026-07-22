using GerenciaContas.Domain.Entities;
using GerenciaContas.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GerenciaContas.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Conta> Contas => Set<Conta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conta>(conta =>
        {
            conta.HasKey(c => c.Id);
            conta.Property(c => c.NomeTitular).IsRequired().HasMaxLength(200);
            conta.Property(c => c.Ativa).IsRequired();

            conta.Property(c => c.Cpf)
                 .HasConversion(cpf => cpf.Numero, valor => Cpf.Criar(valor))
                 .IsRequired();

            conta.Ignore(c => c.DomainEvents);
        });
    }
}
