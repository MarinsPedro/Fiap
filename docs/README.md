# Documentação do FIAP Cloud Games

Esta documentação registra decisões de desenho, contratos, regras e formas de
trabalhar no projeto. Ela não mantém manualmente inventários que o código e as
ferramentas já conseguem produzir.

## Fontes de verdade dinâmicas

| Assunto | Fonte de verdade |
|---|---|
| API HTTP | OpenAPI em `/swagger/v1/swagger.json` durante Development |
| SDK | política em `global.json`; seleção local em `dotnet --version` |
| dependências | `Directory.Packages.props` e `*.csproj` |
| projetos | `dotnet sln FiapCloudGames.sln list` |
| testes | `dotnet test FiapCloudGames.sln` |
| migrations | assembly e diretórios do projeto de migrations |

Os documentos manuais explicam como interpretar e preservar essas fontes, sem
copiar suas listas completas.

## Onboarding

- [Primeiros passos](onboarding/getting-started.md)
- [Ambiente local](onboarding/local-environment.md)
- [Primeira contribuição](onboarding/first-task.md)
- [Checklist de onboarding](onboarding/onboarding-checklist.md)

## Arquitetura

- [Visão geral](architecture/overview.md)
- [Módulos](architecture/modules.md)
- [Camadas](architecture/layers.md)
- [Dependências](architecture/dependencies.md)
- [Comunicação e contratos](architecture/data-contracts.md)
- [Modelo de domínio](architecture/domain-model.md)
- [Fluxo de requisição](architecture/request-flow.md)
- [Restrições e riscos](architecture/decisions.md)
- [Diagramas](architecture/diagrams.md)

## Desenvolvimento

- [Criar uma feature HTTP](development/creating-an-endpoint.md)
- [Regras de negócio](development/business-rules.md)
- [Persistência](development/persistence.md)
- [Migrations](development/database-migrations.md)
- [Validação](development/validation.md)
- [Tratamento de erros](development/error-handling.md)
- [Autenticação e autorização](development/authentication-authorization.md)
- [Configuração](development/configuration.md)
- [Padrões de código](development/coding-standards.md)
- [Padrões de documentação](development/documentation-standards.md)

## API

- [Visão geral e OpenAPI](api/overview.md)
- [Convenções](api/conventions.md)
- [Autenticação](api/authentication.md)
- [Erros](api/errors.md)

## Testes

- [Estratégia](testing/overview.md)
- [Testes unitários](testing/unit-tests.md)
- [Testes transversais](testing/transversal-tests.md)
- [Integração](testing/integration-tests.md)
- [Arquitetura](testing/architecture-tests.md)
- [Dados de teste](testing/test-data.md)

## Operações

- [Índice de operações](operations/README.md)
- [Docker](operations/docker.md)
- [Health checks](operations/health-checks.md)
- [Logging e monitoramento](operations/logging-monitoring.md)
- [CI/CD](operations/ci-cd.md)
- [Deployment](operations/deployment.md)
- [Troubleshooting](operations/troubleshooting.md)

## Governança

- [Como contribuir](../CONTRIBUTING.md)
- [Changelog](../CHANGELOG.md)
- [Pendências consolidadas](backlog.md)
- [Padrões de documentação](development/documentation-standards.md)

Ao encontrar divergência entre texto e uma fonte dinâmica, corrija o documento
que explica a regra, mas não crie outro inventário manual do estado do código.
