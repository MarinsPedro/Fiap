# FiapCloudGames.Database.Migrations

## Propósito

Executável central de EF Core Migrations para os schemas `identity`, `catalog`,
`promotions` e `library`. O projeto referencia os quatro `Infrastructure` e usa
seus `DbContext` e mappings como fonte do modelo.

## Execução

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

O processo aplica migrations pendentes, em ordem:

1. `IdentityDbContext`;
2. `CatalogDbContext`;
3. `PromotionsDbContext`;
4. `LibraryDbContext`;
5. seed opcional do administrador.

Para o seed, informe `Admin__Email` e `Admin__Password`; `Admin__Name` é
opcional.

## Organização

- `Migrations/Identity`: migration e snapshot de Identity;
- `Migrations/Catalog`: migration e snapshot de Catalog;
- `Migrations/Promotions`: migration e snapshot de Promotions;
- `Migrations/Library`: migration e snapshot de Library;
- `Factories`: criação dos quatro contexts pelo `dotnet-ef`;
- `Configuration/DesignTimeConnectionString.cs`: leitura obrigatória de
  `ConnectionStrings__Database` em design-time;
- `Configuration/MigrationDbContextOptions.cs`: assembly, schema `infra` e
  tabelas de histórico;
- `Initialization/MigrationSchemaInitializer.cs`: cria `infra` antes de aplicar
  migrations;
- `Program.cs`: `Database.MigrateAsync()` e seed;
- `Seeding/AdminSeeder.cs`: administrador inicial idempotente.

Cada contexto usa sua própria tabela `infra.__EFMigrationsHistory_<Contexto>`.

## Gerar uma migration

```powershell
dotnet tool restore
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
dotnet tool run dotnet-ef migrations add AddGamePublisher `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --output-dir Migrations/Catalog
```

Troque contexto e pasta conforme o módulo. Não edite o snapshot manualmente.
As factories não possuem connection string fictícia de fallback; o comando
falha se `ConnectionStrings__Database` estiver ausente.

## Segurança

O `appsettings.json` não contém connection string. Use variável de ambiente,
User Secrets ou secret store:

```powershell
dotnet user-secrets set `
  --project src/Database/FiapCloudGames.Database.Migrations `
  "ConnectionStrings:Database" `
  "<connection-string-local>"
```

Uma credencial anteriormente exposta em arquivo versionado deve ser rotacionada
no provedor correspondente.

## Testes e limitações

- os testes validam mapping e descoberta das quatro migrations;
- o repositório ainda não aplica migrations em PostgreSQL efêmero durante testes;
- o executável aplica somente migrations pendentes;
- rollback é feito explicitamente com `dotnet-ef database update` por contexto.

Guia completo: [EF Core Migrations](../../../docs/development/database-migrations.md).
