# Changelog

Todas as mudanças relevantes do projeto devem ser registradas neste arquivo.

O repositório não informa uma estratégia de versionamento nem contém tags ou releases consultáveis nesta documentação.

## Não publicado

### Adicionado

- migrations EF Core e snapshots para Identity, Catalog, Promotions e Library;
- ferramenta local `dotnet-ef` 10.0.10;
- teste de descoberta das migrations por `DbContext`;
- documentação técnica central em português do Brasil;
- guias de onboarding, arquitetura, desenvolvimento, testes, API e operações;
- READMEs dos módulos, API, migrador e testes;
- índice e template para Architecture Decision Records (ADRs);
- política de contribuição e Definition of Done da documentação.

### Estado funcional documentado

- API ASP.NET Core em .NET 10;
- módulos Identity, Catalog, Promotions e Library;
- autenticação JWT e autorização por roles;
- persistência PostgreSQL por schema;
- migrations EF Core centralizadas, consumindo os `Infrastructure` dos módulos;
- execução com Docker Compose;
- testes unitários, de integração de modelo/host e de arquitetura.

### Alterado

- os casos de uso foram separados em classes e arquivos por responsabilidade,
  organizados por feature em Application;
- contratos entre módulos agora possuem um arquivo por Query, Snapshot e
  interface `I*Module`;
- contratos HTTP, mappings e Controllers foram organizados em pastas
  `Features` dentro de Presentation;
- a leitura da Library passou a usar `LibraryGameReadModel` e resultados
  próprios antes do mapeamento para Snapshot/Response;
- as factories de design-time do EF Core foram separadas por `DbContext` e a
  connection string de scaffolding passou a ser obrigatória;
- os testes HTTP compartilham `FiapCloudGamesApiFactory` para inicialização do
  host;
- toda a documentação foi revisada contra a estrutura atual, incluindo rotas do
  Swagger, contagem de testes, snippets e troubleshooting;
- o migrador deixou de usar FluentMigrator e agora executa
  `Database.MigrateAsync()` sequencialmente nos quatro contextos;
- cada contexto usa uma tabela de histórico EF própria;
- a regra arquitetural agora permite dependências do migrador para
  `Infrastructure`, mas continua proibindo `Presentation`.
- a connection string deixou de ser armazenada no `appsettings.json` do
  migrador.

### Pendências conhecidas

- TODO: definir versão inicial e processo de release;
- TODO: implementar e documentar CI/CD;
- TODO: definir alvo de deploy e rollback operacional;
- TODO: adicionar observabilidade e health check de banco se exigidos pela operação.
