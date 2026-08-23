# Erros da API

## Problem Details

Falhas HTTP retornam `application/problem+json`:

```json
{
  "type": "urn:fiap-cloud-games:problem:not-found",
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "O jogo informado não foi encontrado.",
  "traceId": "18904cfedc6a6bcb08f53c175daec39d"
}
```

`type` identifica de forma estável a categoria do problema. Os URNs são
mantidos enquanto não existir uma página pública e estável de documentação para
cada tipo. `traceId` usa o TraceId W3C de 32 caracteres quando há uma `Activity`
HTTP ativa e identifica a ocorrência para correlação com os logs; fora desse
contexto, utiliza `HttpContext.TraceIdentifier` como fallback.

Falhas funcionais expõem uma mensagem controlada. Erros inesperados retornam
`Não foi possível concluir a operação.` e preservam detalhes técnicos somente no
log. Validações com vários campos acrescentam `errors`.

```json
{
  "type": "urn:fiap-cloud-games:problem:validation",
  "title": "Um ou mais dados são inválidos",
  "status": 400,
  "detail": "Verifique os dados informados.",
  "traceId": "18904cfedc6a6bcb08f53c175daec39d",
  "errors": [
    {
      "message": "O nome deve possuir entre 2 e 120 caracteres.",
      "field": "name"
    },
    {
      "message": "O e-mail informado é inválido.",
      "field": "email"
    }
  ]
}
```

`errors` não é enviado em falhas que não sejam validações estruturadas. Cada
entrada possui `message` e pode possuir `field`; não existe código personalizado
por erro.

## Catálogo

| Status | Tipo | Categoria/origem |
|---:|---|---|
| 400 | `validation` | `Validation`, JSON ou erro de modelo |
| 400 | `bad-request` | resposta 400 vazia do framework |
| 401 | `unauthorized` | `Authentication` ou challenge do framework |
| 403 | `forbidden` | `Forbidden` ou forbid do framework |
| 404 | `not-found` | `NotFound` ou rota inexistente |
| 409 | `conflict` | `Conflict` |
| 422 | `business-rule` | `BusinessRule` ou `DomainRuleViolationException` |
| 500 | `internal-server-error` | falha inesperada |

## Diagnóstico

1. Registre método, rota, status e horário.
2. Capture o `traceId`.
3. Não copie token, senha ou connection string.
4. Consulte os logs da mesma execução.
5. Para validação, verifique todas as entradas de `errors`.

Veja [tratamento de erros](../development/error-handling.md) e
[troubleshooting](../operations/troubleshooting.md).
