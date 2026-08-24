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
Library. A atualização de Identity aciona o seed opcional de administrador via
`UseAsyncSeeding`. A API não aplica migrations no startup.

## Ferramenta local

O repositório fixa `dotnet-ef` 10.0.10 em `.config/dotnet-tools.json`:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef --version
```

Depois do restore, os exemplos abaixo podem usar diretamente `dotnet ef`.

## Formas de criar e aplicar migrations

Existem duas formas de criar migrations:

1. Package Manager Console: `Add-Migration`;
2. .NET CLI: `dotnet ef migrations add`.

Existem três formas de aplicar migrations:

1. Package Manager Console: `Update-Database`;
2. .NET CLI: `dotnet ef database update`;
3. executável central:
   `dotnet run --project src/Database/FiapCloudGames.Database.Migrations`.

| Entrada | Cria migration | Aplica um contexto | Aplica todos | Executa `AdminSeeder` |
|---|---:|---:|---:|---:|
| Package Manager Console | sim | sim | não | ao atualizar Identity |
| .NET CLI | sim | sim | não | ao atualizar Identity |
| Executável central | não | não | sim | sim |

O executável central aplica todas as migrations existentes na ordem Identity,
Catalog, Promotions e Library. Ele não cria migrations novas.

## Package Manager Console

O projeto `FiapCloudGames.Database.Migrations` também referencia
`Microsoft.EntityFrameworkCore.Tools` 10.0.10. No Package Manager Console do
Visual Studio, selecione esse projeto como **Default project** e use o mesmo
projeto como startup project da solução.

Por padrão, o comando lê `ConnectionStrings:Database` do `appsettings.json`
copiado para o diretório de saída. Para substituir a conexão sem alterar o
arquivo, defina a variável de ambiente antes de executar o comando:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
```

Para criar uma migration de Catalog:

```powershell
Add-Migration AddGamePublisher `
  -Context CatalogDbContext `
  -Project FiapCloudGames.Database.Migrations `
  -StartupProject FiapCloudGames.Database.Migrations `
  -OutputDir Migrations/Catalog
```

Para aplicar as migrations de Catalog:

```powershell
Update-Database `
  -Context CatalogDbContext `
  -Project FiapCloudGames.Database.Migrations `
  -StartupProject FiapCloudGames.Database.Migrations
```

Execute o comando separadamente para `IdentityDbContext`, `CatalogDbContext`,
`PromotionsDbContext` e `LibraryDbContext`. O fluxo preferencial para aplicar
os quatro contextos de uma vez continua sendo o executável central descrito
acima. Somente `Update-Database -Context IdentityDbContext` aciona o seed de
administrador, via `UseSeeding`, inclusive quando não há migration pendente.

## .NET CLI

Antes dos comandos, restaure a ferramenta local e configure a conexão:

```powershell
dotnet tool restore
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
```

Para criar uma migration de Catalog:

```powershell
dotnet ef migrations add AddGamePublisher `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --output-dir Migrations/Catalog
```

Para aplicar as migrations de Catalog:

```powershell
dotnet ef database update `
  --context CatalogDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj
```

Ao criar uma migration:

1. altere a entidade e o mapping no Infrastructure do módulo;
2. escolha o `DbContext` correto;
3. gere a migration no projeto central;
4. revise `Up`, `Down` e o snapshot.

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

Todas usam `Configuration/DesignTimeConnectionString.Resolve()`. A resolução
carrega primeiro o `appsettings.json` do diretório de saída e depois as
variáveis de ambiente. Assim, `ConnectionStrings__Database` substitui o valor
do arquivo quando estiver definida. Use uma conexão local/de desenvolvimento e
nunca uma credencial de produção para scaffolding.

## Seed de administrador

O `AdminSeeder` é registrado somente nas opções do `IdentityDbContext`:

- `UseSeeding` atende `Update-Database` e `dotnet ef database update`;
- `UseAsyncSeeding` atende o `Database.MigrateAsync()` do executável central;
- Catalog, Promotions e Library não executam esse seed.

As três entradas usam a mesma implementação. Se `Admin:Email` e
`Admin:Password` forem omitidos, o seed não acessa o banco. Se apenas um deles
for informado, a operação falha por configuração inválida. O insert usa
`ON CONFLICT (email) DO NOTHING`, tornando novas execuções idempotentes.

Configure o administrador preferencialmente por variáveis de ambiente:

```powershell
$env:Admin__Name = "Administrador local"
$env:Admin__Email = "admin@example.com"
$env:Admin__Password = "change-me-now-1!"
```

Package Manager Console:

```powershell
Update-Database `
  -Context IdentityDbContext `
  -Project FiapCloudGames.Database.Migrations `
  -StartupProject FiapCloudGames.Database.Migrations
```

.NET CLI:

```powershell
dotnet ef database update `
  --context IdentityDbContext `
  --project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  --startup-project src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj
```

Executável central:

```powershell
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

`Update-Database` e `dotnet ef database update` executam o delegate síncrono
`UseSeeding`. O executável central usa `Database.MigrateAsync()` e executa
`UseAsyncSeeding`. Nos três casos, o seed acontece somente durante a operação
do `IdentityDbContext`.

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
