# Camadas e projetos

| Projeto/camada | Responsabilidade | Pode depender de | Não deve depender de |
|---|---|---|---|
| Domain | agregados, entidades, value objects, invariantes e contratos de repository | biblioteca padrão do .NET e `Domain.Common` | Contracts, Application, ASP.NET Core, EF Core, Infrastructure, Presentation |
| Contracts | Queries, Snapshots e fachadas públicas síncronas | biblioteca padrão do .NET | Domain, Application, EF Core, Controllers, implementações |
| Application | casos de uso, Inputs, Results e orquestração | Domain, Contracts, DI abstractions | Infrastructure, Presentation, HTTP, `DbContext` |
| Infrastructure | persistência e recursos técnicos | Application e Domain do próprio módulo | Presentation de qualquer módulo |
| Presentation | entrada HTTP | Application do próprio módulo e ASP.NET Core | Infrastructure e `DbContext` |
| API | composition root e pipeline | Presentation e Infrastructure | regra de negócio |
| Database.Migrations | EF migrations, snapshots e seed | Infrastructure dos módulos, EF Core, Npgsql, Hosting | Presentation dos módulos |
| Testes | validação automatizada | projetos sob teste e bibliotecas de teste | código de produção não depende de testes |

## Domain

Raízes usam construtores privados para materialização EF e expõem operações na
linguagem do negócio. Invariantes usam
métodos específicos da entidade ou do objeto de valor e lançam
`DomainRuleViolationException` com uma mensagem descritiva.

Exemplos: `Email.Create`, `Game.ChangeDetails`, `Game.Activate`,
`Promotion.ApplyTo`, `Promotion.End` e `GameLibrary.AcquireGame`.

## Contracts

As interfaces `IIdentityModule`, `ICatalogModule`, `IPromotionsModule` e
`ILibraryModule` são a fronteira síncrona. Recebem records `Query` e devolvem
records `Snapshot` imutáveis e mínimos. Eventos só serão adicionados quando
existir publicação e consumo reais.

## Application

O padrão de caso de uso é `*Service` com `ExecuteAsync`; não há MediatR.
Inputs e Results são records junto do caso de uso. Services não recebem Requests,
não devolvem Responses ou Snapshots e não conhecem HTTP. Portas de leitura,
como `ILibraryQueries`, evitam contaminar repositories de agregado com detalhes
de tracking. Services, Inputs, Results, mappings e fachadas ocupam arquivos
separados e são agrupados pela feature (`Games`, `Users`, `Authentication`,
`Pricing`, `Promotions` ou `UserLibrary`).

## Infrastructure

Cada módulo registra `DbContext`, repository interno e Unit of Work resolvida
pelo contexto. Objetos de valor usam `ValueConverter`. Consultas de leitura
usam `AsNoTracking`; Identity também implementa PBKDF2 e JWT.

## Presentation

Controllers recebem o service por `[FromServices]`, convertem request em input e
definem status. Requests, Responses, mappings HTTP e Controllers ficam em
`Features/<Feature>`. Regra de domínio não pertence ao Controller.

## API

`Program.cs` é o composition root. `public partial class Program;` permite que `WebApplicationFactory<Program>` encontre o entry point no teste.

## Onde criar cada mudança

| Mudança | Local |
|---|---|
| nova invariante | entidade/value object em Domain |
| novo caso de uso | Application |
| novo DTO entre módulos | Contracts do fornecedor |
| novo contrato HTTP | Request/Response e mapping em Presentation |
| nova entrada/saída de caso de uso | Input/Result em Application |
| nova consulta EF | repository em Infrastructure |
| nova rota | Controller em Presentation |
| novo mapping | `*DbContext` em Infrastructure |
| nova coluna/tabela | migration central |
| configuração global | API ou migrador |
| teste de regra | UnitTests do módulo |
| teste de host | Api.IntegrationTests |
| regra de dependência | ArchitectureTests |
