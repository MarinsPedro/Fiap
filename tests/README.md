# Testes

## Projetos

| Pasta | Projeto | Cobertura atual |
|---|---|---|
| `Unit/` | quatro projetos por módulo | oito regras de domínio |
| `Integration/` | API | inicialização do host e `/health` |
| `Integration/` | Database | metadados de schema/tabela no modelo EF |
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

Os testes de integração atuais não conectam ao PostgreSQL. Eles não validam
migrations, constraints, repositórios reais nem fluxos HTTP de negócio.

Consulte:

- [Estratégia](../docs/testing/overview.md)
- [Unitários](../docs/testing/unit-tests.md)
- [Integração](../docs/testing/integration-tests.md)
- [Arquitetura](../docs/testing/architecture-tests.md)
- [Dados](../docs/testing/test-data.md)

