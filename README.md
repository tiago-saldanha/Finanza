# Finanza

![Build](https://github.com/tiago-saldanha/Finanza/actions/workflows/ci.yml/badge.svg)
![Coverage](https://codecov.io/gh/tiago-saldanha/Finanza/branch/master/graph/badge.svg)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Finanza é uma aplicação de gestão financeira pessoal full-stack, construída com ASP.NET Core (.NET 8) e Angular 18, seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD).

---

## Funcionalidades

### Transações
- Cadastro de receitas, despesas e transferências entre contas
- Marcação de pagamento, reabertura e cancelamento
- Filtros por status, tipo e período
- Vínculo opcional com ativos, passivos, empréstimos, investimentos ou metas

### Contas Financeiras
- Múltiplas contas bancárias por usuário
- Conta de origem e destino em transferências

### Categorias
- Categorias de receita e despesa para classificação de transações

### Empréstimos Concedidos
- Controle de empréstimos feitos a terceiros (LoanReceivable)
- Parcelas com vencimento, marcação de recebimento e estorno
- KPIs: total emprestado, total recebido, saldo a receber, parcelas em atraso

### Empréstimos Obtidos
- Controle de dívidas com credores (LoanPayable)
- Parcelas com vencimento, marcação de pagamento e estorno
- KPIs: total obtido, total pago, saldo devedor, parcelas em atraso

### Ativos e Passivos
- Cadastro de ativos (imóveis, veículos, etc.) e passivos (dívidas, financiamentos)
- Histórico de valor dos ativos ao longo do tempo
- Passivos com parcelas e controle de pagamento

### Investimentos
- Cadastro de investimentos com valor aplicado e valor atual
- Cálculo de retorno e rentabilidade

### Metas Financeiras
- Definição de metas com valor alvo e prazo
- Acompanhamento de progresso e projeção mensal necessária
- Aporte manual e vínculo com transações
- KPIs: metas ativas, concluídas, total acumulado, próximo prazo

### Patrimônio
- Visão consolidada de ativos, passivos e patrimônio líquido
- Histórico de snapshots patrimoniais
- Gráficos de evolução

### Fluxo de Caixa
- Visão de entradas e saídas por período

### Planejamento
- Orçamento por categoria

### FIRE
- Calculadora de independência financeira (Financial Independence, Retire Early)

### Conta e Perfil
- Registro, login, recuperação de senha e troca de senha
- Exportação de dados e exclusão de conta
- Modo claro/escuro

---

## Relacionamentos entre Entidades

```
Transaction ──→ Account          (origem, FK nullable SetNull)
            ──→ Account          (destino em transferências, FK nullable SetNull)
            ──→ Category         (FK nullable Restrict)
            ──→ Asset            (FK nullable SetNull)
            ──→ Liability        (FK nullable SetNull)
            ──→ LoanReceivable   (FK nullable SetNull)
            ──→ LoanPayable      (FK nullable SetNull)
            ──→ Investment       (FK nullable SetNull)
            ──→ Goal             (FK nullable SetNull)

LoanReceivable ──→ LoanInstallment[]        (1:N Cascade)
LoanPayable    ──→ LoanPayableInstallment[] (1:N Cascade)
Liability      ──→ LiabilityInstallment[]   (1:N Cascade)
Asset          ──→ AssetValueHistory[]      (1:N Cascade)

Goal, Investment, PatrimonySnapshot — agregados independentes
```

---

## Arquitetura

### Backend

Estruturado em camadas seguindo Clean Architecture:

#### Domain
- Entidades, Value Objects (`Money`, `Description`, `TransactionDates`), Domain Events
- Regras de negócio sem dependências externas

#### Application
- Application Services e DTOs
- Interfaces de repositórios e Unit of Work
- Orquestração de casos de uso

#### Infrastructure
- Entity Framework Core com SQLite
- Implementações de repositórios e Unit of Work
- Identity (ASP.NET Core Identity + JWT)
- Serviço de e-mail via Resend API
- Multi-tenancy: um banco SQLite por usuário (`user_{guid}.db`)
- `TenantMigrationStartupService`: aplica migrações incrementais em bancos existentes via SQL idempotente

#### API
- Minimal APIs
- Autenticação JWT
- Swagger / OpenAPI
- Middleware de Problem Details

### Frontend (`Finanza.Client`)

Angular 18 SPA com standalone components, Angular Signals e Angular Material:

- Formulários reativos com integração a múltiplos domínios
- Modo claro/escuro com CSS custom properties
- Lazy loading por rota

---

## Tecnologias

### Backend
- .NET 8 / ASP.NET Core
- Entity Framework Core + SQLite
- ASP.NET Core Identity + JWT
- Resend (e-mail transacional)

### Frontend
- Angular 18 (standalone components, Signals)
- Angular Material
- Chart.js + ng2-charts
- Reactive Forms

### Testes
- xUnit + FluentAssertions
- Moq
- WebApplicationFactory (testes de integração)

---

## Padrões Arquiteturais

- Clean Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- Unit of Work
- Domain Events
- Multi-tenancy (banco SQLite por usuário)
- Dependency Injection

---

## Estratégia de Testes

**Domain:** testes unitários para entidades, value objects e regras de negócio.

**Application:** testes unitários para os application services com Moq.

**Infrastructure:** testes de integração para repositórios e serviços de autenticação contra SQLite real.

**API:** testes end-to-end com `WebApplicationFactory` e SQLite in-memory.

---

## Banco de Dados

Cada usuário possui um banco SQLite isolado (multi-tenant por usuário). Um banco compartilhado `app.db` gerencia o roteamento de tenants.

Em testes de integração, SQLite in-memory é utilizado com schema criado automaticamente no setup.

---

## Executando a Aplicação

### API

```bash
cd src/Finanza.API
dotnet run
```

### Frontend

```bash
cd src/Finanza.Client
npm install
npm start
```

O frontend roda em `http://localhost:4200` e a API em `https://localhost:7XXX` (ver `launchSettings.json`).

---

## Executando os Testes

```bash
dotnet test
```

---

## Autor

Tiago Ávila Saldanha — .NET Full Stack Developer

---

## Licença

Este projeto é para fins educacionais e de portfólio.
