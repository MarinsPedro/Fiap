# Autenticação da API

## Cadastro

```http
POST /api/users
Content-Type: application/json

{
  "name": "Aluno FIAP",
  "email": "aluno@example.com",
  "password": "<senha-local-com-8-ou-mais-caracteres>"
}
```

Resposta `201 Created`:

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "name": "Aluno FIAP",
  "email": "aluno@example.com",
  "role": "User",
  "isActive": true
}
```

## Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "aluno@example.com",
  "password": "<senha-local>"
}
```

Resposta `200 OK`:

```json
{
  "accessToken": "<jwt>",
  "expiresAtUtc": "2026-07-23T14:00:00+00:00",
  "user": {
    "id": "11111111-1111-1111-1111-111111111111",
    "name": "Aluno FIAP",
    "email": "aluno@example.com",
    "role": "User",
    "isActive": true
  }
}
```

## Chamada autenticada

```powershell
$token = "<jwt-retornado-pelo-login>"
Invoke-RestMethod `
  -Uri "http://localhost:5080/api/users/me" `
  -Headers @{ Authorization = "Bearer $token" }
```

## Autorização

- `[Authorize]`: qualquer usuário autenticado.
- `[Authorize(Roles = "Administrator")]`: somente administrador.

O administrador inicial é criado pelo migrador, não pela API pública. Veja
[autenticação e autorização no desenvolvimento](../development/authentication-authorization.md).

## Limitações

O access token dura duas horas. Não há refresh token, revogação, MFA,
recuperação de senha ou troca de papel por endpoint.

