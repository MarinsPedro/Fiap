# Erros da API

## Problem Details

Falhas HTTP retornam `application/problem+json`:

```json
{
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "O jogo informado não foi encontrado.",
  "instance": "/api/games/22222222-2222-2222-2222-222222222222",
  "code": "not_found",
  "traceId": "00-..."
}
```

O campo `type` é opcional. Quando fornecido pelo ASP.NET Core, aponta para uma
referência da especificação HTTP, nunca para uma documentação fictícia da
aplicação.

Falhas funcionais expõem uma mensagem controlada. Erros inesperados retornam
`Ocorreu um erro interno inesperado.` e preservam detalhes técnicos somente no
log. Validações com vários campos acrescentam `errors`.

## Catálogo

| Status | Código | Categoria/origem |
|---:|---|---|
| 400 | `validation_error` | `Validation` ou erro de modelo |
| 400 | `bad_request` | resposta 400 vazia do framework |
| 401 | `authentication_error` | `Authentication` |
| 401 | `authentication_required` | challenge do framework |
| 403 | `forbidden` | `Forbidden` ou forbid do framework |
| 404 | `not_found` | `NotFound` ou rota inexistente |
| 409 | `conflict` | `Conflict` |
| 422 | `business_rule_violation` | `BusinessRule` |
| 422 | `domain_rule_violation` | `DomainRuleViolationException` |
| 500 | `internal_error` | falha inesperada |

## Diagnóstico

1. Registre método, rota, status e horário.
2. Capture o `traceId`.
3. Não copie token, senha ou connection string.
4. Consulte os logs da mesma execução.
5. Para validação, verifique todas as entradas de `errors`.

Veja [tratamento de erros](../development/error-handling.md) e
[troubleshooting](../operations/troubleshooting.md).
