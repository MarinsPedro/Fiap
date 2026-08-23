# Testes

## Arquitetura da suíte

| Categoria | Responsabilidade |
|---|---|
| `Unit/` | Regras de Domain e decisões de Application de cada módulo |
| `Integration/FiapCloudGames.Api.IntegrationTests` | Contratos HTTP e componentes transversais da API |
| `Integration/FiapCloudGames.Database.IntegrationTests` | Metadados, mappings, migrations e convenções do EF Core |
| `Architecture/` | Fronteiras, dependências e convenções estruturais |

## Regra central

Testes específicos de uma feature devem se concentrar em Domain e Application.
Os projetos transversais protegem regras globais de forma genérica e não devem
ser alterados para cada nova entidade, endpoint, repository ou migration que já
siga as convenções existentes.

## Executar

```powershell
dotnet test FiapCloudGames.sln
```

Após restore e build:

```powershell
dotnet test FiapCloudGames.sln --no-build --no-restore
```

## Guias

- [Estratégia](../docs/testing/overview.md)
- [Testes unitários](../docs/testing/unit-tests.md)
- [Testes transversais](../docs/testing/transversal-tests.md)
- [Integração transversal](../docs/testing/integration-tests.md)
- [Arquitetura](../docs/testing/architecture-tests.md)
- [Dados de teste](../docs/testing/test-data.md)
