# Visão geral da arquitetura

## Estilo

FIAP Cloud Games é um **monólito modular**. Há dois executáveis:

- `FiapCloudGames.Api`, processo HTTP permanente;
- `FiapCloudGames.Database.Migrations`, processo pontual que aplica migrations e encerra.

Os quatro domínios de negócio vivem no mesmo processo da API, mas têm assemblies, injeção de dependência, persistência e fronteiras públicas separados.

## Por que a divisão é comprovável

Cada módulo possui cinco projetos:

```text
FiapCloudGames.<Modulo>.Domain
FiapCloudGames.<Modulo>.Contracts
FiapCloudGames.<Modulo>.Application
FiapCloudGames.<Modulo>.Infrastructure
FiapCloudGames.<Modulo>.Presentation
```

`FiapCloudGames.Api` referencia Presentation e Infrastructure para compor o processo. Controllers de assemblies externos são descobertos por `AddApplicationPart`.

Dentro de Application e Presentation, o código é agrupado por feature; em
Contracts, cada contrato ocupa seu próprio arquivo. Services, Inputs, Results,
Requests, Responses, Queries, Snapshots e mappings não ficam reunidos em
arquivos monolíticos de casos de uso ou contratos.

## Responsabilidades

| Parte | Responsabilidade real |
|---|---|
| API | composition root, pipeline HTTP, CORS, OpenAPI, health check, autenticação/autorização |
| Presentation | Controllers, Requests, Responses, mappings HTTP, rotas, status e claims |
| Application | services, Inputs, Results e coordenação de repositories e módulos |
| Domain | entidades, value objects, invariantes e interfaces de repository |
| Contracts | Queries, Snapshots e fachadas públicas entre módulos |
| Infrastructure | EF Core, Npgsql, repositories, JWT, hash e registros de DI |
| Migrator | schema, tabelas, índices, constraints e seed do primeiro administrador |
| Tests | comportamento de Domain, host HTTP, mappings e dependências |

Há dois Building Blocks mínimos: `Domain.Common`, com
`DomainRuleViolationException`; e `Application.Common`, com `IUnitOfWork`,
`AppException` e `AppErrorCategory`. Eles não contêm marcador de agregado,
guard clauses ou entidades de negócio compartilhadas.

## Fronteiras

Identity e Catalog não dependem de outros módulos. Promotions consulta Catalog via `ICatalogModule`. Library consulta Identity, Catalog e Promotions via suas interfaces em `Contracts`.

A comunicação atual é inteiramente síncrona e em memória. Não existem eventos
de integração, publicação, persistência ou consumo.

## Dados

Todos os módulos usam uma única connection string `ConnectionStrings:Database` e um banco PostgreSQL físico. A separação lógica ocorre pelos schemas `identity`, `catalog`, `promotions` e `library`.

Não há foreign keys entre módulos. IDs de outro domínio são armazenados como identificadores simples.

## Regras que orientam alterações

- regra/invariante pertence ao Domain;
- orquestração pertence à Application;
- consulta EF e integração técnica pertencem à Infrastructure;
- rota e status HTTP pertencem à Presentation;
- entrada entre módulos é uma Query e a saída imutável é um Snapshot em Contracts;
- relógio entra por `TimeProvider` e o instante UTC é passado ao Domain;
- configuração do processo pertence à API;
- mudança estrutural pertence ao migrador central.

Veja [Modelo de domínio](domain-model.md),
[Objetos e contratos](data-contracts.md), [Camadas](layers.md),
[Dependências](dependencies.md) e
[Criar um endpoint](../development/creating-an-endpoint.md).

## Limitações atuais

- não há mensageria, outbox ou transação distribuída;
- cada `SaveChangesAsync` confirma somente um contexto;
- não há cache, retry, circuit breaker ou timeout de integração interna;
- não há abstração de usuário atual na Application; Presentation extrai a claim;
- não há API versioning.

Essas limitações devem ser consideradas antes de uma extração para microserviços.
