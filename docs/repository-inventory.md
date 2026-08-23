# Mapa do repositório

## Organização da solução

```text
src/
├── Api/             composição do host ASP.NET Core
├── BuildingBlocks/  contratos técnicos compartilhados
├── Database/        executável e assembly central de migrations
└── Modules/         módulos de negócio

tests/
├── Unit/            Domain e Application por módulo
├── Integration/     contratos transversais de API e metadados EF
└── Architecture/    fronteiras e convenções estruturais
```

Cada módulo segue a estrutura:

```text
Domain
Contracts
Application
Infrastructure
Presentation
```

Domain e Application concentram regras específicas. Infrastructure implementa
persistência e integrações técnicas. Presentation adapta os casos de uso para
HTTP. Contracts estabelece a comunicação permitida entre módulos.

## Pontos de entrada

- API: `src/Api/FiapCloudGames.Api/Program.cs`;
- migrador: `src/Database/FiapCloudGames.Database.Migrations/Program.cs`;
- ambiente local: `docker-compose.yml` e Dockerfiles da API/migrador.

## Banco e persistência

O PostgreSQL é separado por schemas de módulo. Cada Infrastructure possui seu
próprio `DbContext`, repositories internos e implementação de Unit of Work. As
migrations permanecem em uma assembly central, com histórico separado por
contexto no schema técnico.

O seed administrativo é opcional e idempotente. Os módulos não compartilham
entidades nem acessam diretamente o `DbContext` de outro módulo.

## API e segurança

Controllers são carregados por MVC Application Parts. O host configura JWT,
roles, CORS, OpenAPI, health check, autenticação, autorização e os middlewares
globais de logging e tratamento de erros.

O contrato de erro público é `ApiProblemDetails`. Falhas internas são
sanitizadas antes da resposta e recebem identificador de rastreamento.

## Comunicação entre módulos

Chamadas entre módulos são síncronas e passam por interfaces em Contracts.
Application não referencia a implementação de outro módulo. Não há eventos de
integração, outbox ou transação única atravessando módulos.

## Estratégia de testes

Testes de feature ficam em Domain/Application. Os demais projetos protegem
regras transversais:

- ArchitectureTests fiscaliza dependências e convenções;
- Api.IntegrationTests protege pipeline e contrato HTTP global;
- Database.IntegrationTests inspeciona metadados e migrations sem banco real.

Consulte a [estratégia de testes](testing/overview.md).

## Limitações arquiteturais registradas

- health check é somente liveness;
- não existe transação distribuída ou outbox entre módulos;
- persistência real não faz parte da estratégia transversal de testes;
- alterações na matriz de dependências devem ser registradas antes de mudar as
  regras arquiteturais.
