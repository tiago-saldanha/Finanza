# 07 — Investimentos

## Visão geral

O módulo de investimentos permite registrar a carteira de investimentos, acompanhar o valor atual e calcular o retorno sobre o capital investido.

---

## Tipos de investimento

| Tipo | Descrição |
|---|---|
| **Renda Fixa** | CDB, LCI, LCA, Tesouro Direto, poupança |
| **Ações** | Ações de empresas na bolsa de valores |
| **FII** | Fundos de Investimento Imobiliário |
| **Cripto** | Criptomoedas (Bitcoin, Ethereum, etc.) |
| **Outros** | Qualquer outro tipo de investimento |

---

## Campos

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | Sim | Identificação do investimento (ex: "PETR4", "Bitcoin", "CDB Nubank") |
| Tipo | Sim | Tipo de ativo (tabela acima) |
| Valor Investido | Sim | Quanto foi aportado no total (R$) |
| Valor Atual | Sim | Valor de mercado atual (R$) |

> O **Valor Atual** deve ser atualizado manualmente sempre que desejar refletir a cotação atual do ativo.

---

## Como usar

### Cadastrar um investimento

1. Acesse **Investimentos** no menu lateral
2. Clique em **Novo Investimento**
3. Preencha os campos
4. Clique em **Salvar**

### Editar / atualizar valor

Clique no ícone ✏️ no item da lista → atualize o **Valor Atual** → **Salvar**.

### Excluir um investimento

Clique no ícone 🗑️ → confirmação obrigatória.

---

## KPIs do portfólio

| Indicador | Cálculo |
|---|---|
| **Total Investido** | Soma de todos os valores investidos |
| **Valor Atual** | Soma de todos os valores atuais |
| **Retorno Total** | Valor Atual − Total Investido |
| **Retorno (%)** | (Retorno Total / Total Investido) × 100 |

---

## Gráfico de alocação

O gráfico de rosca (doughnut) exibe a distribuição percentual do portfólio por tipo de ativo (Renda Fixa, Ações, FII, Cripto, Outros).

---

## Integração com outros módulos

- Os valores do portfólio são exibidos no **Dashboard** no card de Investimentos
- Para registrar um investimento como **ativo patrimonial** (ex: imóvel, veículo), use o módulo **Patrimônio → Ativos** com o tipo `Investment`
