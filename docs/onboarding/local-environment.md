# Ambiente local

## Requisitos confirmados

| Item | Versão/configuração | Obrigatório |
|---|---|---:|
| .NET SDK | 10.0.302, roll-forward `latestPatch` | sim |
| PostgreSQL | Compose usa 17-alpine | sim para operações com dados |
| Docker Compose | versão não fixada | não se PostgreSQL externo estiver disponível |
| HTTPS dev certificate | usado pelo perfil `https` | apenas para HTTPS local |

```text
TODO: sistemas operacionais oficialmente suportados não identificados.
TODO: versões mínimas de Docker e Compose não identificadas.
```

## Certificado HTTPS

O perfil `https` usa `https://localhost:7080`. Se a máquina não confiar no certificado:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Alternativa sem certificado:

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile http
```

URL: `http://localhost:5080`.

## Banco isolado

Defina a senha e suba somente PostgreSQL:

```powershell
$env:POSTGRES_PASSWORD = "change-me"
docker compose up database -d
docker compose ps database
```

Connection string correspondente:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=fiap_cloud_games;Password=change-me"
```

## Configuração da API

```powershell
$env:Jwt__Key = "change-me-with-at-least-32-characters"
$env:Jwt__Issuer = "FiapCloudGames"
$env:Jwt__Audience = "FiapCloudGames.Client"
```

`Jwt__Issuer` e `Jwt__Audience` possuem fallback no código, mas mantê-los explícitos reduz ambiguidade.

Opcional para CORS:

```powershell
$env:Cors__AllowedOrigins__0 = "http://localhost:3000"
$env:Cors__AllowedOrigins__1 = "https://localhost:3001"
```

## Aplicar migrations e seed

```powershell
$env:Admin__Name = "Administrador local"
$env:Admin__Email = "admin@example.com"
$env:Admin__Password = "change-me-now"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

Se `Admin__Email` e `Admin__Password` forem ambos omitidos, migrations rodam sem seed. Se apenas um estiver presente, o migrador falha.

O `ON CONFLICT (email) DO NOTHING` torna o seed repetível.

## Executar API

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
```

Valide em outro terminal:

```powershell
Invoke-WebRequest https://localhost:7080/health
Invoke-WebRequest https://localhost:7080/openapi/v1.json
```

## User Secrets

A mensagem de validação sugere variável de ambiente ou user-secrets, mas o `.csproj` da API não possui `UserSecretsId`.

Estado atual:

```text
TODO: suporte versionado a dotnet user-secrets não configurado no projeto.
```

Use variáveis de ambiente ou `.env` com Compose até existir decisão e configuração próprias.

## IDE e extensões

Nenhuma IDE ou extensão é exigida pelo repositório.

Recomendações:

- suporte C#/.NET;
- cliente HTTP;
- visualizador Mermaid;
- integração Docker;
- editor draw.io para `docs/EventStorming.drawio`.

## Diagnóstico rápido

```powershell
dotnet --list-sdks
docker compose config
docker compose ps
docker compose logs database
docker compose logs migrator
docker compose logs api
```

Consulte [Troubleshooting](../operations/troubleshooting.md) se algum passo falhar.
