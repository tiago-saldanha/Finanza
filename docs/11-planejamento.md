# 11 — Planejamento Financeiro

## Visão geral

A tela de planejamento analisa o orçamento do mês selecionado com base nas transações **pagas**, aplicando três métricas de saúde financeira: taxa de poupança, regra 50-30-20 e reserva de emergência.

---

## Filtros

| Filtro | Descrição |
|---|---|
| **Mês** | Mês de referência para o cálculo |
| **Ano** | Ano de referência |

---

## Banner de resumo

Exibe os totais de receitas e despesas pagas no período selecionado, e a economia do mês (receitas − despesas).

---

## Indicadores

### 1. Taxa de Poupança

```
Taxa de Poupança = (Receitas − Despesas) ÷ Receitas × 100
```

**Meta:** ≥ 20% da receita.

| Faixa | Avaliação |
|---|---|
| < 0% | Déficit — gastando mais do que ganha |
| 0% – 10% | Abaixo do recomendado |
| 10% – 20% | Razoável |
| ≥ 20% | ✅ Meta atingida |

---

### 2. Regra 50-30-20

Distribuição sugerida da receita bruta:

| Destinação | Meta | Descrição |
|---|---|---|
| **Necessidades** | ≤ 50% | Moradia, alimentação, saúde, transporte |
| **Desejos** | ≤ 30% | Lazer, roupas, restaurantes, viagens |
| **Poupança** | ≥ 20% | Reserva, investimentos, metas |

O sistema exibe o percentual **real** de cada fatia e indica se está dentro da meta. Quando **Poupança (real) < 20%**, o valor fica em vermelho.

> **Como o sistema calcula:** toda a despesa paga no período é considerada como "necessidades + desejos". A diferença entre receita e despesa é a poupança. A separação entre necessidades e desejos é uma responsabilidade do usuário via categorias.

---

### 3. Reserva de Emergência

**Meta:** 6 meses de gastos médios.

```
Gastos médios (3 meses) = Média de despesas pagas nos últimos 3 meses
Meta da reserva = Gastos médios × 6
Meses cobertos = Saldo total nas contas ÷ Gastos médios
```

O card exibe:
- Saldo atual nas contas
- Gastos médios dos últimos 3 meses
- Quantidade de meses cobertos pela reserva atual
- Barra de progresso em relação à meta de 6 meses

---

## Dicas de uso

- Use o planejamento **no final de cada mês** para revisar como você gastou
- Compare diferentes meses para identificar tendências
- Se a taxa de poupança estiver consistentemente abaixo de 20%, revise as categorias de despesas no módulo de Categorias
- Use em conjunto com as **Metas** para dar destino à poupança acumulada
