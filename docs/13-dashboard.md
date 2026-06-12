# 13 — Dashboard

## Visão geral

O Dashboard é a tela inicial do Finanza após o login. Apresenta uma visão consolidada e em tempo real de todas as dimensões financeiras do usuário.

---

## Filtro de período

O seletor de **Período** no canto superior direito afeta os KPIs e gráficos de transações:

| Opção | Intervalo |
|---|---|
| Este mês | Do dia 1 até o último dia do mês atual |
| Mês passado | Mês anterior completo |
| Últimos 30 dias | Últimos 30 dias corridos |
| Este ano | Do dia 1/1 até 31/12 do ano atual |
| Tudo | Sem filtro de data |

---

## Linha 1 — KPIs principais

| Card | Fonte dos dados |
|---|---|
| **Saldo nas Contas** | Soma de `currentBalance` de todas as contas |
| **Receitas** | Soma das receitas **pagas** no período |
| **Despesas** | Soma das despesas **pagas** no período |
| **Economia** | Receitas − Despesas no período |
| **Patrimônio Líquido** | Total de Ativos − Total de Passivos (módulo Patrimônio) |
| **Atenção** | Aparece somente quando há pendentes ou transações em atraso |

---

## Linha 2 — Módulos

### Investimentos
- Total investido, valor atual e retorno percentual
- Link rápido para `/investments`

### Empréstimos
- Total emprestado, total recebido e saldo a receber
- Alerta vermelho se houver parcelas em atraso
- Link rápido para `/loans`

### Metas
- Top 3 metas ativas com maior progresso
- Barra de progresso colorida por faixa
- Contagem de metas concluídas
- Link rápido para `/goals`

---

## Linha 3 — Gráficos

### Despesas por Categoria
Gráfico de rosca (doughnut) com a distribuição das despesas **pagas** no período por categoria. Tooltip exibe valor e percentual.

### Saldo por Conta
Gráfico de barras horizontais com o saldo atual de cada conta cadastrada (até 8 contas). Barras verdes = saldo positivo; vermelhas = saldo negativo.

---

## Linha 4 — Transações Recentes

Lista das 6 transações mais recentes no período selecionado (ordenadas por data de vencimento decrescente).

Cada linha exibe:
- Ícone colorido por tipo (↑ receita / ↓ despesa / ⇄ transferência)
- Descrição e categoria
- Conta de origem
- Valor (negativo para despesas)
- Chip de status (Pendente / Pago / Cancelado)

Botão **Ver todas** redireciona para `/transactions`.
