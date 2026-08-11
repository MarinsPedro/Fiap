# Objetos e contratos por fronteira

## Fluxo de dados

Cada representação pertence à fronteira em que é utilizada:

```text
HTTP
  ↓
Request (Presentation)
  ↓
Input (Application)
  ↓
Entity / Value Object (Domain)
  ↓
Result (Application)
  ↓
Response (Presentation)
  ↓
HTTP
```

Entre módulos:

```text
Application consumidora
  ↓
Query do módulo fornecedor
  ↓
I<Module>Module
  ↓
Snapshot imutável
```

A entidade de domínio nunca é devolvida diretamente pela API nem enviada para
outro módulo.

## Convenções

| Fronteira | Entrada | Saída | Exemplos |
|---|---|---|---|
| Presentation | `Request` | `Response` | `CreateGameRequest`, `GameResponse` |
| Application | `Input` ou argumentos simples | `Result` | `CreateGameInput`, `GameResult` |
| Porta de leitura da Application | argumentos simples | `ReadModel` | `LibraryGameReadModel` |
| Domain | Entity / Value Object | Entity / Value Object | `Game`, `GamePrice` |
| Contracts | `Query` | `Snapshot` | `GetGameQuery`, `GameSnapshot` |
| Integração externa futura | DTO específico | DTO específico | `PaymentRequestDto` |

Não são criados Requests ou Inputs vazios apenas para obedecer à convenção. Um
resultado pode ser compartilhado entre casos de uso quando representa a mesma
visão de forma coerente.

## Presentation

Requests e Responses representam o contrato HTTP. Controllers convertem o
Request para Input antes de chamar a Application e convertem o Result para
Response antes de responder.

Exemplo do Catalog:

```csharp
var result = await service.ExecuteAsync(
    request.ToInput(),
    cancellationToken);

var response = result.ToResponse();
return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
```

O service não recebe um Request e não devolve um Response. Assim, pode ser
acionado por HTTP, testes, jobs ou outro adapter sem depender do ASP.NET Core.

## Application

Inputs representam os dados necessários para executar um caso de uso. Results
representam sua saída sem semântica de transporte HTTP.

Os casos de uso permanecem como classes `*Service` com `ExecuteAsync`; esta
separação não exige MediatR, Commands ou Handlers.

Mapeamentos de entidade para Result ficam na Application:

```csharp
internal static class GameApplicationMappings
{
    public static GameResult ToResult(Game game) => new(
        game.Id,
        game.Title,
        game.Description,
        game.Category,
        game.BasePrice.Amount,
        game.IsActive);
}
```

Uma projeção técnica devolvida por uma porta de leitura usa `ReadModel`, não
`Result`. Atualmente `ILibraryQueries.ListGamesAsync` devolve
`LibraryGameReadModel`; a Infrastructure cria essa projeção sem tracking e
`GetLibraryService` a enriquece com o título do Catalog antes de produzir
`UserLibraryResult`.

## Contracts entre módulos

Contracts formam a API pública interna do bounded context. O consumidor conhece
somente o projeto Contracts do fornecedor.

Consultas usam uma Query explícita e devolvem um Snapshot mínimo e imutável:

```csharp
public sealed record GetGameQuery(Guid GameId);

public sealed record GameSnapshot(
    Guid Id,
    string Title,
    decimal BasePrice,
    bool IsActive);

public interface ICatalogModule
{
    Task<GameSnapshot?> GetGameAsync(
        GetGameQuery query,
        CancellationToken cancellationToken);
}
```

`Snapshot` deixa explícito que o objeto não é a entidade, não permite mudança de
estado e representa os dados observados naquele instante. Ele contém apenas os
campos necessários pelos consumidores.

Os contratos síncronos implementados são:

| Módulo | Query | Snapshot |
|---|---|---|
| Identity | `GetUserQuery` | `UserSnapshot` |
| Catalog | `GetGameQuery` | `GameSnapshot` |
| Promotions | `GetPriceQuoteQuery` | `PriceQuoteSnapshot` |
| Library | `GetUserLibraryQuery` | `UserLibrarySnapshot` |

## Validação

| Local | Responsabilidade |
|---|---|
| Presentation / model binding | JSON, tipos e formato do contrato HTTP |
| Application | coordenação, existência, permissão, conflito e dependências entre módulos |
| Domain | invariantes válidas em qualquer entrada ou adapter |

A validação de entrada na Application pode antecipar uma resposta 400 com todos
os campos, mas não substitui as invariantes do Domain.

## Proteções automatizadas

Os testes arquiteturais verificam que:

- services da Application não retornam tipos de Contracts;
- ações de Controller não retornam tipos de Application ou Contracts;
- Presentation não referencia Contracts diretamente;
- tipos públicos de Contracts terminam em `Query` ou `Snapshot`, exceto as
  interfaces `I*Module`;
- dependências entre módulos continuam restritas aos Contracts.
