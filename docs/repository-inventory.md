# Inventário do repositório

Inventário atualizado em 10 de agosto de 2026 a partir dos arquivos da solution,
projetos, código, testes, configurações e Docker.

## Solution

`FiapCloudGames.sln` contém 31 projetos:

| Grupo | Quantidade | Projetos |
|---|---:|---|
| API | 1 | `FiapCloudGames.Api` |
| Building Blocks | 2 | `Domain.Common` e `Application.Common` |
| Identity | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Catalog | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Library | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Promotions | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Banco | 1 | `FiapCloudGames.Database.Migrations` |
| Testes unitários | 4 | um por módulo |
| Testes de integração | 2 | API e compatibilidade de mappings |
| Testes de arquitetura | 1 | regras com NetArchTest |

Não foi encontrado frontend, worker, message consumer ou segunda API. Os
Building Blocks não compartilham entidades de negócio.

## Pontos de entrada

- API: `src/Api/FiapCloudGames.Api/Program.cs`;
- migrador: `src/Database/FiapCloudGames.Database.Migrations/Program.cs`;
- Docker: `docker-compose.yml` e dois Dockerfiles.

## Banco e persistência

- PostgreSQL 17-alpine no Compose;
- quatro `DbContext`: `IdentityDbContext`, `CatalogDbContext`, `LibraryDbContext` e `PromotionsDbContext`;
- schemas `identity`, `catalog`, `library` e `promotions`;
- seis tabelas de domínio;
- quatro migrations EF Core iniciais e quatro snapshots, um conjunto por
  `DbContext`;
- quatro tabelas independentes de histórico EF;
- o migrador referencia os quatro projetos Infrastructure;
- seed opcional/idempotente de administrador via `AdminSeeder`;
- repositories internos e interfaces no Domain;
- Unit of Work representada pelo próprio `DbContext`.

## API e segurança

- Controllers carregados com MVC Application Parts;
- Application e Presentation organizadas por feature, com um tipo principal por
  arquivo para services, contratos e mappings;
- 14 endpoints de controllers e um health check;
- JWT HS256 com validade de duas horas;
- roles `User` e `Administrator`;
- PBKDF2-SHA256, 100.000 iterações, salt de 16 bytes e hash de 32 bytes;
- OpenAPI e Swagger UI somente em `Development`;
- CORS baseado em `Cors:AllowedOrigins`;
- middleware global para exceções e `ProblemDetails`.

## Testes encontrados

- 17 casos unitários de Domain;
- 18 casos de integração da API/middleware/host;
- três testes de banco sem conexão real: mappings, descoberta e sincronismo das migrations;
- 36 execuções de regras arquiteturais;
- total atual: 74 casos.

Não foram encontrados mocks, builders, snapshots, cobertura configurada, Testcontainers ou PostgreSQL real nos testes.

## Integrações

Integração externa confirmada: PostgreSQL por Npgsql.

As chamadas Identity/Catalog/Promotions feitas por Library são integrações
internas e síncronas em memória por `Contracts`. Não há eventos de integração.

## Documentação anterior consolidada

Os antigos documentos Architecture.md, Dependencies.md, Database.md e a pasta
Decisions foram substituídos pela árvore navegável atual para evitar duplicação.
O arquivo draw.io de Event Storming foi preservado em
`docs/EventStorming.drawio`.

## Lacunas confirmadas

```text
TODO: URL e política de acesso ao repositório não identificadas.
TODO: sistemas operacionais oficialmente suportados não identificados.
TODO: estratégia oficial de branches, commits, revisão e merge não identificada.
TODO: pipeline CI/CD não identificado.
TODO: registro de imagens e ambiente de deploy não identificados.
TODO: processo automatizado de rollback de migration não identificado.
TODO: estratégia de métricas, tracing, alertas e retenção de logs não identificada.
TODO: meta e ferramenta de cobertura de testes não identificadas.
TODO: política de versionamento e release não identificada.
```

## Riscos técnicos observáveis

- health check não consulta banco;
- não há concorrência otimista nem tratamento específico de violação de unicidade;
- erros de persistência inesperados resultam em HTTP 500;
- `Location` de criação de promoção aponta para uma rota sem GET correspondente;
- não há transação única entre módulos nem outbox;
- testes de integração não validam migrations contra PostgreSQL real.
