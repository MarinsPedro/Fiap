# EF Core Migrations

## Responsabilidade

`FiapCloudGames.Database.Migrations` é o executável central de evolução do banco.
Ele referencia os projetos Infrastructure dos quatro módulos e contém migrations
e snapshots separados para:

| Contexto | Pasta | Histórico |
|---|---|---|
| `IdentityDbContext` | `Migrations/Identity` | `infra.__EFMigrationsHistory_Identity` |
| `CatalogDbContext` | `Migrations/Catalog` | `infra.__EFMigrationsHistory_Catalog` |
| `PromotionsDbContext` | `Migrations/Promotions` | `infra.__EFMigrationsHistory_Promotions` |
| `LibraryDbContext` | `Migrations/Library` | `infra.__EFMigrationsHistory_Library` |

As tabelas de histórico ficam no schema técnico `infra`, separadas dos schemas
de domínio `identity`, `catalog`, `promotions` e `library`.

## Aplicar todas as migrations

Com PostgreSQL acessível:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

Antes de chamar o EF, o executável cria o schema técnico `infra`. Depois, chama
`Database.MigrateAsync()` sequencialmente para Identity, Catalog, Promotions e
Library e executa o seed opcional de administrador. A API não aplica migrations
no startup.

Com containers:

```powershell
docker compose up --build
```

## Ferramenta local

O repositório fixa `dotnet-ef` 10.0.10 em `.config/dotnet-tools.json`:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef --version
```

## Criar uma migration

1. Altere entidade e mapping no Infrastructure do módulo.
2. Escolha o `DbContext` correto.
3. Gere a migration no projeto central.
4. Revise `Up`, `Down` e o snapshot.

Exemplo para Catalog:

```powershell
dotnet tool run dotnet-ef migrations add AddGamePublisher `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --output-dir Migrations/Catalog
```

Pastas por contexto:

| Contexto | `--output-dir` |
|---|---|
| `IdentityDbContext` | `Migrations/Identity` |
| `CatalogDbContext` | `Migrations/Catalog` |
| `PromotionsDbContext` | `Migrations/Promotions` |
| `LibraryDbContext` | `Migrations/Library` |

Cada contexto possui uma factory própria:

- `Factories/IdentityDbContextFactory.cs`;
- `Factories/CatalogDbContextFactory.cs`;
- `Factories/PromotionsDbContextFactory.cs`;
- `Factories/LibraryDbContextFactory.cs`.

Todas usam `Configuration/DesignTimeConnectionString.Resolve()`. A variável
`ConnectionStrings__Database` é obrigatória inclusive para comandos de
design-time; sem ela o `dotnet-ef` encerra com
`InvalidOperationException`. Use uma conexão local/de desenvolvimento e nunca
uma credencial de produção para scaffolding.

## Inspecionar migrations

```powershell
dotnet tool run dotnet-ef migrations list `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj
```

Gere SQL para revisão sem aplicar:

```powershell
dotnet tool run dotnet-ef migrations script `
  --context CatalogDbContext `
  --idempotent `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj
```

## Aplicar ou voltar um contexto

O fluxo normal aplica todos os contextos pelo executável. Para manutenção
controlada de um contexto:

```powershell
$env:ConnectionStrings__Database = "<connection-string-do-ambiente>"
dotnet tool run dotnet-ef database update `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj
```

Para rollback, informe a migration de destino. `0` remove todas as migrations
daquele contexto e é destrutivo:

```powershell
dotnet tool run dotnet-ef database update 0 `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj
```

Faça backup e obtenha aprovação antes de rollback em ambiente compartilhado.

## Checklist

- Migration e snapshot pertencem ao contexto correto.
- `Up` e `Down` foram revisados.
- Schema, nomes, precisão, índices e FKs refletem o mapping.
- Mudanças destrutivas possuem estratégia de preservação dos dados.
- SQL idempotente foi revisado quando necessário.
- Build, testes de mapping e descoberta de migrations passam.
- A ordem entre migrador e API foi considerada.
- Nenhuma connection string real foi versionada.

## Testes

Os testes descobrem automaticamente os contexts e validam migrations,
snapshots, assembly, histórico e convenções de mapping por metadados, sem abrir
conexão.

PostgreSQL real não faz parte do padrão transversal. Adicionar aplicação ou
reversão de migrations em banco exige uma decisão explícita de estratégia,
isolamento e ciclo de vida para essa nova categoria de teste.
