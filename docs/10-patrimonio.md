# 10 — Patrimônio

## Visão geral

O módulo de patrimônio consolida todos os bens (ativos), investimentos e dívidas (passivos) para calcular o **Patrimônio Líquido**. Inclui histórico de snapshots para acompanhar a evolução ao longo do tempo.

---

## Fórmula do Patrimônio Líquido

```
Patrimônio Líquido = Ativos + Investimentos − Passivos
```

Os investimentos cadastrados no módulo de Investimentos integram diretamente o cálculo — não é necessário duplicá-los como ativos.

Para passivos com parcelas, o valor considerado no cálculo é o **saldo devedor** (valor total − total já pago), não o valor original.

---

## Ativos

Bens com valor monetário.

**Tipos:**

| Valor | Tipo |
|---|---|
| 0 | Conta Bancária |
| 1 | Veículo |
| 2 | Imóvel |
| 3 | Investimento (uso alternativo, sem módulo próprio) |
| 4 | Outro |

**Campos:** Nome, Tipo, Valor (todos obrigatórios)

**Histórico de Valor:** Cada ativo mantém um histórico de atualizações. Clique em "Atualizar Valor" → informe o novo valor e a data.

---

## Passivos

Obrigações financeiras.

**Tipos:**

| Valor | Tipo |
|---|---|
| 0 | Financiamento |
| 1 | Empréstimo |
| 2 | Cartão de Crédito |
| 3 | Outro |

**Campos:**

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | ✓ | |
| Tipo | ✓ | |
| Valor | ✓ | Valor total da dívida |
| Data de Início | — | Início do contrato |
| Data de Vencimento | — | Vencimento final |
| Observações | — | Máximo 300 caracteres |
| Nº de Parcelas | — | Gera parcelas automaticamente |

### Parcelas de Passivos

Ao informar um número de parcelas, o sistema gera automaticamente parcelas com valores iguais (Valor ÷ Nº Parcelas) e datas de vencimento mensais a partir da data de início.

**Status de parcela:**
- ⬜ Pendente — não paga, dentro do prazo
- ✅ Pago — quitada
- 🔴 Vencida — não paga após o vencimento

**Saldo Devedor:** `Valor Total − Σ Parcelas Pagas`

O saldo devedor é o valor utilizado no cálculo do Patrimônio Líquido, refletindo o que ainda efetivamente se deve.

---

## Vínculo Transação → Passivo

Pagamentos de parcelas podem ser registrados como transações do tipo **Despesa** vinculadas ao passivo correspondente. O vínculo exclui a transação dos KPIs orçamentários, pois o pagamento de dívida não é uma despesa do orçamento corrente.

---

## Tela de Patrimônio

A tela exibe um **banner com 4 cards**:

| Card | Valor |
|---|---|
| Patrimônio Líquido | Ativos + Investimentos − Passivos |
| Ativos | Σ valor de todos os ativos |
| Investimentos | Σ valor atual de todos os investimentos |
| Passivos | Σ saldo devedor de todos os passivos |

Abaixo, layout em 3 colunas: **Ativos · Investimentos · Passivos**

---

## Histórico de Patrimônio (Snapshots)

Acesse **Histórico Patrimonial** na barra lateral para visualizar a evolução ao longo do tempo.

- Clique em **Registrar Snapshot Atual** para capturar o estado atual
- Gráfico de linha mostra a variação do patrimônio líquido entre os snapshots
- **Dica:** registre um snapshot mensal para ter uma visão clara da evolução anual
