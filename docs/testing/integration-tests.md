# Testes de integração

## API

`FiapCloudGames.Api.IntegrationTests` usa
`WebApplicationFactory<Program>`, configura o ambiente `Testing` e chama
`GET /health`. A factory fornece connection string e JWT de teste apenas para
satisfazer a inicialização.

O health check atual não consulta o banco, portanto essa suíte passa mesmo sem
PostgreSQL.

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

Nenhuma conexão é aberta. O teste não valida SQL, constraints, índices, migration
ou comportamento do PostgreSQL.

```powershell
dotnet test tests/Integration/FiapCloudGames.Database.IntegrationTests
```

## Lacunas prioritárias

- subir PostgreSQL isolado para a suíte;
- aplicar todas as migrations FluentMigrator;
- executar cadastro, login, administração, promoção e aquisição via HTTP;
- cobrir 400, 401, 403, 404, 422 e conflitos de concorrência;
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

