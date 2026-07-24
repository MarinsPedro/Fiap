# ADR-0006: Tratamento global com Problem Details

## Status

Aceito.

## Contexto

Exceções de Application/Domain não devem vazar stack trace ao cliente e precisam de status previsível.

## Decisão

Registrar `AddProblemDetails` e envolver o pipeline com `ExceptionHandlingMiddleware`.

Mapeamento:

- `UnauthorizedAccessException` → 401;
- `KeyNotFoundException` → 404;
- `ArgumentException` → 400;
- `InvalidOperationException` → 422;
- demais → 500 com detalhe genérico.

## Consequências

- falhas conhecidas têm corpo uniforme quando passam pelo middleware;
- falhas 5xx recebem log de erro;
- falhas 4xx lançadas recebem log de warning;
- respostas diretas de MVC e autenticação podem ter outro corpo;
- não existe mapeamento para 409 ou exceções de persistência.

## Alternativas consideradas

Filtros MVC e handlers do framework não estão implementados.
