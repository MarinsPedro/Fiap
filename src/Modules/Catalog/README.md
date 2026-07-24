# Módulo Catalog

## Propósito e responsabilidades

Catalog mantém os jogos disponíveis, seus dados descritivos, categoria, preço
base e estado ativo.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Catalog.Domain` | entidade `Game` e repositório de domínio |
| `FiapCloudGames.Catalog.Application` | criar, atualizar, listar e consultar |
| `FiapCloudGames.Catalog.Contracts` | `GameSummary`, `ICatalogModule` e evento declarado |
| `FiapCloudGames.Catalog.Infrastructure` | EF Core, repositório e unidade de trabalho |
| `FiapCloudGames.Catalog.Presentation` | `GamesController` |

## Endpoints

- `GET /api/games` — público, somente ativos;
- `GET /api/games/{id}` — público;
- `POST /api/games` — Administrator;
- `PUT /api/games/{id}` — Administrator.

## Persistência

`CatalogDbContext` é dono de `catalog.games`.

## Integrações

Expõe `ICatalogModule`. Promotions usa o contrato para validar jogos e Library o
usa durante a aquisição. `GameDeactivatedIntegrationEvent` está declarado, mas
não existe publicação ou consumidor.

## Regras principais

- título entre 2 e 160 caracteres;
- categoria obrigatória;
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
- `TODO: implementar publicação de desativação.`
- `TODO: alinhar limites de strings entre domínio, API e banco.`

