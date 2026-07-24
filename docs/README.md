# Documentação do FIAP Cloud Games

Este é o índice central. Os documentos descrevem somente o estado comprovado pelo repositório; recomendações e informações ausentes são identificadas explicitamente.

## Para quem acabou de chegar

- [Inventário do repositório](repository-inventory.md)
- [Primeiros passos](onboarding/getting-started.md)
- [Ambiente local](onboarding/local-environment.md)
- [Primeira tarefa](onboarding/first-task.md)
- [Checklist de onboarding](onboarding/onboarding-checklist.md)

## Para entender o sistema

- [Visão geral da arquitetura](architecture/overview.md)
- [Módulos](architecture/modules.md)
- [Camadas](architecture/layers.md)
- [Dependências](architecture/dependencies.md)
- [Fluxo de requisição](architecture/request-flow.md)
- [Decisões e riscos](architecture/decisions.md)
- [Diagramas](architecture/diagrams.md)
- [Índice de ADRs](adr/README.md)

## Para desenvolver funcionalidades

- [Criar um endpoint do zero](development/creating-an-endpoint.md)
- [Regras de negócio](development/business-rules.md)
- [Persistência](development/persistence.md)
- [Migrations](development/database-migrations.md)
- [Validação](development/validation.md)
- [Tratamento de erros](development/error-handling.md)
- [Autenticação e autorização](development/authentication-authorization.md)
- [Configuração](development/configuration.md)
- [Padrões de código](development/coding-standards.md)
- [Padrões de documentação](development/documentation-standards.md)

## Para testar

- [Visão geral dos testes](testing/overview.md)
- [Testes unitários](testing/unit-tests.md)
- [Testes de integração](testing/integration-tests.md)
- [Testes de arquitetura](testing/architecture-tests.md)
- [Dados de teste](testing/test-data.md)

## Para consumir a API

- [Visão geral](api/overview.md)
- [Convenções](api/conventions.md)
- [Autenticação](api/authentication.md)
- [Erros](api/errors.md)
- [Endpoints](api/endpoints.md)

## Para operar e publicar

- [Índice de operações](operations/README.md)
- [Docker](operations/docker.md)
- [Health checks](operations/health-checks.md)
- [Logs e monitoramento](operations/logging-monitoring.md)
- [CI/CD](operations/ci-cd.md)
- [Deploy](operations/deployment.md)
- [Troubleshooting](operations/troubleshooting.md)

## Documentação junto ao código

- [API](../src/Api/FiapCloudGames.Api/README.md)
- [Migrador](../src/Database/FiapCloudGames.Database.Migrations/README.md)
- [Identity](../src/Modules/Identity/README.md)
- [Catalog](../src/Modules/Catalog/README.md)
- [Library](../src/Modules/Library/README.md)
- [Promotions](../src/Modules/Promotions/README.md)
- [Testes](../tests/README.md)

## Governança

- [Como contribuir](../CONTRIBUTING.md)
- [Changelog](../CHANGELOG.md)
- [Definition of Done da documentação](development/documentation-standards.md#definition-of-done-da-documentação)

Ao encontrar divergência entre documentação e código, trate o código/configuração versionados como evidência do estado atual, abra uma correção documental e registre a decisão em ADR quando for arquitetural.
