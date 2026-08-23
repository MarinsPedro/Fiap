# Health checks

## API

```http
GET /health
```

O endpoint é público e retorna `200 OK` com o texto `Healthy` quando o processo
ASP.NET Core está respondendo.

```powershell
Invoke-WebRequest http://localhost:5080/health
```

O health check registrado não valida conexão com PostgreSQL, estado das
migrations ou dependências entre módulos. Portanto, `200` não garante que os
endpoints de negócio funcionem.

## Uso em orquestradores

O repositório não diferencia readiness, liveness e startup probes.

