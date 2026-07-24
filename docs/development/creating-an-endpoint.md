# Criar um endpoint do zero

Este guia usa dois endpoints existentes do Catalog como referência:

- consulta: `GET /api/games/{id}`;
- alteração: `POST /api/games`.

Eles representam o padrão real do repositório: Controller → Application Service → Domain/Repository → `DbContext`. Não há MediatR, Command/Query Handler ou FluentValidation.

## 1. Escolher o módulo

Pergunte qual domínio possui a regra e o dado.

| Assunto | Módulo |
|---|---|
| usuário, login, role | Identity |
| jogo e preço base | Catalog |
| desconto e vigência | Promotions |
| aquisição e biblioteca | Library |

Se a funcionalidade consulta outro módulo, referencie somente `Contracts`. Não referencie Application, Domain ou Infrastructure externos.

## 2. Identificar as camadas

Para Catalog:

```text
src/Modules/Catalog/
├── FiapCloudGames.Catalog.Domain
├── FiapCloudGames.Catalog.Contracts
├── FiapCloudGames.Catalog.Application
├── FiapCloudGames.Catalog.Infrastructure
└── FiapCloudGames.Catalog.Presentation
```

## 3. Criar ou alterar a entidade

O endpoint de criação usa `Game.Create`, e a atualização usa `Game.Update`.

```csharp
public static Game Create(
    string title,
    string description,
    string category,
    decimal basePrice) =>
    new(Guid.NewGuid(), title, description, category, basePrice);

public void Update(
    string title,
    string description,
    string category,
    decimal basePrice)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(title);
    ArgumentException.ThrowIfNullOrWhiteSpace(category);

    var normalizedTitle = title.Trim();
    if (normalizedTitle.Length is < 2 or > 160)
    {
        throw new InvalidOperationException(
            "O título deve ter entre 2 e 160 caracteres.");
    }

    if (basePrice < 0)
    {
        throw new InvalidOperationException(
            "O preço base não pode ser negativo.");
    }

    Title = normalizedTitle;
    Description = description?.Trim() ?? string.Empty;
    Category = category.Trim();
    BasePrice = decimal.Round(
        basePrice,
        2,
        MidpointRounding.ToEven);
}
```

Regra: mantenha invariantes dentro da entidade quando definem estado válido.

## 4. Criar DTO HTTP de entrada

Requests pertencem a Presentation. O record existente é:

```csharp
public sealed record GameRequest(
    string Title,
    string Description,
    string Category,
    decimal BasePrice);
```

Não reutilize entidade de Domain como body HTTP.

## 5. Criar input/output da Application

O input existente:

```csharp
public sealed record CreateGameInput(
    string Title,
    string Description,
    string Category,
    decimal BasePrice);
```

O output usado pelo endpoint é o contrato público:

```csharp
public sealed record GameSummary(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
```

Use Contracts quando outro módulo também precisa do DTO. Para um output exclusivamente interno, siga os records da Application.

## 6. Command, Query, Handler ou Service

Estado atual:

- não existem Commands;
- não existem Queries;
- não existem Handlers;
- casos de uso são classes `*Service` com `ExecuteAsync`.

Portanto, não introduza um Handler isolado em apenas uma funcionalidade sem decisão arquitetural.

Consulta existente:

```csharp
public sealed class GetGameService(IGameRepository games)
{
    public async Task<GameSummary?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var game = await games.GetAsync(id, cancellationToken);
        return game is null
            ? null
            : CatalogMappings.ToSummary(game);
    }
}
```

Alteração existente:

```csharp
public sealed class CreateGameService(
    IGameRepository games,
    ICatalogUnitOfWork unitOfWork)
{
    public async Task<GameSummary> ExecuteAsync(
        CreateGameInput input,
        CancellationToken cancellationToken)
    {
        var game = Game.Create(
            input.Title,
            input.Description,
            input.Category,
            input.BasePrice);

        await games.AddAsync(game, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CatalogMappings.ToSummary(game);
    }
}
```

## 7. Implementar validação

Estado atual:

- `[ApiController]` executa validação/model binding do ASP.NET Core;
- alguns checks de entrada ficam no service;
- invariantes ficam no Domain;
- limites físicos ficam no mapping/migration;
- não há classes `*Validator` nem pacote FluentValidation.

Para Game, título/preço são validados no Domain. Categoria e descrição possuem limites no banco que ainda não são espelhados integralmente no Domain.

```text
TODO: confirmar com a equipe se validators dedicados serão adotados.
```

Enquanto o padrão não mudar, valide estado da entidade no Domain e coordenação no service. Não coloque regra no Controller.

## 8. Criar a interface do repository

Interfaces de persistência pertencem ao Domain:

```csharp
public interface IGameRepository
{
    Task AddAsync(
        Game game,
        CancellationToken cancellationToken);

    Task<Game?> GetAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Game>> ListAsync(
        bool onlyActive,
        CancellationToken cancellationToken);
}
```

Adicione somente operações necessárias ao caso de uso. Não exponha `IQueryable`.

## 9. Implementar o repository

Infrastructure implementa EF Core:

```csharp
internal sealed class GameRepository(
    CatalogDbContext dbContext) : IGameRepository
{
    public async Task AddAsync(
        Game game,
        CancellationToken cancellationToken) =>
        await dbContext.Games.AddAsync(game, cancellationToken);

    public Task<Game?> GetAsync(
        Guid id,
        CancellationToken cancellationToken) =>
        dbContext.Games.SingleOrDefaultAsync(
            game => game.Id == id,
            cancellationToken);
}
```

Consultas somente leitura devem usar `AsNoTracking()`, como `ListAsync`.

## 10. Configurar persistência

O mapping fica em `CatalogDbContext.OnModelCreating`:

```csharp
modelBuilder.HasDefaultSchema("catalog");
modelBuilder.Entity<Game>(builder =>
{
    builder.ToTable("games");
    builder.HasKey(game => game.Id);
    builder.Property(game => game.Title)
        .HasColumnName("title")
        .HasMaxLength(160)
        .IsRequired();
    builder.Property(game => game.BasePrice)
        .HasColumnName("base_price")
        .HasPrecision(12, 2)
        .IsRequired();
});
```

Se a mudança não altera schema, nenhuma migration é necessária.

## 11. Criar migration

Migrations não são geradas por `dotnet ef`. Crie uma classe no projeto central:

```text
src/Database/FiapCloudGames.Database.Migrations/Migrations/
```

Modelo para uma alteração futura, claramente não existente hoje:

```csharp
using FluentMigrator;

namespace FiapCloudGames.Database.Migrations.Migrations;

[Migration(AAAAMMDDNNNN)]
public sealed class AddGamePublisher : Migration
{
    public override void Up()
    {
        Alter.Table("games")
            .InSchema("catalog")
            .AddColumn("publisher")
            .AsString(120)
            .Nullable();
    }

    public override void Down()
    {
        Delete.Column("publisher")
            .FromTable("games")
            .InSchema("catalog");
    }
}
```

Ao usar o modelo:

1. escolha identificador único e crescente;
2. atualize entidade, mapping e DTOs;
3. preserve compatibilidade de deploy;
4. valide contra PostgreSQL;
5. documente rollback.

Aplicação:

```powershell
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

O entry point atual não expõe rollback. Veja [Migrations](database-migrations.md).

## 12. Criar o Controller e a rota

Consulta:

```csharp
[AllowAnonymous]
[HttpGet("{id:guid}")]
public async Task<ActionResult<GameSummary>> Get(
    Guid id,
    [FromServices] GetGameService service,
    CancellationToken cancellationToken)
{
    var game = await service.ExecuteAsync(
        id,
        cancellationToken);

    return game is null ? NotFound() : Ok(game);
}
```

Alteração:

```csharp
[Authorize(Roles = "Administrator")]
[HttpPost]
public async Task<ActionResult<GameSummary>> Create(
    GameRequest request,
    [FromServices] CreateGameService service,
    CancellationToken cancellationToken)
{
    var game = await service.ExecuteAsync(
        new CreateGameInput(
            request.Title,
            request.Description,
            request.Category,
            request.BasePrice),
        cancellationToken);

    return CreatedAtAction(
        nameof(Get),
        new { id = game.Id },
        game);
}
```

Convenções observadas:

- base route `api/<recurso>`;
- constraint `:guid`;
- consulta pública com `[AllowAnonymous]`;
- escrita administrativa com `[Authorize(Roles = "Administrator")]`;
- `CancellationToken` propagado;
- 201 com `Location` para criação;
- 200 para consulta;
- 404 quando `Get` retorna nulo.

## 13. Registrar na injeção de dependência

Em `FiapCloudGames.Catalog.Application/DependencyInjection.cs`:

```csharp
services.AddScoped<CreateGameService>();
services.AddScoped<GetGameService>();
```

Repository e Unit of Work são registrados em Infrastructure:

```csharp
services.AddScoped<IGameRepository, GameRepository>();
services.AddScoped<ICatalogUnitOfWork>(
    provider => provider.GetRequiredService<CatalogDbContext>());
```

Um novo módulo Presentation também exige `AddApplicationPart` na API.

## 14. Códigos HTTP e erros

| Cenário | Resultado atual |
|---|---|
| criação válida | 201 |
| consulta válida | 200 |
| jogo não encontrado na consulta | 404 direto do MVC |
| ID inexistente na atualização | 404 Problem Details |
| entrada inválida | 400 ou 422, conforme exceção |
| sem token em rota admin | 401 |
| role incorreta | 403 |
| erro inesperado | 500 |

Não use 409 na documentação de uma action sem implementar exceção/mapeamento correspondente.

## 15. Logs

O padrão atual não injeta `ILogger` nos services/Controllers. O middleware registra exceções 4xx lançadas como warning e 5xx como error.

```text
TODO: política de logs de negócio não identificada.
```

Se logging de caso de uso for necessário, defina primeiro eventos, nível e dados proibidos; nunca registre senha, JWT ou connection string.

## 16. Testes unitários

Teste real de regra:

```csharp
[Fact]
public void CreateShouldRejectNegativePrice()
{
    Assert.Throws<InvalidOperationException>(() =>
        Game.Create(
            "Cloud Quest",
            "Aventura",
            "RPG",
            -0.01m));
}
```

O repositório não contém biblioteca de mocks. Services não possuem testes unitários atuais; ao adicioná-los, um fake manual de `IGameRepository` é compatível sem novo pacote.

## 17. Teste de integração

A infraestrutura existente usa:

```csharp
public sealed class FiapCloudGamesApiFactory
    : WebApplicationFactory<Program>
```

Ela valida somente `/health` e não substitui os `DbContext`. Uma chamada a endpoint de dados tentará usar a connection string de teste em `localhost`.

```text
TODO: definir infraestrutura de PostgreSQL real/Testcontainers e limpeza de dados antes de adicionar testes de endpoint com persistência.
```

Não documente um teste autenticado como executável até essa infraestrutura existir.

## 18. OpenAPI

Controllers/records são descobertos automaticamente. Depois da alteração:

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
Invoke-WebRequest https://localhost:7080/openapi/v1.json
```

Procure a rota e confirme método, request, response e requisito de autorização. Não há Swagger UI nem anotações de descrição.

## 19. Chamada manual

Criar:

```http
POST /api/games
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "title": "Cloud Quest",
  "description": "Aventura cooperativa",
  "category": "RPG",
  "basePrice": 99.90
}
```

Resposta 201:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "title": "Cloud Quest",
  "description": "Aventura cooperativa",
  "category": "RPG",
  "basePrice": 99.90,
  "isActive": true
}
```

O UUID acima é ilustrativo; a aplicação gera um valor novo.

Consultar:

```http
GET /api/games/{id}
```

## 20. Validar

```powershell
dotnet restore FiapCloudGames.sln
dotnet build FiapCloudGames.sln --no-restore
dotnet test FiapCloudGames.sln --no-build --no-restore
dotnet format FiapCloudGames.sln --verify-no-changes --no-restore
```

## Checklist final

- [ ] O endpoint está no módulo correto.
- [ ] A regra não está no Controller.
- [ ] Domain preserva estado válido.
- [ ] Input/output não expõem entidade.
- [ ] Dependências externas usam Contracts.
- [ ] Repository não expõe `IQueryable`.
- [ ] Mapping e migration estão sincronizados.
- [ ] Service e repository foram registrados.
- [ ] Status HTTP corresponde à implementação.
- [ ] Autenticação/autorização foram revisadas.
- [ ] `CancellationToken` foi propagado.
- [ ] Testes unitários foram criados.
- [ ] Infraestrutura de integração foi considerada.
- [ ] Migration foi validada em PostgreSQL, quando aplicável.
- [ ] OpenAPI foi verificado em Development.
- [ ] README do módulo e documentação central foram atualizados.
- [ ] Logs não contêm dados sensíveis.
- [ ] Links Markdown funcionam.
