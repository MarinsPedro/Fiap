# FiapCloudGames.Api

## Propósito

Host ASP.NET Core da solução. Ele compõe os quatro módulos, configura middleware,
autenticação, autorização, CORS, OpenAPI e health checks.

## Composição

O projeto referencia Presentation e Infrastructure de Identity, Catalog,
Promotions e Library. Os controllers são carregados com `AddApplicationPart`; não
ficam neste diretório.

Pipeline:

1. tratamento global de exceções;
2. CORS;
3. autenticação;
4. autorização;
5. OpenAPI somente em Development;
6. health checks e controllers.

## Executar

Defina no mínimo:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
$env:Jwt__Key = "<chave-local-com-pelo-menos-32-caracteres>"
dotnet run --project src/Api/FiapCloudGames.Api
```

Perfis locais:

- `http://localhost:5080`;
- `https://localhost:7080`;
- OpenAPI em `/swagger/v1/swagger.json` somente em Development;
- Swagger UI em `/swagger/index.html` somente em Development.

## Configurações

Veja [configuração](../../../docs/development/configuration.md). Nunca grave
segredos reais nos arquivos `appsettings`.

## Testes

```powershell
dotnet test tests/Integration/FiapCloudGames.Api.IntegrationTests
```

Os testes da API protegem contratos transversais e são separados em:

- `Host`: pipeline real com `WebApplicationFactory` e `HttpClient`;
- `Components`: middlewares, factories e extensões compartilhadas;
- `Contracts`: OpenAPI e convenções declaradas pelos controllers;
- `Support`: infraestrutura exclusiva da suíte.

Regras específicas de negócio permanecem nos testes de Domain e Application.
Um endpoint que segue os contratos globais não precisa de um novo teste HTTP por
padrão.

Consulte o [padrão de integração
transversal](../../../docs/testing/integration-tests.md).

## Limitações

- health check não verifica o banco;
- não há versionamento ou rate limiting;
- não há observabilidade além de logs/health;

Documentação da API: [visão geral](../../../docs/api/overview.md) e
[endpoints](../../../docs/api/endpoints.md).
