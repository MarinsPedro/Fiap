# Tratamento de erros

## Pipeline

`ExceptionHandlingMiddleware` envolve o pipeline da API, registra falhas e converte
exceções conhecidas em respostas HTTP. O corpo segue o formato Problem Details
com `status`, `title`, `detail` e `instance`; o writer padrão também pode
acrescentar extensões de diagnóstico, como `traceId`.

Exemplo ilustrativo:

```json
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Jogo não encontrado.",
  "instance": "/api/games/11111111-1111-1111-1111-111111111111",
  "traceId": "00-..."
}
```

## Mapeamento atual

| Exceção | Status | Uso esperado |
|---|---:|---|
| `UnauthorizedAccessException` | 401 | autenticação recusada pela aplicação |
| `KeyNotFoundException` | 404 | recurso não encontrado |
| `ArgumentException` | 400 | entrada inválida |
| `InvalidOperationException` | 422 | regra de negócio não satisfeita |
| qualquer outra | 500 | falha inesperada |

O middleware não possui mapeamento para `409 Conflict`. Conflitos de unicidade que
escapem da validação da aplicação podem resultar em 500.

## Actions que retornam diretamente

Controllers também podem usar resultados como `NotFound()`, `Unauthorized()` e o
middleware de autorização pode gerar `401`/`403`. Essas respostas não
necessariamente carregam o mesmo Problem Details do middleware.

## Orientações

- Lance exceções de domínio/aplicação que já tenham tradução conhecida.
- Não inclua senha, token, connection string ou dados pessoais no texto da
  exceção.
- Use 400 para formato/argumento, 404 para ausência e 422 para uma operação bem
  formada que viola a regra atual.
- Registre contexto técnico no log, mas devolva uma mensagem segura ao cliente.
- Não capture uma exceção apenas para lançá-la novamente sem acrescentar contexto.

## Lacunas

- `TODO: padronizar respostas produzidas diretamente pelos controllers e pela
  autenticação.`
- `TODO: definir catálogo estável de códigos de erro.`
- `TODO: mapear violações de unicidade e concorrência para 409 quando aplicável.`
- `TODO: revisar a exposição de mensagens internas em respostas 500.`

Consulte também [Erros da API](../api/errors.md) e
[Logging e monitoramento](../operations/logging-monitoring.md).
