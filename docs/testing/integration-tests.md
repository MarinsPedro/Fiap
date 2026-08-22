# Testes de integração

## API

`FiapCloudGames.Api.IntegrationTests` usa
`FiapCloudGamesApiFactory`, derivada de `WebApplicationFactory<Program>`, e
configura o ambiente `Testing`. A factory fornece connection string, issuer,
audience e chave JWT de teste apenas para satisfazer a inicialização.

Os casos atuais cobrem:

- `GET /health`;
- JSON inválido e Data Annotations convertidos em
  `ApiProblemDetails` com `errors`;
- normalização de caminhos simples, aninhados e indexados em `camelCase`;
- respostas vazias 401 e 404 completadas por `UseStatusCodePages`;
- todas as categorias de `AppException`;
- `DomainRuleViolationException` como 422;
- exceções técnicas como 500 sanitizado;
- validação estruturada e cancelamento iniciado pelo cliente.
- níveis de logging dos status 4xx e proteção contra vazamento de mensagens de
  validação.

O health check atual não consulta o banco, portanto essa suíte passa mesmo sem
PostgreSQL. Os requests exercitados não alcançam persistência com dados válidos.

```powershell
dotnet test tests/Integration/FiapCloudGames.Api.IntegrationTests
```

## Banco

`FiapCloudGames.Database.IntegrationTests` cria os quatro `DbContext` com o
provedor Npgsql e inspeciona o modelo EF. Ele confirma o mapeamento:

- `identity.users`;
- `catalog.games`;
- `library.game_libraries` e `library.library_games`;
- `promotions.promotions` e `promotions.promotion_games`.

Outro teste configura os contexts como o executável central e confirma que cada
um descobre sua migration EF inicial e que o modelo não possui mudanças
pendentes em relação aos snapshots. Nenhuma conexão é aberta. A suíte ainda não
valida SQL executado, constraints ou comportamento real do PostgreSQL.

```powershell
dotnet test tests/Integration/FiapCloudGames.Database.IntegrationTests
```

## Lacunas prioritárias

- subir PostgreSQL isolado para a suíte;
- aplicar e reverter todas as migrations EF Core;
- executar cadastro, login, administração, promoção e aquisição via HTTP com
  persistência real;
- cobrir autorização 403, tokens válidos/expirados e conflitos reais de
  persistência/concorrência;
- garantir limpeza e isolamento entre casos;
- comparar o modelo EF com o schema migrado;
- verificar que dados de um teste não vazam para outro.

`TODO: adotar container efêmero ou banco dedicado de CI e implementar os fluxos
acima.`

## Regras para futuros testes com banco

- Nunca aponte para banco compartilhado ou de produção.
- Gere nome/schema isolado por execução.
- Execute migrations da mesma forma usada na implantação.
- Não dependa da ordem dos testes.
- Remova dados/recursos temporários ao final.
- Mantenha credenciais de teste fora do repositório quando não forem placeholders.
