# Módulo Library

## Propósito e responsabilidades

Library representa a coleção adquirida por um usuário e registra um snapshot do
jogo, preço, promoção e instante da aquisição.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Library.Domain` | `GameLibrary`, `LibraryGame` e prevenção de duplicidade |
| `FiapCloudGames.Library.Application` | consulta e aquisição |
| `FiapCloudGames.Library.Contracts` | summaries, `ILibraryModule` e evento declarado |
| `FiapCloudGames.Library.Infrastructure` | EF Core, repositório e unidade de trabalho |
| `FiapCloudGames.Library.Presentation` | `LibraryController` |

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
teste de arquitetura. `GameAddedToLibraryIntegrationEvent` está declarado, mas
não é publicado.

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
- `TODO: implementar publicação do evento de aquisição.`
- `TODO: adicionar GET do item ou corrigir o Location da criação.`

