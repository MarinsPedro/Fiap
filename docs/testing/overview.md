# Estratégia de testes

## Estado atual

A solução possui sete projetos de teste e 16 casos executados:

| Categoria | Projetos | Casos atuais | Escopo |
|---|---:|---:|---|
| Unitário | 4 | 8 | regras das entidades e value objects |
| Integração | 2 | 2 | host da API e metadados EF Core |
| Arquitetura | 1 | 6 execuções | dependências entre camadas/módulos |

Os quatro casos parametrizados de domínio em
`DomainShouldNotReferenceFrameworkOrInfrastructure` contam como quatro execuções.

## Executar

```powershell
dotnet test FiapCloudGames.sln
```

Quando restore e build já foram concluídos:

```powershell
dotnet test FiapCloudGames.sln --no-build --no-restore
```

Um projeto isolado:

```powershell
dotnet test tests/Unit/FiapCloudGames.Catalog.UnitTests
```

Um caso pelo nome:

```powershell
dotnet test FiapCloudGames.sln --filter "FullyQualifiedName~HealthEndpointShouldReturnSuccess"
```

## Pirâmide pretendida

O repositório tem uma base inicial de testes unitários, mas ainda não cobre
serviços de aplicação, controllers, autenticação, persistência real ou fluxos
entre módulos. Os chamados testes de integração atuais não abrem conexão com o
PostgreSQL.

`TODO: definir metas de cobertura e ampliar a suíte com testes de serviço, HTTP
autenticado e PostgreSQL efêmero.`

## Guias

- [Testes unitários](unit-tests.md)
- [Testes de integração](integration-tests.md)
- [Testes de arquitetura](architecture-tests.md)
- [Dados de teste](test-data.md)

