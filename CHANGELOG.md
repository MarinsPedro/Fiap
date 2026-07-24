# Changelog

Todas as mudanças relevantes do projeto devem ser registradas neste arquivo.

O repositório não informa uma estratégia de versionamento nem contém tags ou releases consultáveis nesta documentação.

## Não publicado

### Adicionado

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
- migrations centralizadas com FluentMigrator;
- execução com Docker Compose;
- testes unitários, de integração de modelo/host e de arquitetura.

### Pendências conhecidas

- TODO: definir versão inicial e processo de release;
- TODO: implementar e documentar CI/CD;
- TODO: definir alvo de deploy e rollback operacional;
- TODO: adicionar observabilidade e health check de banco se exigidos pela operação.
