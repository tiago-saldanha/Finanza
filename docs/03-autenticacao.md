# 03 — Autenticação

## Visão geral

O Finanza usa autenticação baseada em **JWT Bearer Token**. O token é armazenado no `localStorage` do navegador e enviado automaticamente em todas as requisições autenticadas.

---

## Fluxos disponíveis

### Registro

1. Acesse `/register`
2. Preencha: **Nome completo**, **E-mail**, **Senha** (mínimo 6 caracteres)
3. Ao confirmar, um banco SQLite exclusivo é provisionado para o usuário
4. O sistema redireciona automaticamente para o login

### Login

1. Acesse `/login`
2. Preencha **E-mail** e **Senha**
3. O token JWT (validade: 60 minutos) é armazenado localmente
4. O sistema redireciona para o **Dashboard**

### Recuperação de senha

1. Na tela de login, clique em **Esqueci minha senha**
2. Informe o e-mail cadastrado
3. Um link de redefinição é enviado por e-mail (via Resend API)
4. Clique no link recebido → preencha a nova senha em `/reset-password`

### Troca de senha (usuário autenticado)

1. Acesse **Minha Conta** (`/account`) no menu lateral
2. Vá até a seção **Segurança**
3. Preencha a senha atual e a nova senha
4. Clique em **Salvar**

### Logout

Clique no ícone de saída na barra superior (canto direito). O token é removido do `localStorage` e o usuário é redirecionado para o login.

### Exclusão de conta

Em **Minha Conta**, botão **Excluir conta**. Esta ação remove permanentemente o usuário e todos os seus dados financeiros (banco de tenant).

---

## Segurança

- Todas as rotas do frontend são protegidas pelo `authGuard` — sem token válido, o usuário é redirecionado para `/login`
- O token é enviado no header `Authorization: Bearer <token>`
- O backend valida o token em cada requisição; expirado, retorna `401 Unauthorized`
- A senha é armazenada com hash (nunca em texto puro)

---

## Exportação de dados

Em **Minha Conta**, botão **Exportar dados**. Gera um arquivo JSON com todas as transações do usuário para uso externo.
