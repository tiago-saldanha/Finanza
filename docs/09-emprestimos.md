# 09 — Empréstimos a Terceiros

## Visão geral

O módulo de empréstimos registra dinheiro emprestado a outras pessoas, com controle de parcelas, datas de vencimento e pagamentos recebidos.

---

## Campos do empréstimo

| Campo | Obrigatório | Descrição |
|---|---|---|
| Nome do Devedor | Sim | Nome de quem recebeu o dinheiro |
| Valor Total | Sim | Valor total emprestado (R$) |
| Data de Início | Sim | Quando o empréstimo foi feito |
| Data de Vencimento | Sim | Prazo final para quitação |
| Nº de Parcelas | Sim | Quantidade de parcelas (mínimo 1) |
| Observações | Não | Notas livres sobre o empréstimo |

> As parcelas são geradas **automaticamente** com valor igual (Valor Total ÷ Nº de Parcelas) e vencimentos mensais a partir da data de início.

---

## Como usar

### Registrar um empréstimo

1. Acesse **Empréstimos** no menu lateral
2. Clique em **+ Novo Empréstimo**
3. Preencha os campos
4. Clique em **Salvar**
   - As parcelas são criadas automaticamente

### Registrar recebimento de parcela

1. Expanda o card do devedor
2. Localize a parcela na lista
3. Clique em **Receber** na parcela desejada
4. Confirme a data de recebimento

### Desfazer recebimento

Clique em **Desfazer** na parcela já recebida.

### Editar um empréstimo

Clique no botão **Editar** no rodapé do card expandido.

### Excluir um empréstimo

Clique no botão **Excluir** → confirmação obrigatória. Remove o empréstimo e todas as suas parcelas.

---

## Status das parcelas

| Status | Descrição |
|---|---|
| ⬜ Pendente | Parcela não recebida, dentro do prazo |
| ✅ Recebida | Parcela paga pelo devedor |
| 🔴 Em atraso | Não recebida após a data de vencimento |

---

## KPIs

| Indicador | Cálculo |
|---|---|
| **Total Emprestado** | Soma de todos os valores emprestados |
| **Total Recebido** | Soma das parcelas recebidas |
| **Saldo a Receber** | Total Emprestado − Total Recebido |
| **Empréstimos em Atraso** | Quantidade com pelo menos uma parcela vencida |

---

## Integração com o Dashboard

O card de **Empréstimos** no Dashboard exibe Total Emprestado, Total Recebido e Saldo a Receber. Se houver parcelas em atraso, um alerta vermelho é exibido.
