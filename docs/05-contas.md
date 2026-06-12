# 05 — Contas Financeiras

## Visão geral

As contas representam onde o dinheiro está armazenado. O saldo de cada conta é calculado dinamicamente a partir do saldo inicial mais todas as transações **pagas**.

---

## Tipos de conta

| Tipo | Descrição |
|---|---|
| **Conta Corrente** | Conta bancária de uso geral |
| **Poupança** | Conta poupança |
| **Carteira** | Dinheiro físico em espécie |
| **Cartão de Crédito** | Cartão de crédito (fatura) |
| **Investimento** | Conta de custódia de investimentos |

---

## Campos

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | Sim | Identificação da conta (ex: "Nubank", "Carteira") |
| Tipo | Sim | Tipo da conta (ver tabela acima) |
| Saldo Inicial | Sim | Saldo no momento do cadastro (pode ser negativo) |

---

## Como usar

### Criar uma conta

1. Acesse **Contas** no menu lateral
2. Clique em **+ Nova Conta**
3. Preencha nome, tipo e saldo inicial
4. Clique em **Salvar**

### Editar uma conta

Clique no ícone ✏️ no card da conta.

### Excluir uma conta

Clique no ícone 🗑️ no card da conta → confirmação obrigatória.

> ⚠️ A exclusão de uma conta não exclui as transações associadas a ela.

---

## Saldo calculado

```
Saldo Atual = Saldo Inicial
            + Σ Receitas pagas
            − Σ Despesas pagas
            − Σ Transferências pagas (saída)
            + Σ Transferências recebidas pagas (entrada)
```

Transferências entre contas movimentam o saldo de ambas automaticamente.

---

## Banner de resumo

O topo da tela **Contas** exibe:

- **Total de Ativos**: soma dos saldos positivos de todas as contas
- **Total de Passivos**: soma dos saldos negativos (cartões de crédito, contas no negativo)
- **Saldo Líquido**: total de ativos − total de passivos
