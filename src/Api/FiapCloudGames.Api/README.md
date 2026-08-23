# FiapCloudGames.Api

## Propósito

Host ASP.NET Core e composition root da solução. Ele compõe os módulos, adapta
dependências externas e configura o pipeline HTTP.

Regra central: o host não contém regras de negócio. Controllers pertencem a
Presentation dos módulos e são carregados por Application Parts.

## Responsabilidades

- registrar Presentation e Infrastructure dos módulos;
- adaptar claims para `ICurrentUserContext`;
- configurar JSON, CORS, autenticação e autorização;
- normalizar erros com Problem Details;
- configurar logging e correlação;
- publicar OpenAPI em Development;
- expor health checks.

O pipeline autoritativo está em
[fluxo de requisição](../../../docs/architecture/request-flow.md).

## Contrato HTTP

O OpenAPI gerado é a fonte de verdade para operações, parâmetros, schemas,
respostas e requisitos de segurança:

```text
/swagger/v1/swagger.json
/swagger
```

Essas rotas existem somente em Development. Não mantenha uma lista manual de
endpoints neste README.

## Executar

Forneça `ConnectionStrings__Database`, `Jwt__Key`, `Jwt__Issuer` e
`Jwt__Audience`, depois execute:

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
```

A política de configuração e segredos está em
[configuração](../../../docs/development/configuration.md).

## Testar

```powershell
dotnet test tests/Integration/FiapCloudGames.Api.IntegrationTests
```

A suíte protege o pipeline e contratos transversais. Regras de feature permanecem
nos testes de Domain e Application.

## Limites

O health check não valida o banco. OpenAPI não é publicado em Production. A API
ainda não possui versionamento, rate limiting ou probes de prontidão de
dependências.
