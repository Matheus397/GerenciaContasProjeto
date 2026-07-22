# GerenciaContas — API de Contas (Banco KRT)

API REST em **.NET 10** para o CRUD de contas de clientes (Id, Nome do titular, CPF e Status).
Feita com **DDD + Clean Architecture + MVC**, seguindo SOLID.

## Camadas

- **Domain** — entidade `Conta`, value object `Cpf`, enum `StatusConta`, eventos de domínio e `IContaRepository`.
- **Application** — casos de uso (`ContaService`) e DTOs.
- **Infrastructure** — EF Core InMemory, repositório, cache e handlers das áreas.
- **API** — `ContasController` (endpoints REST) e middleware de exceções.
- **Tests** — 22 testes xUnit (domínio + aplicação).

## Notificação das áreas

A `Conta` emite eventos de domínio ao ser criada/atualizada/deletada. O `ContaService`
os publica via `IDomainEventDispatcher` para os handlers registrados —
**Prevenção à Fraude** e **Cartões** —, hoje simulados com log e prontos para
virar mensageria (ex.: SNS/SQS) em produção.

## Cache (custo de consultas)

`CachedContaRepository` é um _decorator_ sobre o repositório (padrão Cache-Aside):
uma conta já consultada volta do cache em memória, sem nova consulta ao banco.
A entrada expira no fim do dia; escritas invalidam o cache. Em produção, basta
trocar o `IMemoryCache` por um cache distribuído (Redis/ElastiCache).

## Endpoints

| Método | Rota               | Respostas     |
|--------|--------------------|---------------|
| GET    | `/api/contas`      | 200           |
| GET    | `/api/contas/{id}` | 200, 404      |
| POST   | `/api/contas`      | 201, 400, 409 |
| PUT    | `/api/contas/{id}` | 200, 400, 404 |
| DELETE | `/api/contas/{id}` | 204, 404      |

Exemplos em [`GerenciaContas/GerenciaContas.http`](GerenciaContas/GerenciaContas.http).

## Rodando

```bash
dotnet run --project GerenciaContas
dotnet test
```
