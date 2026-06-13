# 14 — Referência da API REST

## Base URL

```
https://localhost:{port}/api
```

Todas as rotas exceto auth (register, login, forgot-password, reset-password) requerem:
```
Authorization: Bearer <jwt_token>
```

---

## Autenticação (`/api/auth`)

| Método | Rota | Descrição |
|---|---|---|
| POST | `/register` | Cadastrar novo usuário |
| POST | `/login` | Autenticar e obter JWT |
| PUT | `/change-password` | Alterar senha (autenticado) |
| POST | `/forgot-password` | Solicitar e-mail de recuperação |
| POST | `/reset-password` | Redefinir senha com token do e-mail |

---

## Conta do Usuário (`/api/account`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/export` | Exportar dados do usuário (JSON) |
| DELETE | `/` | Excluir conta e todos os dados |

---

## Transações (`/api/transactions`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/all` | Listar todas |
| GET | `/{id}` | Buscar por ID |
| GET | `/search` | Buscar com filtros (search, status, type, startDate, endDate) |
| GET | `/status/{status}` | Filtrar por status |
| GET | `/type/{type}` | Filtrar por tipo |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar (Pendente ou Pago) |
| PUT | `/pay/{id}` | Marcar como pago |
| PUT | `/reopen/{id}` | Reabrir para Pendente |
| PUT | `/cancel/{id}` | Cancelar (Pendente ou Pago) |
| DELETE | `/remove/{id}` | Excluir |

**Body (Criar/Editar):**
```json
{
  "description": "string",
  "amount": 0.00,
  "dueDate": "2026-01-15T00:00:00",
  "transactionType": "Revenue | Expense | Transfer",
  "categoryId": "guid | null",
  "accountId": "guid",
  "destinationAccountId": "guid | null",
  "assetId": "guid | null",
  "liabilityId": "guid | null",
  "loanReceivableId": "guid | null",
  "investmentId": "guid | null"
}
```

> Apenas um dos campos de vínculo patrimonial (`assetId`, `liabilityId`, `loanReceivableId`, `investmentId`) deve ser preenchido por vez.

**Body (Marcar como Pago):** `{ "paymentDate": "2026-06-15T00:00:00" }`

---

## Categorias (`/api/categories`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/all` | Listar todas |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |

**Body:** `{ "name": "string", "description": "string | null" }`

---

## Contas Financeiras (`/api/financial-accounts`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/all` | Listar todas |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |
| DELETE | `/{id}` | Excluir |

**Body:** `{ "name": "string", "type": 0, "initialBalance": 0.00 }`

**Tipos:** `0` Conta Corrente · `1` Poupança · `2` Carteira · `3` Cartão de Crédito · `4` Conta Investimento

---

## Investimentos (`/api/investments`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/portfolio` | Carteira completa com KPIs e alocações |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |
| DELETE | `/{id}` | Excluir |

**Body:** `{ "name": "string", "type": 0, "investedAmount": 0.00, "currentValue": 0.00 }`

**Tipos:** `0` Renda Fixa · `1` Ações · `2` FII · `3` Cripto · `4` Outros

---

## Metas (`/api/goals`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Listar todas |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |
| POST | `/{id}/contribute` | Registrar contribuição |
| DELETE | `/{id}` | Excluir |

**Body (Criar/Editar):** `{ "name": "string", "targetAmount": 0.00, "currentAmount": 0.00, "targetDate": "2026-12-31" }`

**Body (Contribuição):** `{ "amount": 0.00 }`

---

## Empréstimos a Receber (`/api/loans`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Listar todos |
| GET | `/summary` | Resumo consolidado (KPIs) |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |
| DELETE | `/{id}` | Excluir |
| POST | `/installments/{installmentId}/pay` | Marcar parcela como recebida |
| POST | `/installments/{installmentId}/unpay` | Desfazer recebimento |

**Body (Criar/Editar):**
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

**Body (Pagar parcela):** `{ "paidAt": "2026-06-11T00:00:00" }`

---

## Ativos (`/api/assets`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/all` | Listar todos |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |
| DELETE | `/{id}` | Excluir |
| GET | `/{id}/value-history` | Histórico de valores |
| POST | `/{id}/value` | Registrar novo valor |

**Body (Criar/Editar):** `{ "name": "string", "type": 0, "value": 0.00 }`

**Tipos:** `0` Conta Bancária · `1` Veículo · `2` Imóvel · `3` Investimento · `4` Outro

**Body (Novo valor):** `{ "value": 0.00, "date": "2026-06-11" }`

---

## Passivos (`/api/liabilities`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/all` | Listar todos |
| GET | `/{id}` | Buscar por ID |
| POST | `/` | Criar |
| PUT | `/{id}` | Editar |
| DELETE | `/{id}` | Excluir |
| POST | `/installments/{installmentId}/pay` | Marcar parcela como paga |
| POST | `/installments/{installmentId}/unpay` | Desfazer pagamento |

**Body (Criar/Editar):**
```json
{
  "name": "string",
  "type": 0,
  "value": 0.00,
  "startDate": "2026-01-01 | null",
  "dueDate": "2026-12-31 | null",
  "notes": "string | null",
  "installmentCount": 0
}
```

**Tipos:** `0` Financiamento · `1` Empréstimo · `2` Cartão de Crédito · `3` Outro

**Body (Pagar parcela):** `{ "paidAt": "2026-06-11T00:00:00" }`

---

## Patrimônio Líquido (`/api/net-worth`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Total de ativos, investimentos, passivos e patrimônio líquido |

**Response:**
```json
{
  "totalAssets": 0.00,
  "totalInvestments": 0.00,
  "totalLiabilities": 0.00,
  "netWorth": 0.00,
  "assets": [],
  "investments": [],
  "liabilities": []
}
```

> `netWorth = totalAssets + totalInvestments − totalLiabilities`

---

## Snapshots de Patrimônio (`/api/patrimony-snapshots`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Listar todos os snapshots |
| POST | `/` | Registrar snapshot atual |

---

## Planejamento Financeiro (`/api/financial-planning`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | KPIs do mês/ano informado |

**Query params:** `?month=6&year=2026`

---

## FIRE (`/api/fire`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Todos os indicadores de independência financeira |

---

## Códigos de Resposta

| Código | Significado |
|---|---|
| 200 | Sucesso |
| 201 | Criado com sucesso |
| 204 | Sem conteúdo (exclusão) |
| 400 | Dados inválidos |
| 401 | Não autenticado (token ausente ou expirado) |
| 404 | Recurso não encontrado |
| 500 | Erro interno do servidor |
