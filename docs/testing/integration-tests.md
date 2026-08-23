# Testes de integração transversal

## API

`FiapCloudGames.Api.IntegrationTests` protege o contrato HTTP global e é
organizado conceitualmente em:

```text
Host/        WebApplicationFactory, HttpClient e pipeline ASP.NET Core
Components/  middlewares, factories e extensões transversais
Contracts/   OpenAPI e convenções declaradas pelos controllers
Support/     infraestrutura exclusiva da suíte
```

### Responsabilidades

- serialização de `ApiProblemDetails`;
- validação MVC e JSON inválido;
- mapeamento global de erros;
- respostas de autenticação e autorização;
- sanitização de falhas inesperadas;
- logging transversal e rastreabilidade;
- health check de liveness;
- contrato OpenAPI;
- declaração consistente de respostas pelos controllers.

O mapeamento de Validation, Authentication, Forbidden, NotFound, Conflict,
BusinessRule e falhas inesperadas é protegido uma vez. Não deve ser repetido em
cada endpoint.

### Contratos genéricos

O documento OpenAPI é percorrido para assegurar que respostas de erro usam
`application/problem+json` e o schema fechado de `ApiProblemDetails`. As actions
dos controllers também são descobertas automaticamente para verificar respostas
transversais, como erro interno, autenticação e autorização administrativa.

### Testes específicos de endpoint

Um endpoint só recebe teste próprio quando introduz comportamento HTTP novo,
como binding especial, serialização diferente, upload, download, streaming,
webhook ou autorização excepcional. Regras de negócio comuns continuam em
Domain/Application.

## Database

`FiapCloudGames.Database.IntegrationTests` utiliza o provedor Npgsql somente
para construir o modelo e inspecionar metadados do EF Core. Nenhuma conexão é
aberta.

Os `DbContext` são descobertos automaticamente pelas assemblies de
Infrastructure. A suíte protege:

- descoberta das migrations centralizadas;
- compatibilidade entre modelo e snapshot;
- assembly de migrations;
- schema e tabela de histórico;
- propriedade do schema por módulo;
- nomes de tabelas e colunas em `lower_snake_case`;
- presença de chaves primárias;
- precisão explícita para valores armazenados como decimal;
- ausência de entidades de outro módulo no context.

### Fora do escopo

Este projeto não valida repositories, CRUD, SQL, constraints em runtime,
transações, concorrência, performance ou PostgreSQL real. Também não utiliza
container de banco.

Esse trade-off é intencional: a estratégia prioriza baixo custo de manutenção,
regras de negócio em Domain/Application e proteção estrutural por metadados. Se
o risco de persistência mudar, a decisão deve ser revista explicitamente e não
introduzida como consequência automática de uma feature.
