# Finanza — Roadmap

> **Princípio fundamental:**
> ```
> Fluxo de Caixa ≠ Patrimônio
> ```
> Fluxo de Caixa responde: _Entrou dinheiro? Saiu dinheiro?_
> Patrimônio responde: _O que eu possuo? O que eu devo? Qual meu patrimônio líquido?_

---

## Fase 1 — Controle Financeiro ✅

### Entidades
- `Account`
- `Transaction`
- `Category`
- `User`

### Funcionalidades
- [x] Cadastro de contas
- [x] Receitas
- [x] Despesas
- [x] Categorias
- [x] Dashboard financeiro
- [x] Saldo por conta
- [x] Fluxo de caixa

### KPIs
- [x] Receita Total
- [x] Despesa Total
- [x] Saldo Mensal
- [x] Receita x Despesa

---

## Fase 2 — Evolução Patrimonial ✅

### Novas Entidades
- `Asset`
- `AssetType`
- `Liability`
- `LiabilityType`
- `PatrimonySnapshot`

### Ativos
- Conta Corrente
- Poupança
- Tesouro Selic
- ETF
- Ações
- Criptomoedas
- Veículos
- Imóveis
- Empréstimos a Receber

### Passivos
- Cartão de Crédito
- Financiamentos
- Empréstimos

### KPI
```
Patrimônio Líquido = Ativos - Passivos
```

---

## Fase 3 — Novos Tipos de Transação ✅

### Tipos de Transação
| Tipo | Exemplos |
|------|----------|
| `Income` | Salário, Bônus, Freelance |
| `Expense` | Mercado, Aluguel, Restaurante |
| `Transfer` | Sicredi → Nubank, Carteira → Banco |
| `Investment` | Tesouro Selic, ETF, Ações, Bitcoin, Terreno |
| `Loan` | Amigo, Familiar |

---

## Fase 4 — Gestão de Investimentos ✅

### Entidades
- `Investment`
- `AssetTransaction`

### Operações
- Compra
- Venda
- Transferência
- Aporte
- Resgate

### KPIs
- Rentabilidade
- Rentabilidade Acumulada
- Benchmark Selic
- Distribuição da Carteira
- Alocação por Classe

---

## Fase 5 — Gestão de Créditos ✅

### Entidades
- `LoanReceivable`
- `LoanInstallment`

### Funcionalidades
- [x] Empréstimos para terceiros
- [x] Parcelas
- [x] Histórico de pagamento
- [x] Controle de inadimplência

### KPIs
- Total Emprestado
- Total Recebido
- Saldo a Receber
- Inadimplência

---

## Fase 6 — Gestão Patrimonial Completa ✅

### Casos Reais
- [x] Terreno em Vale Real
- [x] Veículos
- [x] Imóveis
- [x] Criptomoedas
- [x] Tesouro Selic
- [x] Empréstimos concedidos

### Funcionalidades — Snapshots Mensais
```
01/01/2026  →  Patrimônio = R$ 50.000
01/02/2026  →  Patrimônio = R$ 52.000
01/03/2026  →  Patrimônio = R$ 55.000
```

---

## Fase 7 — BI Financeiro ✅

### Data Warehouse

**Dimensões**
- `DimDate`
- `DimCategory`
- `DimAccount`
- `DimAsset`
- `DimLiability`
- `DimUser`

**Fatos**
- `FactTransaction`
- `FactPatrimonySnapshot`
- `FactInvestmentPosition`

### KPIs
- Taxa de Poupança
- Taxa de Conversão Patrimonial
- Patrimônio Líquido
- Reserva de Emergência
- FIRE Number
- Evolução Patrimonial
- Receita x Despesa
- Rentabilidade

---

## Fase 8 — Motor de Insights ✅

> Sem IA inicialmente. Motor baseado em regras.

```
Se gastos > receita         → alerta
Se reserva < 6 meses        → alerta
Se patrimônio caiu          → alerta
Se taxa de poupança aumentou → destaque positivo
```

---

## Fase 9 — IA ✅

> A IA não fará cálculos. Ela explicará os dados.

**Exemplos de saída:**
```
"Seu patrimônio cresceu 8% este mês."
"Seu gasto com alimentação aumentou 15%."
"Você possui R$ 10.000 emprestados para terceiros."
```
