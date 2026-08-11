# Módulo Promotions

## Propósito e responsabilidades

Promotions cadastra campanhas para conjuntos de jogos, lista promoções ativas,
encerra campanhas e calcula o melhor preço vigente.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Promotions.Domain` | agregado `Promotion`, `PromotionGame` e `DiscountPercentage` |
| `FiapCloudGames.Promotions.Application` | `Promotions/` para campanhas e `Pricing/` para cotação |
| `FiapCloudGames.Promotions.Contracts` | arquivos separados para `GetPriceQuoteQuery`, `PriceQuoteSnapshot` e `IPromotionsModule` |
| `FiapCloudGames.Promotions.Infrastructure` | EF Core, repositório e unidade de trabalho |
| `FiapCloudGames.Promotions.Presentation` | `Features/Promotions/` com Request, Response, mapping e `PromotionsController` |

Casos de uso atuais: `CreatePromotionService`,
`ListActivePromotionsService`, `EndPromotionService` e
`GetPromotionalPriceService`, todos com `ExecuteAsync`.

## Endpoints

- `GET /api/promotions/active` — público;
- `POST /api/promotions` — Administrator;
- `POST /api/promotions/{id}/end` — Administrator.

## Persistência

`PromotionsDbContext` é dono de `promotions.promotions` e
`promotions.promotion_games`.

## Integrações

Application referencia apenas `FiapCloudGames.Catalog.Contracts` para validar os
jogos. Expõe `IPromotionsModule`, consumido por Library para cotação. Não há
vazamento de `Promotion` ou `PriceQuoteResult`; o consumidor recebe um
`PriceQuoteSnapshot`. Não há evento de integração implementado.

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
- `TODO: definir se o início precisa de integração assíncrona e outbox.`
- `TODO: adicionar consulta individual ou corrigir o Location da criação.`
