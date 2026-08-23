# Checklist de onboarding

## Primeiro dia

- [ ] Recebi acesso ao repositório.
- [ ] Clonei o projeto e localizei `FiapCloudGames.sln`.
- [ ] Instalei um SDK compatível com `global.json` e confirmei `dotnet --version`.
- [ ] Validei Docker e Docker Compose, se usados.
- [ ] Criei `.env` a partir de `.env.example` sem versioná-lo.
- [ ] Executei `dotnet restore`.
- [ ] Executei `dotnet build`.
- [ ] Executei `dotnet test`.
- [ ] Subi PostgreSQL, migrador e API.
- [ ] Recebi `Healthy` em `/health`.
- [ ] Acessei `/swagger/v1/swagger.json` e a Swagger UI em Development.
- [ ] Fiz login e usei um Bearer token.

## Primeira semana

- [ ] Li a [arquitetura](../architecture/overview.md).
- [ ] Entendi as cinco camadas.
- [ ] Identifiquei os quatro módulos e schemas.
- [ ] Acompanhei o fluxo de uma aquisição.
- [ ] Li autenticação, erros e endpoints.
- [ ] Entendi limitações dos testes de integração.
- [ ] Concluí a [primeira tarefa](first-task.md).
- [ ] Li o processo de contribuição.
- [ ] Abri um pull request pequeno.

## Conhecimento operacional

- [ ] Sei onde definir connection string e chave JWT.
- [ ] Sei aplicar migrations.
- [ ] Sei que rollback não está automatizado.
- [ ] Sei coletar logs do Compose.
- [ ] Sei que health check não valida banco.
- [ ] Sei por que OpenAPI não aparece em Production.
- [ ] Sei remover containers sem apagar o volume.

## Acessos e políticas pendentes

Governança, suporte e ferramenta de gestão estão em
[DOC-001](../backlog.md). Ambientes e processo de deploy estão em
[DOC-007](../backlog.md).
