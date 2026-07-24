# Camadas e projetos

| Projeto/camada | Responsabilidade | Pode depender de | Não deve depender de |
|---|---|---|---|
| Domain | entidades, value objects, invariantes e contratos de repository | biblioteca padrão do .NET | ASP.NET Core, EF Core, Infrastructure, Presentation |
| Contracts | DTOs, fachadas públicas e records de eventos | biblioteca padrão do .NET | Domain, EF Core, Controllers, implementações |
| Application | casos de uso e orquestração | Domain, Contracts, DI abstractions | Infrastructure, Presentation, HTTP, `DbContext` |
| Infrastructure | persistência e recursos técnicos | Application e Domain do próprio módulo | Presentation de qualquer módulo |
| Presentation | entrada HTTP | Application do próprio módulo e ASP.NET Core | Infrastructure e `DbContext` |
| API | composition root e pipeline | Presentation e Infrastructure | regra de negócio |
| Database.Migrations | estrutura física e seed | FluentMigrator, Npgsql, Hosting | qualquer projeto de módulo |
| Testes | validação automatizada | projetos sob teste e bibliotecas de teste | código de produção não depende de testes |

## Domain

Entidades usam construtores privados para materialização EF e métodos públicos para mudança de estado. Regras são expressas por `ArgumentException` ou `InvalidOperationException`; não existe hierarquia própria de exceções de domínio.

Exemplos: `Email.Create`, `Game.Update`, `Promotion.Create`, `Promotion.ApplyTo`, `Promotion.End` e `GameLibrary.AddGame`.

## Contracts

As interfaces `IIdentityModule`, `ICatalogModule`, `IPromotionsModule` e `ILibraryModule` são a fronteira síncrona. DTOs são records imutáveis.

Os records `IntegrationEvent` não formam uma integração funcional sozinhos. Falta infraestrutura de publicação/consumo.

## Application

O padrão atual é `*Service` com `ExecuteAsync`; não há Commands, Queries, Handlers ou MediatR. Inputs/outputs são records junto do caso de uso.

## Infrastructure

Cada módulo registra `DbContext`, repository interno e Unit of Work resolvida pelo contexto. Identity também implementa PBKDF2 e JWT.

## Presentation

Controllers recebem o service por `[FromServices]`, convertem request em input e definem status. Regra de domínio não pertence ao Controller.

## API

`Program.cs` é o composition root. `public partial class Program;` permite que `WebApplicationFactory<Program>` encontre o entry point no teste.

## Onde criar cada mudança

| Mudança | Local |
|---|---|
| nova invariante | entidade/value object em Domain |
| novo caso de uso | Application |
| novo DTO entre módulos | Contracts do fornecedor |
| nova consulta EF | repository em Infrastructure |
| nova rota | Controller em Presentation |
| novo mapping | `*DbContext` em Infrastructure |
| nova coluna/tabela | migration central |
| configuração global | API ou migrador |
| teste de regra | UnitTests do módulo |
| teste de host | Api.IntegrationTests |
| regra de dependência | ArchitectureTests |
