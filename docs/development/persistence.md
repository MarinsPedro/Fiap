# Persistência

## Visão geral

O sistema usa Entity Framework Core com o provedor Npgsql. Existe uma única
instância PostgreSQL e cada módulo possui seu próprio `DbContext` e schema:

| Módulo | Contexto | Schema |
|---|---|---|
| Identity | `IdentityDbContext` | `identity` |
| Catalog | `CatalogDbContext` | `catalog` |
| Promotions | `PromotionsDbContext` | `promotions` |
| Library | `LibraryDbContext` | `library` |

As migrations não são geradas pelo EF Core. A criação e evolução física das
tabelas ficam no projeto FluentMigrator centralizado.

## Padrão de acesso

Cada módulo define interfaces de repositório na camada Domain e suas
implementações na Infrastructure. As abstrações de unidade de trabalho ficam em
Application. As operações de leitura que não precisam alterar entidades usam
consultas sem rastreamento quando implementado pelo repositório. Alterações são
persistidas pela unidade de trabalho do próprio módulo.

Um `SaveChanges` do EF Core é atômico para aquele contexto. Não existe transação
compartilhada entre os quatro contextos, outbox ou coordenador distribuído.

## Mapeamentos e integridade

Os mapeamentos ficam na camada Infrastructure de cada módulo. A migration inicial
também cria schemas, chaves, índices, chaves estrangeiras internas e constraints
de verificação. Não há chaves estrangeiras físicas entre módulos; referências
como `UserId` e `GameId` atravessam fronteiras apenas como identificadores.

Consulte:

- [Migrations](database-migrations.md)
- [Regras de negócio](business-rules.md)
- [Módulos](../architecture/modules.md)

## Concorrência e auditoria

O modelo atual não possui:

- token de concorrência otimista;
- histórico de alterações;
- colunas padronizadas de criação/alteração em todas as entidades;
- soft delete genérico;
- interceptadores de persistência;
- retry policy explicitamente configurada.

`TODO: definir requisitos de concorrência, auditoria e resiliência do banco antes
de operar com múltiplas réplicas da API.`

## Boas práticas ao alterar o modelo

1. Altere primeiro o domínio e o mapeamento EF do módulo.
2. Crie uma migration FluentMigrator equivalente, com `Up` e `Down`.
3. Preserve o schema do módulo e evite relacionamentos físicos entre schemas.
4. Atualize os testes de mapeamento e de domínio.
5. Documente incompatibilidades e ordem de implantação.
6. Execute `dotnet test FiapCloudGames.sln`.

Não use `dotnet ef database update`: não há migrations EF Core neste repositório.
