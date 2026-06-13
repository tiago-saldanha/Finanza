# 04 — Transações

## Visão geral

As transações são o núcleo financeiro do Finanza. Registram todo movimento de dinheiro: entradas (receitas), saídas (despesas) e transferências entre contas. Opcionalmente, cada transação pode ser vinculada a um item patrimonial (ativo, passivo, empréstimo a receber ou investimento).

---

## Tipos de Transação

| Tipo | Descrição | Indicador |
|---|---|---|
| **Receita** | Entrada de dinheiro | Verde ↑ |
| **Despesa** | Saída de dinheiro | Vermelho ↓ |
| **Transferência** | Movimentação entre contas próprias | Azul ⇄ |

> Os tipos **Investimento** e **Empréstimo** foram removidos. Movimentações relacionadas ao patrimônio são registradas como **Despesa** ou **Receita** e vinculadas ao item patrimonial correspondente via campo de vínculo patrimonial.

---

## Status

| Status | Descrição |
|---|---|
| **Pendente** | Lançada mas ainda não executada |
| **Pago** | Confirmada com data de pagamento registrada |
| **Cancelada** | Anulada — não afeta saldos nem KPIs |

---

## Campos

| Campo | Obrigatório | Observação |
|---|---|---|
| Descrição | ✓ | |
| Valor | ✓ | |
| Tipo | ✓ | Receita, Despesa ou Transferência |
| Conta | ✓ | |
| Categoria | — | Não disponível em transferências |
| Data de Vencimento | ✓ | |
| Conta Destino | — | Apenas em transferências |
| Data de Pagamento | — | Definida ao marcar como pago |
| Vínculo Patrimonial | — | Ativo, Passivo, Empréstimo ou Investimento |

---

## Vínculo Patrimonial

Uma transação pode ser vinculada a exatamente um item patrimonial. O vínculo indica que a movimentação é de natureza patrimonial, não orçamentária.

| Vínculo | Quando usar | Exemplo |
|---|---|---|
| **Ativo** | Compra ou venda de um bem | Entrada de veículo, imóvel |
| **Passivo** | Pagamento de parcela de dívida | Parcela de financiamento |
| **Empréstimo a Receber** | Recebimento de parcela de empréstimo concedido | Recebimento de amigo |
| **Investimento** | Aporte ou resgate de investimento | Compra de CDB, ações |

> **Transações vinculadas são excluídas dos KPIs de orçamento** (receitas/despesas do Dashboard e Planejamento). Um aporte em investimento não deve inflar as despesas do mês — apenas o saldo da conta é afetado.

---

## Operações

| Operação | Disponível para |
|---|---|
| Criar | — |
| Editar | Pendente e **Pago** |
| Marcar como Pago | Pendente |
| Reabrir | Pago e Cancelado |
| Cancelar | Pendente e **Pago** |
| Excluir | Qualquer status |

> **Edição de transações pagas:** ao alterar a data de pagamento de uma transação já paga, o sistema reabre automaticamente e registra o novo pagamento com a data informada, mantendo o histórico consistente.

---

## Impacto no Saldo

| Situação | Efeito |
|---|---|
| Receita paga | +valor na conta de origem |
| Despesa paga | −valor na conta de origem |
| Transferência paga | −valor na origem, +valor no destino |
| Pendente | Sem efeito no saldo calculado |
| Cancelada | Sem efeito no saldo calculado |

---

## Filtros

- Busca por descrição
- Status (Pendente / Pago / Cancelado)
- Tipo (Receita / Despesa / Transferência)
- Intervalo de datas (vencimento)

Os filtros são persistidos no `localStorage` do navegador entre sessões.
