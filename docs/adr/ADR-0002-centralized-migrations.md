# ADR-0002: Migrations centralizadas

## Status

Substituído por
[ADR-0008](ADR-0008-ef-core-centralized-migrations.md).

## Contexto

Quatro contexts usam um banco físico. A estrutura precisa evoluir em ordem única sem permitir que a API aplique migrations no startup.

## Decisão

Usar `FiapCloudGames.Database.Migrations`, console FluentMigrator sem referência aos módulos. O processo executa `MigrateUp()` e depois o seed opcional de administrador.

## Consequências

- release deve executar migrador antes da API;
- API não precisa de permissão para alterar schema;
- mappings EF e migration precisam ser mantidos manualmente em sincronia;
- não há comando de rollback exposto pelo entry point atual.

## Alternativas consideradas

EF Core Migrations não era usado nesta decisão original. A substituição e sua
motivação estão registradas no ADR-0008.
