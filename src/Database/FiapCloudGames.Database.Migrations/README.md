# FiapCloudGames.Database.Migrations

## Propósito

Executável FluentMigrator responsável por criar e evoluir os schemas `identity`,
`catalog`, `promotions` e `library`. Ele é independente dos assemblies dos
módulos, uma fronteira verificada pelos testes de arquitetura.

## Execução

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

Para criar o administrador inicial, informe também `Admin__Email` e
`Admin__Password`; `Admin__Name` é opcional.

No Compose, o migrador roda depois do health check do PostgreSQL e antes da API:

```powershell
docker compose up --build
```

## Organização

- `Migrations/`: classes FluentMigrator com `Up` e `Down`;
- `Seeding/`: criação opcional do administrador;
- `Program.cs`: configuração, `MigrateUp` e seed.

## Segurança

Connection string, senha do administrador e qualquer outro segredo devem vir do
ambiente/secret store. Não os registre nem os versione.

## Limitações

- o entrypoint não expõe rollback;
- a suíte não aplica migrations em um PostgreSQL real;
- não há lock operacional, backup ou pipeline de implantação;
- `TODO: adicionar validação de schema real e rollback controlado.`

Guia completo: [migrations](../../../docs/development/database-migrations.md).

