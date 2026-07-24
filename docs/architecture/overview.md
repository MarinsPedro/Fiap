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

## Responsabilidades

| Parte | Responsabilidade real |
|---|---|
| API | composition root, pipeline HTTP, CORS, OpenAPI, health check, autenticação/autorização |
| Presentation | Controllers, rotas, request records, status HTTP e leitura de claims |
| Application | serviços de caso de uso, inputs/outputs, coordenação de repositories e módulos |
| Domain | entidades, value objects, invariantes e interfaces de repository |
| Contracts | DTOs, fachadas públicas e tipos de eventos de integração |
| Infrastructure | EF Core, Npgsql, repositories, JWT, hash e registros de DI |
| Migrator | schema, tabelas, índices, constraints e seed do primeiro administrador |
| Tests | comportamento de Domain, host HTTP, mappings e dependências |

Não há Shared Kernel ou Building Blocks.

## Fronteiras

Identity e Catalog não dependem de outros módulos. Promotions consulta Catalog via `ICatalogModule`. Library consulta Identity, Catalog e Promotions via suas interfaces em `Contracts`.

Os records de eventos de integração existem, porém nenhum serviço os instancia, publica, persiste ou consome. Portanto, o estado atual da comunicação é inteiramente síncrono e em memória.

## Dados

Todos os módulos usam uma única connection string `ConnectionStrings:Database` e um banco PostgreSQL físico. A separação lógica ocorre pelos schemas `identity`, `catalog`, `promotions` e `library`.

Não há foreign keys entre módulos. IDs de outro domínio são armazenados como identificadores simples.

## Regras que orientam alterações

- regra/invariante pertence ao Domain;
- orquestração pertence à Application;
- consulta EF e integração técnica pertencem à Infrastructure;
- rota e status HTTP pertencem à Presentation;
- DTO usado por outro módulo pertence a Contracts;
- configuração do processo pertence à API;
- mudança estrutural pertence ao migrador central.

Veja [Camadas](layers.md), [Dependências](dependencies.md) e [Criar um endpoint](../development/creating-an-endpoint.md).

## Limitações atuais

- não há mensageria, outbox ou transação distribuída;
- cada `SaveChangesAsync` confirma somente um contexto;
- não há cache, retry, circuit breaker ou timeout de integração interna;
- tempo é obtido diretamente por `DateTimeOffset.UtcNow`;
- não há abstração de usuário atual na Application; Presentation extrai a claim;
- não há API versioning.

Essas limitações devem ser consideradas antes de uma extração para microserviços.
