# GerenciaContas — API de Contas (Banco KRT)

API REST em **.NET 10** para o CRUD de contas de clientes do time de Onboarding.
Cada conta possui **Id, Nome do titular, CPF e Status (Ativa/Inativa)**.

## Arquitetura (DDD + Clean Architecture + MVC)

O código é dividido em camadas, com as dependências sempre apontando para o
domínio (regra de dependência do Clean Architecture / **D** do SOLID):

```
GerenciaContas.Domain          → regras de negócio puras, sem dependências
  ├─ Entities/Conta            → Aggregate Root (invariantes + eventos)
  ├─ ValueObjects/Cpf          → validação de CPF, imutável, igualdade por valor
  ├─ Enums/StatusConta         → Ativa | Inativa
  ├─ Events/                   → ContaCriada / Atualizada / Deletada
  └─ Repositories/IContaRepository (contrato)

GerenciaContas.Application     → casos de uso
  ├─ Services/ContaService     → orquestra domínio + repositório + eventos
  ├─ DTOs/                      → contratos de entrada/saída
  └─ Abstractions/              → IContaService, IDomainEventDispatcher, handlers

GerenciaContas.Infrastructure  → detalhes técnicos
  ├─ Persistence/               → EF Core (InMemory) + ContaRepository
  ├─ Caching/CachedContaRepository → cache de consultas (ver abaixo)
  └─ Events/                    → dispatcher + handlers das áreas (Fraude, Cartões)

GerenciaContas (API)           → camada MVC
  ├─ Controllers/ContasController → endpoints REST finos
  └─ Middleware/                → tradução de exceções em ProblemDetails

GerenciaContas.Tests           → xUnit (domínio + aplicação)
```

**SOLID aplicado:** cada classe tem uma responsabilidade (SRP); o cache é um
_decorator_ que estende comportamento sem alterar o repositório (OCP); a
`IContaRepository` vive no domínio e é implementada na infra (DIP); as
interfaces são pequenas e focadas (ISP).

## Como as áreas do banco são notificadas (criada/atualizada/deletada)

A entidade `Conta` emite **eventos de domínio** a cada mudança de ciclo de vida.
Ao salvar, o `ContaService` publica esses eventos pelo `IDomainEventDispatcher`,
que os entrega aos _handlers_ registrados — um por área interessada:

- **Prevenção à Fraude** (`PrevencaoFraudeHandler`): ao saber que a conta foi
  criada, libera o cliente para transacionar (não ser barrado).
- **Cartões** (`CartoesHandler`): ao saber que a conta foi criada, avalia a
  emissão de um cartão de crédito.

Aqui a integração é simulada com log. Em produção, cada handler publicaria a
mensagem em um tópico/fila (ex.: **SNS/SQS**), desacoplando as áreas. Adicionar
uma nova área é só criar um novo handler e registrá-lo — sem tocar no CRUD.

## Pergunta de custo: evitando consultas repetidas ao banco no mesmo dia

> _"Como a AWS cobra por consulta ao banco, como evitar o custo de reconsultar
> uma conta que já foi consultada naquele mesmo dia?"_

**Resposta: cache (padrão Cache-Aside) via _decorator_.**

`CachedContaRepository` embrulha o `ContaRepository` real. No `ObterPorIdAsync`:

1. Procura a conta no cache em memória (`IMemoryCache`).
2. **Cache HIT** → devolve sem tocar no banco (consulta — e custo — evitada).
3. **Cache MISS** → busca no banco **uma vez**, guarda no cache e devolve.

A entrada expira no **fim do dia corrente** (`AbsoluteExpiration` = próxima
meia-noite), atendendo literalmente ao "naquele mesmo dia": no dia seguinte a
conta é buscada no banco ao menos uma vez. As operações de escrita
(criar/atualizar/remover) atualizam ou invalidam a entrada, mantendo a
consistência.

Por ser um _decorator_ atrás da mesma `IContaRepository`, nem o serviço nem o
controller sabem que há cache. **Em produção com múltiplas instâncias**, basta
trocar o `IMemoryCache` por um cache distribuído (**Redis/ElastiCache**),
mantendo a mesma interface e a mesma economia de consultas.

## Rodando

```bash
dotnet run --project GerenciaContas
```

- API: `http://localhost:5191` (perfil de desenvolvimento)

### Endpoints

| Método | Rota                | Descrição            | Respostas            |
|--------|---------------------|----------------------|----------------------|
| GET    | `/api/contas`       | Lista todas          | 200                  |
| GET    | `/api/contas/{id}`  | Obtém por Id         | 200, 404             |
| POST   | `/api/contas`       | Cria                 | 201, 400, 409        |
| PUT    | `/api/contas/{id}`  | Atualiza             | 200, 400, 404        |
| DELETE | `/api/contas/{id}`  | Remove               | 204, 404             |

Exemplos prontos em [`GerenciaContas/GerenciaContas.http`](GerenciaContas/GerenciaContas.http).

## Testes

```bash
dotnet test
```

22 testes cobrindo validação de CPF, invariantes da `Conta` e todos os casos de
uso do `ContaService` (criação, duplicidade, atualização, remoção, listagem e
publicação de eventos).

## Banco de dados

Foi usado **EF Core InMemory** para o teste rodar sem infraestrutura externa. A
troca por um banco real (SQL Server/Postgres) exige apenas mudar o provider no
`DependencyInjection.AddGerenciaContas` — o restante do código não muda.
