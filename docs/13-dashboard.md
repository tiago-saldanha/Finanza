# 13 — Dashboard

## Visão geral

O Dashboard é a tela inicial do Finanza após o login. Apresenta uma visão consolidada e em tempo real de todas as dimensões financeiras do usuário.

---

## Filtro de Período

Seletor no canto superior direito — afeta os KPIs e gráficos de transações:
- Este mês
- Mês passado
- Últimos 30 dias
- Este ano
- Tudo (sem filtro de data)

---

## Linha 1 — KPIs Principais

| KPI | Cálculo |
|---|---|
| **Saldo das Contas** | Σ saldo atual de todas as contas |
| **Receitas** | Σ receitas **pagas** no período (excluindo vinculadas ao patrimônio) |
| **Despesas** | Σ despesas **pagas** no período (excluindo vinculadas ao patrimônio) |
| **Economia** | Receitas − Despesas no período |
| **Patrimônio Líquido** | Ativos + Investimentos − Passivos |
| **Alerta** | Aparece apenas com transações pendentes ou itens vencidos |

> Transações vinculadas a ativos, passivos, empréstimos a receber ou investimentos são excluídas dos KPIs de receita/despesa. Apenas transações "puras" do orçamento são contabilizadas.

---

## Linha 2 — Cards de Módulos

**Card Investimentos:**
- Total investido, valor atual, retorno percentual
- Link rápido para `/investments`

**Card Empréstimos:**
- Total emprestado, total recebido, saldo a receber
- Alerta vermelho se há parcelas vencidas
- Link rápido para `/loans`

**Card Metas:**
- Top 3 metas ativas por progresso
- Barra de progresso colorida (Azul ≥ 60%, Laranja ≥ 30%, Vermelho < 30%, Verde = 100%)
- Contagem de metas concluídas
- Link rápido para `/goals`

---

## Linha 3 — Gráficos

**Despesas por Categoria:**
Gráfico de rosca com despesas **pagas** no período por categoria (somente transações puras de orçamento). Tooltip exibe valor e percentual.

**Saldo por Conta:**
Gráfico de barras horizontal com o saldo atual de cada conta (até 8 contas). Barras verdes = positivo; vermelhas = negativo.

---

## Linha 4 — Transações Recentes

6 transações mais recentes no período selecionado (ordenadas por data de vencimento decrescente).

Cada linha exibe:
- Ícone colorido por tipo (↑ receita / ↓ despesa / ⇄ transferência)
- Descrição e categoria
- Conta de origem
- Valor (negativo para despesas)
- Chip de status (Pendente / Pago / Cancelado)

Botão **Ver todas** redireciona para `/transactions`.
