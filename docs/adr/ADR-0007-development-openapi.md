# ADR-0007: OpenAPI somente em Development

## Status

Aceito.

## Contexto

A especificação facilita desenvolvimento, mas o host de produção não deve expô-la sem decisão explícita.

## Decisão

Registrar OpenAPI em todos os ambientes e mapear `/openapi/v1.json` somente quando `app.Environment.IsDevelopment()`.

## Consequências

- perfis locais `http` e `https` abrem o JSON;
- Docker Compose usa Production e não expõe a rota;
- não há Swagger UI;
- consumidores externos precisam obter a especificação em Development ou por artefato gerado fora do fluxo atual.

## Alternativas consideradas

Exposição autenticada em produção e geração estática no pipeline não foram implementadas.
