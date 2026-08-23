# Health checks

## API

```http
GET /health
```

O endpoint é público e retorna `200 OK` com o texto `Healthy` quando o processo
ASP.NET Core está respondendo.

```powershell
Invoke-WebRequest http://localhost:8080/health
```

O health check registrado não valida conexão com PostgreSQL, estado das
migrations ou dependências entre módulos. Portanto, `200` não garante que os
endpoints de negócio funcionem.

## PostgreSQL no Compose

O serviço `database` usa:

```text
pg_isready -U fiap_cloud_games -d fiap_cloud_games
```

O migrador só inicia depois desse check; a API só inicia quando o migrador termina
com sucesso.

## Uso em orquestradores

O repositório não diferencia readiness, liveness e startup probes.

Checks nomeados, prontidão das migrations, timeouts e probes são acompanhados em
[DOC-006](../backlog.md).

