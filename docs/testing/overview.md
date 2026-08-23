# Estratégia de testes

## Objetivo

A suíte separa regras específicas de negócio de proteções transversais. Essa
divisão reduz acoplamento, mantém os testes rápidos e permite evoluir uma feature
sem alterar projetos que protegem apenas contratos globais.

```text
Feature
  |
  +-- Domain Tests              invariantes e comportamento do domínio
  +-- Application Tests         decisões e orquestração do caso de uso
  +-- ArchitectureTests         fronteiras e dependências globais
  +-- Api.IntegrationTests      contrato HTTP transversal
  +-- Database.IntegrationTests metadados e convenções do EF Core
```

## Distribuição de responsabilidades

| Projeto | Responsabilidade principal | Nova feature altera? |
|---|---|---|
| `*.UnitTests` | Domain e Application | Normalmente |
| `ArchitectureTests` | Regras estruturais | Somente se a arquitetura mudar |
| `Api.IntegrationTests` | Pipeline e contrato HTTP global | Somente se o contrato transversal mudar |
| `Database.IntegrationTests` | Mappings, migrations e convenções | Somente se a convenção de persistência mudar |

## Princípios

- Testar comportamento observável e decisões relevantes.
- Preferir descoberta automática a listas de módulos, entidades ou endpoints.
- Não duplicar na API uma regra já protegida em Domain ou Application.
- Não acoplar Application a EF Core, HTTP ou repositories concretos.
- Alterações internas que preservam contrato não devem exigir mudanças nos testes.
- Testes transversais devem proteger várias features ao mesmo tempo.
- Quantidade de testes não é meta arquitetural nem faz parte da documentação.

## Quando criar um teste transversal

Crie ou altere um teste transversal quando houver nova regra arquitetural,
convenção global, comportamento compartilhado do pipeline, contrato HTTP ou
estratégia de mapping/migrations.

Uma nova feature que siga as convenções existentes deve ser coberta
automaticamente pelos testes transversais e receber seus cenários específicos em
Domain e Application.

## Guias relacionados

- [Testes unitários](unit-tests.md)
- [Testes transversais](transversal-tests.md)
- [Integração transversal](integration-tests.md)
- [Testes de arquitetura](architecture-tests.md)
- [Dados de teste](test-data.md)
