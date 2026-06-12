# 14 — Referência da API REST

## Base URL

```
https://localhost:{porta}/api
```

Todas as rotas (exceto `/auth/register`, `/auth/login`, `/auth/forgot-password`, `/auth/reset-password`) exigem o header:

```
Authorization: Bearer <jwt_token>
```

---

## Autenticação — `/api/auth`

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/register` | Registrar novo usuário |
| `POST` | `/login` | Autenticar e obter token JWT |
| `PUT` | `/change-password` | Alterar senha (autenticado) |
| `POST` | `/forgot-password` | Solicitar e-mail de recuperação |
| `POST` | `/reset-password` | Redefinir senha com token do e-mail |

---

## Conta do usuário — `/api/account`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/export` | Exportar dados do usuário (JSON) |
| `DELETE` | `/` | Excluir conta e todos os dados |

---

## Transações — `/api/transactions`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/all` | Listar todas as transações |
| `GET` | `/{id}` | Buscar transação por ID |
| `GET` | `/search` | Buscar com filtros (search, status, type, startDate, endDate) |
| `GET` | `/status/{status}` | Filtrar por status |
| `GET` | `/type/{type}` | Filtrar por tipo |
| `POST` | `/` | Criar nova transação |
| `PUT` | `/{id}` | Editar transação |
| `PUT` | `/pay/{id}` | Marcar como paga (body: `{ paymentDate }`) |
| `PUT` | `/reopen/{id}` | Reabrir (volta para Pendente) |
| `PUT` | `/cancel/{id}` | Cancelar transação |
| `DELETE` | `/remove/{id}` | Excluir transação |

### Body — Criar/Editar transação
```json
{
  "description": "string",
  "amount": 0.00,
  "dueDate": "2026-01-15T00:00:00",
  "transactionType": "Revenue | Expense | Transfer",
  "categoryId": "guid | null",
  "accountId": "guid",
  "destinationAccountId": "guid | null"
}
```

---

## Categorias — `/api/categories`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/all` | Listar todas as categorias |
| `GET` | `/{id}` | Buscar por ID |
| `POST` | `/` | Criar categoria |
| `PUT` | `/{id}` | Editar categoria |

### Body
```json
{ "name": "string", "description": "string | null" }
```

---

## Contas Financeiras — `/api/financial-accounts`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/all` | Listar todas as contas |
| `GET` | `/{id}` | Buscar por ID |
| `POST` | `/` | Criar conta |
| `PUT` | `/{id}` | Editar conta |
| `DELETE` | `/{id}` | Excluir conta |

### Body
```json
{ "name": "string", "type": 0, "initialBalance": 0.00 }
```
> Tipos: `0` Corrente · `1` Poupança · `2` Carteira · `3` Cartão de Crédito · `4` Investimento

---

## Investimentos — `/api/investments`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/portfolio` | Portfólio completo com KPIs e alocações |
| `GET` | `/{id}` | Buscar investimento por ID |
| `POST` | `/` | Criar investimento |
| `PUT` | `/{id}` | Editar investimento |
| `DELETE` | `/{id}` | Excluir investimento |

### Body
```json
{ "name": "string", "type": 0, "investedAmount": 0.00, "currentValue": 0.00 }
```
> Tipos: `0` Renda Fixa · `1` Ações · `2` FII · `3` Cripto · `4` Outros

---

## Metas — `/api/goals`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/` | Listar todas as metas |
| `GET` | `/{id}` | Buscar por ID |
| `POST` | `/` | Criar meta |
| `PUT` | `/{id}` | Editar meta |
| `POST` | `/{id}/contribute` | Registrar aporte |
| `DELETE` | `/{id}` | Excluir meta |

### Body — Criar/Editar
```json
{ "name": "string", "targetAmount": 0.00, "currentAmount": 0.00, "targetDate": "2026-12-31" }
```

### Body — Aporte
```json
{ "amount": 0.00 }
```

---

## Empréstimos — `/api/loans`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/` | Listar todos os empréstimos |
| `GET` | `/summary` | Resumo consolidado (KPIs) |
| `GET` | `/{id}` | Buscar por ID |
| `POST` | `/` | Criar empréstimo |
| `PUT` | `/{id}` | Editar empréstimo |
| `DELETE` | `/{id}` | Excluir empréstimo |
| `POST` | `/installments/{installmentId}/pay` | Marcar parcela como recebida |
| `POST` | `/installments/{installmentId}/unpay` | Desfazer recebimento de parcela |

### Body — Criar/Editar
```json
{
  "borrowerName": "string",
  "totalAmount": 0.00,
  "startDate": "2026-01-01",
  "dueDate": "2026-12-31",
  "notes": "string | null",
  "installmentCount": 1
}
```

### Body — Pagar parcela
```json
{ "paidAt": "2026-06-11T00:00:00" }
```

---

## Patrimônio — Ativos (`/api/assets`) e Passivos (`/api/liabilities`)

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/all` | Listar todos |
| `GET` | `/{id}` | Buscar por ID |
| `POST` | `/` | Criar |
| `PUT` | `/{id}` | Editar |
| `DELETE` | `/{id}` | Excluir |

### Body — Ativo
```json
{ "name": "string", "type": 0, "value": 0.00 }
```
> Tipos ativo: `0` Conta Bancária · `1` Veículo · `2` Imóvel · `3` Investimento · `4` Outro

### Body — Passivo
```json
{ "name": "string", "type": 0, "value": 0.00 }
```
> Tipos passivo: `0` Financiamento · `1` Empréstimo · `2` Cartão de Crédito · `3` Outro

---

## Histórico de valor de ativo — `/api/assets`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/{id}/value-history` | Histórico de valores do ativo |
| `POST` | `/{id}/value` | Registrar novo valor |

### Body
```json
{ "value": 0.00, "date": "2026-06-11" }
```

---

## Patrimônio Líquido — `/api/net-worth`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/` | Total de ativos, passivos e patrimônio líquido |

---

## Snapshots de Patrimônio — `/api/patrimony-snapshots`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/` | Listar todos os snapshots |
| `POST` | `/` | Criar snapshot atual |

---

## Planejamento Financeiro — `/api/financial-planning`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/` | Retorna KPIs do mês/ano informados |

**Query params:** `?month=6&year=2026`

---

## FIRE — `/api/fire`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/` | Retorna todos os indicadores FIRE |

---

## Códigos de resposta

| Código | Significado |
|---|---|
| `200` | Sucesso |
| `201` | Criado com sucesso |
| `204` | Sem conteúdo (delete) |
| `400` | Dados inválidos |
| `401` | Não autenticado (token ausente ou expirado) |
| `404` | Recurso não encontrado |
| `500` | Erro interno do servidor |
