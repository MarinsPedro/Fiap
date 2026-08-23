# Convenções da API

## Fonte dinâmica

O OpenAPI é a referência para operações concretas. Este documento descreve
convenções que devem orientar qualquer operação HTTP da aplicação.

## Rotas

Rotas de negócio usam o prefixo `/api`. O health check usa `/health`.
Identificadores em rota usam GUID e constraint `:guid` quando aplicável.

A API ainda não adota prefixo de versão. A estratégia de evolução está em
[DOC-002](../backlog.md).

## Métodos

- `GET` consulta recursos e não altera estado;
- `POST` cria recursos ou executa comandos explícitos;
- `PUT` substitui o estado editável aceito pelo contrato;
- `DELETE` representa remoção ou desativação conforme a regra do recurso.

Operações de criação retornam `201 Created` e devem fornecer um `Location`
resolvido por uma operação de leitura existente. Comandos sem corpo de resposta
usam `204 No Content` quando apropriado.

## JSON

O contrato usa propriedades `camelCase` e enums como texto. Datas de negócio
são `DateTimeOffset` em UTC e serializadas em ISO 8601.

Valores monetários e percentuais são números JSON. Moeda e impostos ainda
dependem de [DOC-003](../backlog.md).

## Coleções

Coleções atuais são representadas como arrays. Paginação, filtros, ordenação
configurável e limites precisam seguir uma política única antes de serem
introduzidos; consulte [DOC-002](../backlog.md).

Não documente manualmente quais operações retornam coleções: consulte o schema no
OpenAPI.

## Idempotência

O protocolo ainda não oferece `Idempotency-Key`. Operações naturalmente
idempotentes no domínio não substituem uma política HTTP para repetição de
requests. A decisão pertence a [DOC-002](../backlog.md).

## Headers

Use `Content-Type: application/json` quando houver body JSON e
`Authorization: Bearer <token>` nas operações protegidas.

Requisitos por operação são declarados no OpenAPI.
