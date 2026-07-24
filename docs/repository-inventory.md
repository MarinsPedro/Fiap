# Inventário do repositório

Inventário realizado em 23 de julho de 2026 a partir dos arquivos da solution, projetos, código, testes, configurações e Docker.

## Solution

`FiapCloudGames.sln` contém 29 projetos:

| Grupo | Quantidade | Projetos |
|---|---:|---|
| API | 1 | `FiapCloudGames.Api` |
| Identity | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Catalog | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Library | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Promotions | 5 | Domain, Contracts, Application, Infrastructure, Presentation |
| Banco | 1 | `FiapCloudGames.Database.Migrations` |
| Testes unitários | 4 | um por módulo |
| Testes de integração | 2 | API e compatibilidade de mappings |
| Testes de arquitetura | 1 | regras com NetArchTest |

Não foi encontrado Shared Kernel, Building Blocks, frontend, worker, message consumer ou segunda API.

## Pontos de entrada

- API: `src/Api/FiapCloudGames.Api/Program.cs`;
- migrador: `src/Database/FiapCloudGames.Database.Migrations/Program.cs`;
- Docker: `docker-compose.yml` e dois Dockerfiles.

## Banco e persistência

- PostgreSQL 17-alpine no Compose;
- quatro `DbContext`: `IdentityDbContext`, `CatalogDbContext`, `LibraryDbContext` e `PromotionsDbContext`;
- schemas `identity`, `catalog`, `library` e `promotions`;
- seis tabelas de domínio;
- uma migration inicial `[Migration(202607220001)]`;
- seed opcional/idempotente de administrador via `AdminSeeder`;
- repositories internos e interfaces no Domain;
- Unit of Work representada pelo próprio `DbContext`.

## API e segurança

- Controllers carregados com MVC Application Parts;
- 14 endpoints de controllers e um health check;
- JWT HS256 com validade de duas horas;
- roles `User` e `Administrator`;
- PBKDF2-SHA256, 100.000 iterações, salt de 16 bytes e hash de 32 bytes;
- OpenAPI JSON somente em `Development`;
- CORS baseado em `Cors:AllowedOrigins`;
- middleware global para exceções e `ProblemDetails`.

## Testes encontrados

- oito casos unitários de Domain;
- um teste de host/health check com `WebApplicationFactory<Program>`;
- um teste de compatibilidade de schemas/tabelas EF sem conexão real;
- seis execuções de regras arquiteturais (quatro assemblies Domain e duas regras adicionais);
- total atual: 16 casos.

Não foram encontrados mocks, builders, snapshots, cobertura configurada, Testcontainers ou PostgreSQL real nos testes.

## Integrações

Integração externa confirmada: PostgreSQL por Npgsql.

As chamadas Identity/Catalog/Promotions feitas por Library são integrações internas e síncronas em memória por `Contracts`. Os tipos de eventos de integração estão declarados, mas não são publicados nem consumidos.

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
- eventos de integração são apenas contratos;
- não há concorrência otimista nem tratamento específico de violação de unicidade;
- erros de persistência inesperados resultam em HTTP 500;
- status 409 não é produzido pelo middleware atual;
- `Location` de criação de promoção aponta para uma rota sem GET correspondente;
- limites de `description` e `category` existem no mapping, mas não no Domain;
- não há transação única entre módulos nem outbox;
- testes de integração não validam migrations contra PostgreSQL real.
