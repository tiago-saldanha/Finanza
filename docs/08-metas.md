# 08 — Metas Financeiras

## Visão geral

O módulo de metas permite definir objetivos financeiros com valor-alvo e data limite, acompanhando o progresso por meio de aportes manuais.

---

## Campos

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome | Sim | Descrição do objetivo (ex: "Reserva de emergência", "Viagem ao Japão") |
| Valor Alvo | Sim | Quanto precisa ser acumulado (R$) |
| Valor Atual | Sim | Quanto já foi acumulado (pode ser 0) |
| Data Limite | Sim | Prazo para atingir a meta |

---

## Como usar

### Criar uma meta

1. Acesse **Metas** no menu lateral
2. Clique em **+ Nova Meta**
3. Preencha os campos
4. Clique em **Salvar**

### Registrar um aporte

1. Clique no botão **Aportar** no card da meta
2. Informe o valor a adicionar
3. Clique em **Confirmar**

> O aporte **acumula** sobre o valor atual — não substitui.

### Editar uma meta

Clique no ícone ✏️ → edite qualquer campo → **Salvar**.

### Excluir uma meta

Clique no ícone 🗑️ → confirmação obrigatória.

---

## KPIs exibidos

| Indicador | Descrição |
|---|---|
| **Total de Metas** | Quantidade total de metas cadastradas |
| **Valor Total Alvo** | Soma de todos os valores-alvo |
| **Valor Total Acumulado** | Soma de todos os valores atuais |
| **Metas Concluídas** | Quantidade de metas com progresso ≥ 100% |

---

## Cards de metas

Cada card exibe:
- Nome e data limite
- Progresso em percentual e barra visual
- Valor atual vs. valor alvo
- Valor restante
- Indicação **✓ Concluída** quando atingida

---

## Integração com o Dashboard

As **top 3 metas ativas** com maior progresso são exibidas no Dashboard com barra de progresso colorida:
- 🟢 ≥ 60% — azul
- 🟡 ≥ 30% — laranja
- 🔴 < 30% — vermelho
- ✅ 100% — verde
