# Módulo Catalog

## Propósito e responsabilidades

Catalog mantém os jogos disponíveis, seus dados descritivos, categoria, preço
base e estado ativo.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Catalog.Domain` | agregado `Game`, `GamePrice` e repositório |
| `FiapCloudGames.Catalog.Application` | `Games/` com create, update, get, list, mappings e `CatalogModule` |
| `FiapCloudGames.Catalog.Contracts` | arquivos separados para `GetGameQuery`, `GameSnapshot` e `ICatalogModule` |
| `FiapCloudGames.Catalog.Infrastructure` | EF Core, repositório e unidade de trabalho |
| `FiapCloudGames.Catalog.Presentation` | `Features/Games/` com Request, Response, mapping e `GamesController` |

Casos de uso atuais: `CreateGameService`, `UpdateGameService`,
`GetGameService` e `ListGamesService`, todos com `ExecuteAsync`.

## Endpoints

- `GET /api/games` — público, somente ativos;
- `GET /api/games/{id}` — público;
- `POST /api/games` — Administrator;
- `PUT /api/games/{id}` — Administrator.

## Persistência

`CatalogDbContext` é dono de `catalog.games`.

## Integrações

Expõe `ICatalogModule`. Promotions usa o contrato para validar jogos e Library o
usa durante a aquisição. Consumidores recebem `GameSnapshot`, nunca `Game` ou
`GameResult`. Não há evento de integração implementado.

## Regras principais

- título entre 2 e 160 caracteres;
- categoria obrigatória com até 80 caracteres;
- descrição com até 4.000 caracteres;
- preço não negativo e arredondado em duas casas;
- novo jogo ativo;
- listagem pública de ativos ordenada por título.

Veja [regras de negócio](../../../docs/development/business-rules.md).

## Testar

```powershell
dotnet test tests/Unit/FiapCloudGames.Catalog.UnitTests
```

## Evolução

- `TODO: definir moeda, paginação e busca.`
- `TODO: definir se a desativação precisa de integração assíncrona e outbox.`
