# Testes

## Projetos

| Pasta | Projeto | Cobertura atual |
|---|---|---|
| `Unit/` | quatro projetos, um por módulo | 17 casos de domínio |
| `Integration/` | API | 18 casos de host, validação e tratamento de erros |
| `Integration/` | Database | 3 casos de mappings, snapshots e migrations EF |
| `Architecture/` | ArchitectureTests | fronteiras de dependência |

## Executar tudo

```powershell
dotnet test FiapCloudGames.sln
```

Após build:

```powershell
dotnet test FiapCloudGames.sln --no-build --no-restore
```

## Importante

Os testes de integração atuais não conectam ao PostgreSQL. Eles descobrem as
migrations na assembly central, mas não validam sua aplicação, constraints,
repositórios reais nem fluxos HTTP de negócio.

Consulte:

- [Estratégia](../docs/testing/overview.md)
- [Unitários](../docs/testing/unit-tests.md)
- [Integração](../docs/testing/integration-tests.md)
- [Arquitetura](../docs/testing/architecture-tests.md)
- [Dados](../docs/testing/test-data.md)
