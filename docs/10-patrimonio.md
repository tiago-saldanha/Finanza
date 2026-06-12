# 10 — Patrimônio

## Visão geral

O módulo de patrimônio registra todos os bens (ativos) e dívidas (passivos) para calcular o **patrimônio líquido** (net worth). Inclui histórico de snapshots para acompanhar a evolução ao longo do tempo.

---

## Ativos

Bens que você possui e que têm valor monetário.

### Tipos de ativo

| Tipo | Exemplos |
|---|---|
| **Conta Bancária** | Saldo em contas não controladas pelo Finanza |
| **Veículo** | Carro, moto, caminhão |
| **Imóvel** | Casa, apartamento, terreno |
| **Investimento** | Portfólio externo, previdência privada |
| **Outro** | Obras de arte, joias, outros bens |

### Campos de ativo

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | Sim | Identificação do bem |
| Tipo | Sim | Tipo do ativo |
| Valor | Sim | Valor atual estimado (R$) |

### Histórico de valor

Cada ativo mantém um histórico de atualizações de valor. Para registrar uma nova avaliação:
1. Clique em **Atualizar Valor** no card do ativo
2. Informe o novo valor e a data
3. O histórico fica disponível para consulta

---

## Passivos

Dívidas e obrigações financeiras.

### Tipos de passivo

| Tipo | Exemplos |
|---|---|
| **Financiamento** | Financiamento de imóvel, veículo |
| **Empréstimo** | Empréstimo bancário, pessoal |
| **Cartão de Crédito** | Fatura total do cartão |
| **Outro** | Outras dívidas |

### Campos de passivo

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | Sim | Identificação da dívida |
| Tipo | Sim | Tipo do passivo |
| Valor | Sim | Saldo devedor atual (R$) |

---

## Patrimônio Líquido

```
Patrimônio Líquido = Total de Ativos − Total de Passivos
```

O resultado é exibido na tela de Patrimônio e no **Dashboard**.

---

## Histórico de Patrimônio (Snapshots)

Acesse **Histórico Patrim.** no menu lateral para ver a evolução do patrimônio ao longo do tempo.

### Criar um snapshot

1. Acesse **Histórico Patrim.**
2. Clique em **Registrar Snapshot Atual**
3. O sistema salva a fotografia do patrimônio naquele momento (Total de Ativos, Total de Passivos e Patrimônio Líquido)

### Gráfico de evolução

O gráfico de linha exibe a variação do patrimônio líquido ao longo de todos os snapshots registrados.

> **Dica:** Registre um snapshot mensalmente para ter uma visão clara da evolução do seu patrimônio ao longo do ano.
