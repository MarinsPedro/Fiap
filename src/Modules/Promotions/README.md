# Módulo Promotions

## Propósito e responsabilidades

Promotions cadastra campanhas para conjuntos de jogos, lista promoções ativas,
encerra campanhas e calcula o melhor preço vigente.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Promotions.Domain` | `Promotion`, `PromotionGame` e regras de desconto |
| `FiapCloudGames.Promotions.Application` | criar, listar, encerrar e cotar preço |
| `FiapCloudGames.Promotions.Contracts` | summaries, `IPromotionsModule` e evento declarado |
| `FiapCloudGames.Promotions.Infrastructure` | EF Core, repositório e unidade de trabalho |
| `FiapCloudGames.Promotions.Presentation` | `PromotionsController` |

## Endpoints

- `GET /api/promotions/active` — público;
- `POST /api/promotions` — Administrator;
- `POST /api/promotions/{id}/end` — Administrator.

## Persistência

`PromotionsDbContext` é dono de `promotions.promotions` e
`promotions.promotion_games`.

## Integrações

Application referencia apenas `FiapCloudGames.Catalog.Contracts` para validar os
jogos. Expõe `IPromotionsModule`, consumido por Library para cotação. O
`PromotionStartedIntegrationEvent` está declarado, porém não é publicado.

## Regras principais

- desconto maior que zero e até 100%;
- fim posterior ao início;
- ao menos um jogo distinto e ativo;
- atividade definida pelo intervalo e ausência de encerramento;
- maior desconto vigente aplicado ao jogo.

Veja [regras de negócio](../../../docs/development/business-rules.md).

## Testar

```powershell
dotnet test tests/Unit/FiapCloudGames.Promotions.UnitTests
```

## Evolução

- `TODO: definir política para sobreposição e alteração de campanhas.`
- `TODO: implementar publicação do evento de início.`
- `TODO: adicionar consulta individual ou corrigir o Location da criação.`

