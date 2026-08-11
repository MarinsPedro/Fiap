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
├── FiapCloudGames.Catalog.Application/
│   └── Games/
├── FiapCloudGames.Catalog.Infrastructure
└── FiapCloudGames.Catalog.Presentation/
    └── Features/Games/
```

Mantenha um tipo principal por arquivo. No fluxo atual, `CreateGameInput`,
`CreateGameService`, `GameResult` e `GameApplicationMappings` ficam em
Application/Games; Request, Response, mapping HTTP e Controller ficam em
Presentation/Features/Games.

## 3. Criar ou alterar a entidade

O endpoint de criação usa `Game.Create`, e a atualização usa
`Game.ChangeDetails`.

```csharp
public static Game Create(
    string title,
    string description,
    string category,
    decimal basePrice,
    DateTimeOffset createdAtUtc) =>
    new(
        Guid.NewGuid(),
        title,
        description,
        category,
        basePrice,
        createdAtUtc);

public void ChangeDetails(
    string title,
    string description,
    string category,
    decimal basePrice)
{
    Title = NormalizeTitle(title);
    Description = NormalizeDescription(description);
    Category = NormalizeCategory(category);
    BasePrice = GamePrice.Create(basePrice);
}

private static string NormalizeTitle(string? title)
{
    if (string.IsNullOrWhiteSpace(title))
    {
        throw new DomainRuleViolationException(
            "O título é obrigatório.");
    }

    var normalized = title.Trim();
    if (normalized.Length is
        < MinimumTitleLength or > MaximumTitleLength)
    {
        throw new DomainRuleViolationException(
            "O tamanho do título é inválido.");
    }

    return normalized;
}
```

Regra: mantenha invariantes dentro da entidade quando definem estado válido.

## 4. Criar Request e Response HTTP

Requests pertencem a Presentation. O record existente é:

```csharp
using System.ComponentModel.DataAnnotations;

public sealed record CreateGameRequest(
    [Required, StringLength(160, MinimumLength = 2)]
    string Title,

    [StringLength(4000)]
    string Description,

    [Required, StringLength(80)]
    string Category,

    [Range(0, double.MaxValue)]
    decimal BasePrice);
```

As anotações antecipam erros simples como resposta `400`. Não reutilize entidade
de Domain como body HTTP e mantenha no domínio as invariantes necessárias para
impedir a criação de estado inválido fora do endpoint.

Responses também pertencem a Presentation:

```csharp
public sealed record GameResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
```

## 5. Criar Input e Result da Application

O input existente:

```csharp
public sealed record CreateGameInput(
    string Title,
    string Description,
    string Category,
    decimal BasePrice);
```

O Result representa a saída do caso de uso sem semântica HTTP:

```csharp
public sealed record GameResult(
    Guid Id,
    string Title,
    string Description,
    string Category,
    decimal BasePrice,
    bool IsActive);
```

Não devolva `GameResponse` ou `GameSnapshot` pelo service. Presentation converte
o Result para Response; a fachada do módulo converte o Result para Snapshot.

## 6. Command, Query, Handler ou Service

Estado atual:

- não existem Commands;
- não existem Queries/Handlers de CQRS na Application;
- não existem Handlers;
- casos de uso são classes `*Service` com `ExecuteAsync`.

Portanto, não introduza um Handler isolado em apenas uma funcionalidade sem decisão arquitetural.

Consulta existente:

```csharp
public sealed class GetGameService(IGameRepository games)
{
    public async Task<GameResult?> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var game = await games.GetAsync(id, cancellationToken);
        return game is null
            ? null
            : GameApplicationMappings.ToResult(game);
    }
}
```

Alteração existente:

```csharp
public sealed class CreateGameService(
    IGameRepository games,
    ICatalogUnitOfWork unitOfWork,
    TimeProvider clock)
{
    public async Task<GameResult> ExecuteAsync(
        CreateGameInput input,
        CancellationToken cancellationToken)
    {
        var game = Game.Create(
            input.Title,
            input.Description,
            input.Category,
            input.BasePrice,
            clock.GetUtcNow());

        await games.AddAsync(game, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return GameApplicationMappings.ToResult(game);
    }
}
```

## 7. Implementar validação

Estado atual:

- `[ApiController]` executa validação/model binding do ASP.NET Core;
- alguns checks de entrada ficam no service;
- invariantes ficam no Domain;
- limites persistidos também são invariantes do Domain;
- não há classes `*Validator` nem pacote FluentValidation.

Para `Game`, título, descrição, categoria e preço são protegidos no Domain. A
Presentation antecipa limites simples por Data Annotations e pode retornar todos
os erros de `ModelState`. A Application valida somente regras próprias do caso
de uso, como a política de senha no cadastro; uma violação que alcançar o Domain
é traduzida para 422.

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
        .HasConversion(priceConverter)
        .HasPrecision(12, 2)
        .IsRequired();
});
```

Se a mudança não altera schema, nenhuma migration é necessária.

## 11. Criar migration

Migrations são geradas pelo EF Core no projeto central:

```text
src/Database/FiapCloudGames.Database.Migrations/Migrations/
```

Restaure a ferramenta local:

```powershell
dotnet tool restore
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
```

Depois de alterar o mapping de Catalog, gere a migration:

```powershell
dotnet tool run dotnet-ef migrations add AddGamePublisher `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --output-dir Migrations/Catalog
```

Revise os arquivos `Up`, `Down` e o snapshot gerados. Para aplicar todos os
contextos:

```powershell
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

Use a pasta `Migrations/Identity`, `Migrations/Promotions` ou
`Migrations/Library` quando a alteração pertencer a outro contexto. Veja
[EF Core Migrations](database-migrations.md).

## 12. Criar o Controller e a rota

Consulta:

```csharp
[AllowAnonymous]
[HttpGet("{id:guid}")]
public async Task<ActionResult<GameResponse>> Get(
    Guid id,
    [FromServices] GetGameService service,
    CancellationToken cancellationToken)
{
    var result = await service.ExecuteAsync(
        id,
        cancellationToken);

    return result is null
        ? NotFound()
        : Ok(result.ToResponse());
}
```

Alteração:

```csharp
[Authorize(Roles = "Administrator")]
[HttpPost]
public async Task<ActionResult<GameResponse>> Create(
    CreateGameRequest request,
    [FromServices] CreateGameService service,
    CancellationToken cancellationToken)
{
    var result = await service.ExecuteAsync(
        request.ToInput(),
        cancellationToken);

    var response = result.ToResponse();

    return CreatedAtAction(
        nameof(Get),
        new { id = response.Id },
        response);
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
| campo/formato inválido | 400 Problem Details |
| regra de negócio não processável | 422 Problem Details |
| conflito de estado/duplicidade | 409 Problem Details |
| sem token em rota admin | 401 |
| role incorreta | 403 |
| erro inesperado | 500 |

Não use 409 na documentação de uma action sem implementar exceção/mapeamento correspondente.

## 15. Logs

O padrão atual não injeta `ILogger` nos services/Controllers. O middleware
registra falhas funcionais em `Information` e falhas 500 em `Error`. A
configuração base eleva a categoria do middleware para `Warning`, portanto os
eventos funcionais ficam suprimidos por padrão.

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
    var createdAtUtc = new DateTimeOffset(
        2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    var exception = Assert.Throws<DomainRuleViolationException>(() =>
        Game.Create(
            "Cloud Quest",
            "Aventura",
            "RPG",
            -0.01m,
            createdAtUtc));

    Assert.Equal(
        "O preço base não pode ser negativo.",
        exception.Message);
}
```

O repositório não contém biblioteca de mocks. Services não possuem testes unitários atuais; ao adicioná-los, um fake manual de `IGameRepository` é compatível sem novo pacote.

## 17. Teste de integração

A infraestrutura existente usa:

```csharp
public sealed class FiapCloudGamesApiFactory
    : WebApplicationFactory<Program>
```

Ela cobre `/health`, validação MVC, respostas 401/404 e o middleware de exceções,
mas não substitui os `DbContext`. Uma chamada válida que alcance persistência
tentará usar a connection string de teste em `localhost`.

```text
TODO: definir infraestrutura de PostgreSQL real/Testcontainers e limpeza de dados antes de adicionar testes de endpoint com persistência.
```

Não documente um fluxo autenticado com persistência como executável até essa
infraestrutura existir.

## 18. OpenAPI

Controllers e seus contratos são descobertos pelo MVC/Application Parts. Depois
da alteração:

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
Invoke-WebRequest https://localhost:7080/swagger/v1/swagger.json
```

Procure a rota e confirme método, request, response e requisito de autorização.
O Swagger UI é exposto em `Development`.

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
