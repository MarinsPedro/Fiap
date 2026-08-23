# FiapCloudGames.Database.Migrations

## Propósito

Executável separado para evolução do banco por EF Core Migrations. Ele reutiliza
os `DbContext` e mappings de Infrastructure e mantém a API sem permissão para
alterar schema durante o startup.

## Regras

- cada contexto possui migrations, snapshot e histórico próprios;
- tabelas de histórico ficam no schema técnico `infra`;
- schemas de negócio continuam pertencendo aos módulos;
- o schema técnico é criado antes da aplicação das migrations;
- migrations são aplicadas sequencialmente;
- o seed administrativo é opcional e ocorre depois das migrations;
- design-time exige `ConnectionStrings__Database`;
- rollback é explícito e executado por contexto.

A lista atual de migrations deve ser consultada no projeto ou com
`dotnet-ef migrations list`, não duplicada neste README.

## Executar

```powershell
$env:ConnectionStrings__Database = "<connection-string-local>"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

Para habilitar o seed, forneça `Admin__Email` e `Admin__Password` em conjunto.
`Admin__Name` é opcional.

## Gerar e revisar

Use a ferramenta `dotnet-ef` fixada pelo repositório. Escolha explicitamente o
`DbContext`, o projeto de migrations e a pasta correspondente. Revise sempre
`Up`, `Down` e o snapshot gerado.

Os comandos completos e a política de rollback estão em
[EF Core Migrations](../../../docs/development/database-migrations.md).

## Segurança e validação

Não versione connection strings. Use variável de ambiente, User Secrets local ou
secret store do ambiente.

Os testes atuais validam modelos e migrations por metadados, sem abrir
PostgreSQL. A adoção de banco real nos testes depende de
[DOC-009](../../../docs/backlog.md).
