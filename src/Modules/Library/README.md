# Módulo Library

## Propósito e responsabilidades

Library representa a coleção adquirida por um usuário e registra o identificador
do jogo e um snapshot do preço, promoção e instante da aquisição. O título não é
persistido: ele é consultado no Catalog ao montar a resposta.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Library.Domain` | agregado `GameLibrary`, `LibraryGame` e `AcquisitionPrice` |
| `FiapCloudGames.Library.Application` | `UserLibrary/` com aquisição/consulta e `Abstractions/` com `ILibraryQueries` e `LibraryGameReadModel` |
| `FiapCloudGames.Library.Contracts` | arquivos separados para `GetUserLibraryQuery`, snapshots e `ILibraryModule` |
| `FiapCloudGames.Library.Infrastructure` | EF Core, repositório, query e unidade de trabalho |
| `FiapCloudGames.Library.Presentation` | `Features/UserLibrary/` com Responses, mapping e `LibraryController` |

Casos de uso atuais: `AcquireGameService` e `GetLibraryService`, ambos com
`ExecuteAsync`. `LibraryModule` adapta o resultado da Application para os
Snapshots públicos internos.

## Endpoints

- `GET /api/library` — usuário autenticado;
- `POST /api/library/games/{gameId}` — usuário autenticado.

## Persistência

`LibraryDbContext` é dono de `library.game_libraries` e
`library.library_games`. IDs de usuário, jogo e promoção são referências lógicas;
não há chaves estrangeiras para outros schemas.

## Integrações

Application depende de:

- `IIdentityModule` para conferir usuário;
- `ICatalogModule` para conferir jogo/preço;
- `IPromotionsModule` para cotar desconto.

Essas dependências usam exclusivamente projetos Contracts, regra protegida por
teste de arquitetura. Não há evento de integração implementado.

## Regras principais

- usuário e jogo ativos;
- uma biblioteca por usuário;
- jogo sem duplicidade;
- snapshot de preço/promoção;
- aquisição sem processamento de pagamento.

Veja [regras de negócio](../../../docs/development/business-rules.md) e
[fluxo de requisição](../../../docs/architecture/request-flow.md).

## Testar

```powershell
dotnet test tests/Unit/FiapCloudGames.Library.UnitTests
```

## Evolução

- `TODO: definir pagamento, idempotência e compensação.`
- `TODO: definir concorrência para aquisições simultâneas.`
- `TODO: definir se a aquisição precisa de integração assíncrona e outbox.`
- `TODO: adicionar GET do item ou corrigir o Location da criação.`
