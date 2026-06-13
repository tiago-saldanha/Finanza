# 07 — Investimentos

## Visão geral

O módulo de investimentos permite registrar a carteira de investimentos, acompanhar o valor atual e calcular o retorno sobre o capital investido. Os investimentos também integram o cálculo do **Patrimônio Líquido** — são somados aos ativos e deduzidos dos passivos.

---

## Tipos de Investimento

| Valor | Tipo |
|---|---|
| 0 | Renda Fixa (CDB, LCI, LCA, Tesouro Direto, poupança) |
| 1 | Ações |
| 2 | Fundos Imobiliários (FII) |
| 3 | Criptomoedas |
| 4 | Outros |

---

## Campos

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | ✓ | Identificação do investimento |
| Tipo | ✓ | Categoria do ativo |
| Valor Investido | ✓ | Total aportado em R$ |
| Valor Atual | ✓ | Valor de mercado atual — atualizado manualmente |

---

## Operações

- Criar, editar (inclusive atualizar valor atual), excluir

---

## KPIs da Carteira

| KPI | Cálculo |
|---|---|
| Total Investido | Σ de todos os valores investidos |
| Valor Atual | Σ de todos os valores atuais |
| Retorno Total | Valor Atual − Total Investido |
| Retorno (%) | (Retorno Total ÷ Total Investido) × 100 |

---

## Gráfico de Alocação

Gráfico de rosca mostrando a distribuição da carteira por tipo de ativo (Renda Fixa, Ações, FII, Cripto, Outros).

---

## Vínculo com Transações

Aportes e resgates de investimentos podem ser registrados como transações do tipo **Despesa** ou **Receita** com vínculo ao investimento correspondente.

**Por que usar o vínculo:**
- O aporte sai da conta bancária (despesa) mas não é uma despesa do orçamento mensal
- O vínculo exclui essa transação dos KPIs orçamentários (receita/despesa do Dashboard e Planejamento)
- O histórico financeiro fica completo e rastreável

**Como vincular:**
1. Ao criar ou editar uma transação, selecione o chip **Investimento** no campo de vínculo patrimonial
2. Escolha o investimento na lista
3. Salve — a transação afeta o saldo da conta mas não os indicadores de orçamento

---

## Integração com Patrimônio Líquido

```
Patrimônio Líquido = Ativos + Investimentos − Passivos
```

O valor atual de cada investimento é somado diretamente ao patrimônio líquido exibido na tela de Patrimônio e no Dashboard.

---

## Integração com Dashboard

O card de Investimentos no Dashboard exibe:
- Total investido
- Valor atual
- Retorno percentual
- Link rápido para `/investments`
