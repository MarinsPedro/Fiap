# Estratégia de testes

## Estado atual

A solução possui sete projetos de teste e 74 casos executados:

| Categoria | Projetos | Casos atuais | Escopo |
|---|---:|---:|---|
| Unitário | 4 | 17 | agregados, invariantes e objetos de valor |
| Integração | 2 | 21 | host, validação/erros HTTP, metadados EF e migrations |
| Arquitetura | 1 | 36 execuções | dependências, domínio e contratos por fronteira |

Casos parametrizados são contabilizados por assembly.

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

O repositório tem uma base inicial de testes unitários e cobre o pipeline HTTP
para validação, challenge 401, 404 e tratamento de exceções. Ainda não cobre
services de Application, autenticação com token válido, persistência real ou
fluxos de negócio entre módulos. Os testes de integração atuais não abrem
conexão com o PostgreSQL.

`TODO: definir metas de cobertura e ampliar a suíte com testes de serviço, HTTP
autenticado e PostgreSQL efêmero.`

## Guias

- [Testes unitários](unit-tests.md)
- [Testes de integração](integration-tests.md)
- [Testes de arquitetura](architecture-tests.md)
- [Dados de teste](test-data.md)
