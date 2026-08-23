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

As migrations são geradas pelo EF Core a partir desses mappings e ficam no
projeto central `FiapCloudGames.Database.Migrations`.

## Padrão de acesso

Cada módulo define interfaces de repositório na camada Domain e suas
implementações na Infrastructure. As abstrações de unidade de trabalho ficam em
Application. As operações de leitura que não precisam alterar entidades usam
consultas sem rastreamento. Em Library, `ILibraryQueries` devolve
`LibraryGameReadModel` a partir de `AsNoTracking`; nos repositories, listagens
como `GameRepository.ListAsync` também desabilitam tracking. Alterações são
persistidas pela unidade de trabalho do próprio módulo.

Um `SaveChanges` do EF Core é atômico para aquele contexto. Não existe transação
compartilhada entre os quatro contextos, outbox ou coordenador distribuído.

## Mapeamentos e integridade

Os mapeamentos ficam na camada Infrastructure de cada módulo. As migrations
iniciais criam schemas, chaves, índices e chaves estrangeiras internas. Não há
chaves estrangeiras físicas entre módulos; referências
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

## Boas práticas ao alterar o modelo

1. Altere primeiro o domínio e o mapeamento EF do módulo.
2. Gere uma migration EF Core no contexto e pasta correspondentes.
3. Preserve o schema do módulo e evite relacionamentos físicos entre schemas.
4. Atualize os testes de mapeamento e de domínio.
5. Documente incompatibilidades e ordem de implantação.
6. Execute `dotnet test FiapCloudGames.sln`.

Use o executável central para aplicar todos os contextos. Reserve
`dotnet tool run dotnet-ef database update` para manutenção explícita de um
contexto.
