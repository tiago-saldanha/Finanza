# 04 — Transações

## Visão geral

As transações são o núcleo financeiro do Finanza. Registram todo movimento de dinheiro: entradas (receitas), saídas (despesas) e transferências entre contas.

---

## Tipos de transação

| Tipo | Descrição | Ícone |
|---|---|---|
| **Receita** | Entrada de dinheiro (salário, freelance, venda) | ↑ verde |
| **Despesa** | Saída de dinheiro (conta, compra, serviço) | ↓ vermelho |
| **Transferência** | Movimento entre duas contas próprias | ⇄ azul |

---

## Status

| Status | Descrição |
|---|---|
| **Pendente** | Lançado, mas ainda não efetivado |
| **Pago** | Confirmado com data de pagamento |
| **Cancelado** | Anulado (não afeta saldos) |

---

## Campos

| Campo | Obrigatório | Descrição |
|---|---|---|
| Descrição | Sim | Texto livre identificando a transação |
| Valor | Sim | Valor em R$ |
| Tipo | Sim | Receita, Despesa ou Transferência |
| Conta | Sim | Conta financeira de origem |
| Categoria | Não (para transferências) | Categoria para classificação |
| Data de Vencimento | Sim | Data de vencimento/competência |
| Conta Destino | Só em Transferência | Conta que receberá o valor |
| Data de Pagamento | Ao pagar | Preenchida ao marcar como pago |

---

## Como usar

### Criar uma transação

1. Acesse **Transações** no menu lateral
2. Clique em **+ Nova Transação**
3. Preencha os campos no modal
4. Para **transferências**: selecione a conta de origem e a conta de destino (campo "Conta Destino" aparece automaticamente)
5. Clique em **Salvar**

### Marcar como pago

1. Na listagem, clique no menu ⋮ da transação pendente
2. Selecione **Marcar como Pago**
3. Confirme a data de pagamento (padrão: hoje)

### Editar uma transação

Disponível apenas para transações com status **Pendente**. Menu ⋮ → **Editar**.

### Reabrir uma transação

Transações pagas ou canceladas podem ser reabertas: Menu ⋮ → **Reabrir (Pendente)**.

### Excluir uma transação

Menu ⋮ → **Excluir** → confirmação obrigatória.

---

## Filtros e busca

A tela de listagem oferece:

| Filtro | Opções |
|---|---|
| **Busca** | Texto livre na descrição |
| **Status** | Todos / Pendente / Pago / Cancelado |
| **Tipo** | Todos / Receita / Despesa / Transferência |
| **Data Inicial** | Seletor de data |
| **Data Final** | Seletor de data |

> Os filtros são **persistidos automaticamente** no navegador (localStorage). Ao retornar à tela, os filtros anteriores são restaurados. Use o botão **Limpar filtros** para redefinir.

---

## Tabela de listagem

Colunas exibidas:

| Coluna | Descrição |
|---|---|
| Tipo | Ícone colorido (↑ receita / ↓ despesa / ⇄ transferência) |
| Descrição | Nome e categoria |
| Valor | Valor formatado em R$; para transferências mostra conta destino |
| Vencimento | Data de vencimento; ícone ⚠ se em atraso |
| Pagamento | Data em que foi pago (verde); `—` se ainda não pago |
| Status | Chip colorido (Pendente / Pago / Cancelado) |

---

## Impacto no saldo das contas

| Tipo | Conta Origem | Conta Destino |
|---|---|---|
| Receita paga | + valor | — |
| Despesa paga | − valor | — |
| Transferência paga | − valor | + valor |

Transações **Pendentes** e **Canceladas** não alteram o saldo calculado.
