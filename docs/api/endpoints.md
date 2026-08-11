# Endpoints

Todos os exemplos usam IDs e dados ilustrativos.

## Resumo

| Método | Rota | Acesso | Sucesso |
|---|---|---|---:|
| GET | `/health` | público | 200 |
| POST | `/api/users` | público | 201 |
| POST | `/api/auth/login` | público | 200 |
| GET | `/api/users/me` | autenticado | 200 |
| GET | `/api/users/{id}` | Administrator | 200 |
| DELETE | `/api/users/{id}` | Administrator | 204 |
| GET | `/api/games` | público | 200 |
| GET | `/api/games/{id}` | público | 200 |
| POST | `/api/games` | Administrator | 201 |
| PUT | `/api/games/{id}` | Administrator | 200 |
| GET | `/api/promotions/active` | público | 200 |
| POST | `/api/promotions` | Administrator | 201 |
| POST | `/api/promotions/{id}/end` | Administrator | 204 |
| GET | `/api/library` | autenticado | 200 |
| POST | `/api/library/games/{gameId}` | autenticado | 201 |

## Health

### `GET /health`

Retorna `200 OK` e o texto `Healthy`. Não consulta o banco.

## Identity

### `POST /api/users`

```json
{
  "name": "Aluno FIAP",
  "email": "aluno@example.com",
  "password": "<senha-local-com-8-ou-mais-caracteres>"
}
```

Retorna `201` com:

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "name": "Aluno FIAP",
  "email": "aluno@example.com",
  "role": "User",
  "isActive": true
}
```

O `Location` aponta para `GET /api/users/{id}`, que exige Administrator; o usuário
recém-criado não consegue seguir esse link com seu papel padrão.

### `POST /api/auth/login`

```json
{
  "email": "aluno@example.com",
  "password": "<senha-local>"
}
```

Retorna `200` com `accessToken`, `expiresAtUtc` e o objeto `user`. Credencial
inválida ou usuário inativo retorna 401.

### `GET /api/users/me`

Retorna um `UserResponse` associado ao claim de identificador do JWT.

### `GET /api/users/{id}`

Retorna `UserResponse` ou 404. Exige Administrator.

### `DELETE /api/users/{id}`

Inativa o usuário e retorna 204. Exige Administrator. Um ID inexistente retorna
404.

## Catalog

Formato de jogo:

```json
{
  "id": "22222222-2222-2222-2222-222222222222",
  "title": "Cloud Quest",
  "description": "Uma aventura cooperativa.",
  "category": "RPG",
  "basePrice": 99.9,
  "isActive": true
}
```

### `GET /api/games`

Retorna array de jogos ativos, ordenado por título.

### `GET /api/games/{id}`

Retorna o jogo ou 404.

### `POST /api/games`

Exige Administrator:

```json
{
  "title": "Cloud Quest",
  "description": "Uma aventura cooperativa.",
  "category": "RPG",
  "basePrice": 99.9
}
```

Retorna 201 e `Location` para o GET do jogo.

### `PUT /api/games/{id}`

Exige Administrator e recebe o estado editável completo:

```json
{
  "title": "Cloud Quest Deluxe",
  "description": "Edição atualizada.",
  "category": "RPG",
  "basePrice": 119.9,
  "isActive": true
}
```

Retorna 200 com o jogo atualizado ou 404 quando o serviço não encontra o ID.

## Promotions

Formato de promoção:

```json
{
  "id": "33333333-3333-3333-3333-333333333333",
  "name": "FIAP Week",
  "discountPercent": 25,
  "startsAtUtc": "2026-07-23T12:00:00+00:00",
  "endsAtUtc": "2026-07-24T12:00:00+00:00",
  "gameIds": [
    "22222222-2222-2222-2222-222222222222"
  ]
}
```

### `GET /api/promotions/active`

Retorna as promoções ativas no instante da requisição.

### `POST /api/promotions`

Exige Administrator:

```json
{
  "name": "FIAP Week",
  "discountPercent": 25,
  "startsAtUtc": "2026-07-23T12:00:00+00:00",
  "endsAtUtc": "2026-07-24T12:00:00+00:00",
  "gameIds": [
    "22222222-2222-2222-2222-222222222222"
  ]
}
```

Retorna 201. O `Location` aponta para `/api/promotions/{id}`, mas não existe GET
para essa rota.

### `POST /api/promotions/{id}/end`

Marca a promoção como encerrada e retorna 204. Exige Administrator.

## Library

### `GET /api/library`

Retorna a biblioteca do usuário autenticado:

```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "games": [
    {
      "id": "44444444-4444-4444-4444-444444444444",
      "gameId": "22222222-2222-2222-2222-222222222222",
      "gameTitle": "Cloud Quest",
      "pricePaid": 74.92,
      "promotionId": "33333333-3333-3333-3333-333333333333",
      "acquiredAtUtc": "2026-07-23T13:00:00+00:00"
    }
  ]
}
```

### `POST /api/library/games/{gameId}`

Não possui corpo. Verifica usuário, jogo e promoção, registra o snapshot da
aquisição e retorna `201` com um `LibraryItemResponse`.

Não há pagamento. O `Location` retornado usa
`/api/library/games/{gameId}`, mas essa rota só aceita POST; não existe GET do item
individual.
