# Migrations de banco de dados

## Responsabilidade

O projeto `FiapCloudGames.Database.Migrations` é um executável separado. Ele usa
FluentMigrator para aplicar, em uma única sequência, as mudanças de todos os
schemas. A API não executa migrations durante a inicialização.

## Aplicar as migrations

Com um PostgreSQL acessível:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

No fluxo com containers:

```powershell
docker compose up --build migrations
```

O Compose espera que o banco fique saudável antes de iniciar o migrador. Quando
`Admin__Email` e `Admin__Password` são informados juntos, o processo também tenta
criar o administrador inicial. `Admin__Name` tem valor padrão no código.

## Criar uma migration

Crie uma classe no projeto de migrations e use um número único e crescente:

```csharp
[Migration(202607230001)]
public sealed class AddExampleColumn : Migration
{
    public override void Up()
    {
        Alter.Table("games").InSchema("catalog")
            .AddColumn("example").AsString(100).Nullable();
    }

    public override void Down()
    {
        Delete.Column("example")
            .FromTable("games")
            .InSchema("catalog");
    }
}
```

O exemplo é um modelo para mudanças futuras; essa coluna não existe no projeto.
Escolha o identificador conforme a convenção cronológica já usada e nunca
reutilize um número publicado.

## Checklist

- O `Up` deixa o banco compatível com o código novo.
- O `Down` desfaz apenas a mudança daquela migration.
- Nomes de schema, tabela, coluna, índice e constraint coincidem com os
  mapeamentos EF.
- Mudanças destrutivas têm estratégia de preservação ou migração dos dados.
- A ordem de implantação entre migrador e API está documentada.
- Testes de mapeamento e build passam.

## Rollback

As classes implementam `Down`, mas o executável atual chama apenas `MigrateUp`.
Não existe parâmetro ou comando suportado no repositório para rollback.

`TODO: implementar e documentar um comando de rollback controlado, incluindo
backup, autorização e limite de versão.`

Até isso existir, não improvise rollback em produção. Restaure um backup ou
aplique uma migration corretiva conforme o procedimento operacional aprovado.

## Testes

Os testes de integração atuais validam metadados dos mapeamentos EF e não sobem um
PostgreSQL real nem executam FluentMigrator.

`TODO: adicionar teste efêmero que aplique todas as migrations em PostgreSQL e
compare o schema com os quatro modelos EF.`

