# 02 — Instalação e Execução Local

## Pré-requisitos

| Ferramenta | Versão mínima |
|---|---|
| .NET SDK | 8.0 |
| Node.js | 18.x ou superior |
| Angular CLI | 18.x (`npm install -g @angular/cli`) |
| dotnet-ef (opcional) | `dotnet tool install --global dotnet-ef` |

---

## 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd Finanza
```

---

## 2. Configurar o backend

### 2.1 — `appsettings.json`

Edite `src/Finanza.API/appsettings.json` e preencha:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=..\\Finanza.Infrastructure\\Tenants\\app.db"
  },
  "TenantDb": {
    "BaseFolder": "..\\Finanza.Infrastructure\\Tenants"
  },
  "Jwt": {
    "SecretKey": "SUA_CHAVE_SECRETA_AQUI_MINIMO_32_CARACTERES",
    "Issuer": "Finanza.API",
    "Audience": "Finanza.Client",
    "ExpiresInMinutes": "60"
  },
  "Resend": {
    "ApiKey": "re_xxxxxxxxxxxx"
  },
  "App": {
    "FrontendUrl": "http://localhost:4200",
    "FromEmail": "finanza@seudominio.com",
    "FromName": "Finanza"
  }
}
```

> **Resend**: necessário para e-mails de recuperação de senha. Crie uma conta gratuita em [resend.com](https://resend.com).

### 2.2 — Criar pasta de tenants

```bash
mkdir src/Finanza.Infrastructure/Tenants
```

### 2.3 — Executar o backend

```bash
cd src/Finanza.API
dotnet run
```

A API estará disponível em `https://localhost:7xxx` (porta exibida no terminal).

---

## 3. Configurar o frontend

### 3.1 — Instalar dependências

```bash
cd src/Finanza.Client
npm install
```

### 3.2 — Verificar URL da API

Edite `src/environments/environment.ts` se a porta da API for diferente:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7XXX/api'
};
```

### 3.3 — Executar o frontend

```bash
npm run start
```

O app abrirá automaticamente em `http://localhost:4200`.

---

## 4. Primeiro acesso

1. Acesse `http://localhost:4200`
2. Clique em **Criar conta**
3. Preencha nome, e-mail e senha
4. Faça login — o banco de dados do seu usuário é criado automaticamente

---

## 5. Build para produção

### Backend
```bash
dotnet publish src/Finanza.API -c Release -o ./publish
```

### Frontend
```bash
cd src/Finanza.Client
npm run build
```

Os arquivos estáticos serão gerados em `dist/finanza-client/browser/`.
