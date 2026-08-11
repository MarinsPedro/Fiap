# Visão geral da API

`FiapCloudGames.Api` é a única entrada HTTP. Controllers dos módulos são
descobertos por `ApplicationPart` e executados no mesmo processo.

## URLs locais

| Perfil | Base URL |
|---|---|
| `dotnet run` HTTP | `http://localhost:5080` |
| `dotnet run` HTTPS | `https://localhost:7080` |
| Docker Compose | `http://localhost:8080` |

As portas dos perfis vêm de `Properties/launchSettings.json`. Elas podem ser
sobrescritas por `ASPNETCORE_URLS`.

## OpenAPI

Em `Development`:

```text
GET /swagger/v1/swagger.json
GET /swagger/index.html
```

O primeiro endereço retorna o documento OpenAPI; o segundo abre a Swagger UI.
O Compose usa `Production`, então não publica nenhum dos dois.

## Recursos

- usuários e autenticação;
- jogos;
- promoções;
- biblioteca do usuário;
- health check.

Consulte a [lista completa de endpoints](endpoints.md).

## Características

- JSON com nomes camelCase;
- enums serializados como string;
- autenticação Bearer JWT;
- controllers com `[ApiController]`;
- erros globais em Problem Details;
- CORS configurável por `Cors:AllowedOrigins`.

Não há versionamento, paginação, rate limiting, idempotency key ou mecanismo
global de filtros/ordenação.
