# 12 — FIRE (Independência Financeira)

## Visão geral

O módulo FIRE (_Financial Independence, Retire Early_) calcula o progresso em direção à independência financeira com base nos seus dados financeiros atuais.

---

## O conceito FIRE

A independência financeira é atingida quando o patrimônio investido gera rendimentos suficientes para cobrir todos os gastos sem necessidade de trabalhar. A referência clássica é a **Regra dos 4%**: com um patrimônio de 25× os gastos anuais, é possível sacar 4% ao ano indefinidamente.

---

## Cálculos realizados

### Número FIRE

```
Número FIRE = Gastos anuais × 25
```

Onde:
- **Gastos anuais** = média dos últimos 3 meses de despesas pagas × 12

### Progresso FIRE

```
Progresso (%) = (Saldo nas contas ÷ Número FIRE) × 100
```

### Anos para a independência

Estimativa de quantos anos faltam para atingir o Número FIRE, considerando:
- Patrimônio atual (saldo nas contas)
- Poupança mensal estimada (receitas − despesas dos últimos 3 meses)
- Taxa de retorno configurada (padrão: 0,5% a.m.)

---

## KPIs exibidos

| Indicador | Descrição |
|---|---|
| **Saldo Atual** | Soma dos saldos de todas as contas |
| **Número FIRE** | Meta de patrimônio para independência |
| **Progresso** | Percentual atingido em relação ao Número FIRE |
| **Anos estimados** | Projeção até atingir o Número FIRE |
| **Gastos mensais** | Média dos últimos 3 meses |
| **Poupança mensal** | Estimativa de aporte mensal |
| **Taxa de retorno** | Rendimento estimado sobre o patrimônio |

### Banner principal

Exibe o progresso com barra visual. Quando o progresso atinge 100%, o banner é marcado como **✅ Independência atingida!**

---

## Cards de apoio

- **Gastos mensais estimados**: baseado na média de despesas pagas nos últimos 3 meses
- **Patrimônio acumulado**: saldo atual nas contas vs. meta FIRE
- **Projeção de anos**: simulação com e sem rendimento sobre o patrimônio
- **Informações sobre a regra dos 4%**: explicação conceitual do método

---

## Dicas

- Reduza os gastos mensais para diminuir o Número FIRE necessário
- Aumente a poupança mensal para chegar mais rápido
- Registre todos os investimentos no módulo de **Investimentos** para ter uma visão mais precisa do patrimônio
- Use em conjunto com o módulo de **Planejamento** para otimizar a taxa de poupança
