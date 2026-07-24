# Decisões confirmadas e riscos

## Decisões registradas

| Decisão | Evidência | ADR |
|---|---|---|
| monólito modular | solution e único host HTTP | [ADR-0001](../adr/ADR-0001-modular-monolith.md) |
| migrations centralizadas | projeto independente FluentMigrator | [ADR-0002](../adr/ADR-0002-centralized-migrations.md) |
| comunicação por Contracts | referências entre projetos | [ADR-0003](../adr/ADR-0003-module-communication.md) |
| Controllers por Application Parts | `Program.cs` | [ADR-0004](../adr/ADR-0004-application-parts.md) |
| autenticação em Identity | DI e infraestrutura JWT | [ADR-0005](../adr/ADR-0005-identity-authentication.md) |
| Problem Details global | middleware da API | [ADR-0006](../adr/ADR-0006-problem-details.md) |
| OpenAPI apenas em Development | condicional no `Program.cs` | [ADR-0007](../adr/ADR-0007-development-openapi.md) |

## Riscos observáveis

- eventos existem sem transporte, publisher, consumer ou outbox;
- verificações entre módulos possuem janela de concorrência;
- `DbUpdateException` e exceções Npgsql não recebem mapeamento específico;
- `/health` não verifica PostgreSQL;
- o entry point do migrador oferece somente `MigrateUp`;
- testes não validam SQL/migrations contra PostgreSQL real;
- listagens não paginam e rotas não possuem versão;
- `Location` da criação de promoção aponta para rota sem GET correspondente.

## TODOs de decisão

```text
TODO: confirmar requisitos de pagamento.
TODO: confirmar necessidade de mensageria e consistência eventual.
TODO: definir estratégia de concorrência e conflitos HTTP 409.
TODO: definir observabilidade e SLOs.
TODO: definir versionamento da API.
TODO: definir estratégia de rollback do banco.
```
