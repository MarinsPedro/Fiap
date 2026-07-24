# Logging e monitoramento

## Logging atual

A API usa o logging padrão do ASP.NET Core. Em configuração base:

- `Default`: `Information`;
- `Microsoft.AspNetCore`: `Warning`.

Em Development, comandos do Entity Framework Core são registrados em
`Information`. O middleware global registra exceções antes de produzir Problem
Details.

Com Compose:

```powershell
docker compose logs -f api
docker compose logs -f migrator
```

## Dados sensíveis

Não registre:

- senha ou hash de senha;
- JWT ou header `Authorization`;
- `Jwt:Key`;
- connection string com senha;
- conteúdo pessoal desnecessário.

O nível de comandos EF em Development pode revelar valores e detalhes de
consultas. Use somente em ambiente controlado.

## Monitoramento ausente

Não há no repositório:

- OpenTelemetry, tracing ou exportador;
- métricas de negócio/aplicação;
- logs estruturados com correlação padronizada;
- dashboard ou alerta;
- SLO/SLI;
- ferramenta de APM;
- retenção e mascaramento definidos.

O `traceId` presente em Problem Details é o identificador disponível para
correlacionar uma falha com o pipeline atual.

`TODO: definir plataforma de observabilidade, correlação, métricas, alertas,
retenção e política de dados sensíveis.`

