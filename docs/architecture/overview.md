# Visão geral da arquitetura

## Estilo

FIAP Cloud Games é um monólito modular. A API hospeda os módulos de negócio no
mesmo processo, enquanto o migrador é um processo separado e pontual para
evolução do banco.

A escolha reduz complexidade operacional sem abrir mão de fronteiras explícitas.
Ela não significa que os módulos possam compartilhar entidades, acessar
diretamente a persistência uns dos outros ou criar dependências arbitrárias.

## Fronteiras de módulo

Cada módulo organiza suas responsabilidades em:

```text
Domain
Application
Contracts
Infrastructure
Presentation
```

As camadas representam limites, não apenas pastas:

- Domain protege estado válido e regras internas;
- Application coordena casos de uso por abstrações;
- Contracts define a API interna oferecida a outros módulos;
- Infrastructure implementa persistência e recursos técnicos;
- Presentation adapta HTTP para a Application.

Controllers são descobertos pelo host através de Application Parts. A lista
atual de projetos deve ser consultada na solution, não duplicada aqui.

## Building Blocks

Building Blocks concentram conceitos transversais reutilizados por módulos da
mesma camada:

```text
Domain       → Domain.Common
Application  → Application.Common
Presentation → Presentation.Common
```

Um Building Block não deve se tornar uma camada de negócio compartilhada.
Entidades, regras específicas e contratos de um bounded context permanecem no
módulo que os possui.

## Comunicação

Chamadas entre módulos usam interfaces, Queries e Snapshots de Contracts. O
consumidor não referencia Domain, Application, Infrastructure ou Presentation
do fornecedor.

A comunicação implementada é síncrona e ocorre dentro do processo. IDs de outros
contextos são referências lógicas; entidades não são compartilhadas e não há
foreign keys entre módulos.

Consulte [comunicação e contratos](data-contracts.md) e
[dependências](dependencies.md).

## Usuário atual

Application acessa a identidade autenticada por `ICurrentUserContext`. A
implementação HTTP pertence ao host e adapta claims do ASP.NET Core para essa
abstração.

Assim, serviços de Application não dependem de `HttpContext`, claims ou
controllers. O uso completo está documentado no
[fluxo de requisição](request-flow.md).

## Persistência

Os módulos compartilham a mesma conexão PostgreSQL, mas cada módulo possui seu
`DbContext` e schema. Uma unidade de trabalho confirma apenas o contexto do
módulo correspondente.

Migrations são aplicadas por um executável separado; a API não altera o schema
ao iniciar. Consulte [persistência](../development/persistence.md) e
[migrations](../development/database-migrations.md).

## Fontes dinâmicas

Use as fontes do repositório para responder perguntas de inventário:

- projetos e referências: solution e `*.csproj`;
- controllers e contratos HTTP: OpenAPI;
- serviços registrados: métodos de injeção de dependência;
- regras automatizadas: projetos de ArchitectureTests;
- mappings e migrations: Infrastructure e projeto de migrations.

## Trade-offs

O desenho atual não fornece transação distribuída, outbox, comunicação
assíncrona, cache ou resiliência entre módulos. O health check não verifica o
banco e a API não possui versionamento.

Decisões pendentes estão consolidadas em
[DOC-002, DOC-004 e DOC-006](../backlog.md).
