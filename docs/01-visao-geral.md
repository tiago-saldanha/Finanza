# 01 — Visão Geral e Arquitetura

## O que é o Finanza

O Finanza é uma aplicação web de gestão financeira pessoal que permite controlar receitas, despesas, transferências, investimentos, metas, empréstimos a terceiros e patrimônio. Inclui ferramentas de planejamento (regra 50-30-20) e simulação de independência financeira (FIRE).

---

## Stack tecnológica

| Camada | Tecnologia |
|---|---|
| Backend | .NET 8 (ASP.NET Core Minimal API) |
| Frontend | Angular 18 (standalone components, signals) |
| Banco de dados | SQLite por tenant (um arquivo `.db` por usuário) |
| ORM | Entity Framework Core 8 |
| Autenticação | JWT Bearer Token |
| E-mail | Resend API |
| UI components | Angular Material |
| Gráficos | Chart.js via ng2-charts |

---

## Arquitetura do backend — Clean Architecture

```
src/
├── Finanza.Domain/          # Entidades, enums, value objects, interfaces de repositório
├── Finanza.Application/     # Serviços de aplicação, DTOs, interfaces
├── Finanza.Infrastructure/  # EF Core, repositórios, multi-tenancy, migrações
└── Finanza.API/             # Minimal API endpoints, DI, configuração
```

### Camadas

**Domain** — núcleo do negócio, sem dependências externas.
- `Entities/` — entidades com lógica de negócio encapsulada
- `Enums/` — tipos enumerados do domínio
- `ValueObjects/` — `Money`, `Description`, `TransactionDates`
- `Repositories/` — interfaces (contratos) dos repositórios

**Application** — orquestra casos de uso.
- `Services/` — serviços de aplicação (lógica de orquestração)
- `DTOs/Requests/` — objetos de entrada da API
- `DTOs/Responses/` — objetos de saída da API
- `Mapper/` — mapeamento entre DTOs e entidades de domínio

**Infrastructure** — implementações concretas.
- `Data/TenantDbContext.cs` — contexto EF Core com isolamento por tenant; usa `ApplyConfigurationsFromAssembly` para registrar todas as configurações automaticamente
- `Data/Configurations/` — classes `IEntityTypeConfiguration<T>` com Fluent API (uma por entidade)
- `Repositories/` — implementações dos repositórios
- `Tenancy/` — provisionamento e migração automática de bancos de tenant
- `Migrations/` — migration única `InitialCreate` que cria todas as tabelas do schema completo

**API** — ponto de entrada HTTP.
- `Endpoints/` — grupos de endpoints por feature
- `Extensions/` — registro de DI, middlewares, CORS

---

## Multi-tenancy

Cada usuário registrado recebe um banco SQLite exclusivo em:
```
src/Finanza.Infrastructure/Tenants/user_{userId}.db
```

O banco principal (`app.db`) armazena apenas os dados de usuários (autenticação). Todos os dados financeiros ficam no banco do tenant.

### Ciclo de vida dos bancos de tenant

| Situação | Mecanismo |
|---|---|
| Novo usuário registrado | `TenantProvisionerService.ProvisionTenant()` → `context.Database.Migrate()` cria todas as tabelas via `InitialCreate` |
| Banco existente no startup | `TenantMigrationStartupService` aplica schema incremental via SQL e registra a migration `InitialCreate` no histórico do EF |
| Banco de identidade (`app.db`) | `AppDbContext.Database.EnsureCreated()` no startup da API |

---

## Estrutura do frontend

```
src/Finanza.Client/src/app/
├── core/
│   ├── models/       # interfaces TypeScript (Transaction, Goal, Loan, etc.)
│   ├── services/     # serviços HTTP + FilterStateService
│   └── guards/       # authGuard (JWT)
├── features/
│   ├── auth/         # login, register, forgot-password, reset-password
│   ├── dashboard/    # visão consolidada
│   ├── transactions/ # listagem + formulário + pagamento
│   ├── categories/   # categorias
│   ├── financial-accounts/ # contas
│   ├── investments/  # portfólio
│   ├── goals/        # metas
│   ├── loans/        # empréstimos
│   ├── patrimony/    # ativos e passivos
│   ├── patrimony-snapshots/ # histórico de patrimônio
│   ├── planning/     # planejamento financeiro
│   ├── fire/         # independência financeira
│   └── account/      # minha conta
├── layout/           # shell com sidebar e toolbar
└── shared/           # componentes reutilizáveis (confirm-dialog, currency-mask)
```

---

## Persistência de estado (filtros)

O `FilterStateService` salva os filtros aplicados pelo usuário no `localStorage` do navegador. Os filtros persistem entre navegações e sessões do browser. As telas que utilizam persistência são:

- **Transações** — busca, status, tipo, data inicial, data final
- **Categorias** — busca, modo de período, datas customizadas
