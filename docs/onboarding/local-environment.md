# Ambiente local

## Requisitos confirmados

| Item | Versão/configuração | Obrigatório |
|---|---|---:|
| .NET SDK | política definida em `global.json`; confirme com `dotnet --version` | sim |
| PostgreSQL | instância acessível pela connection string | sim para operações com dados |
| HTTPS dev certificate | usado pelo perfil `https` | apenas para HTTPS local |

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

## Banco local

Disponibilize uma instância PostgreSQL e configure a connection string:

```powershell
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=fiap_cloud_games;Password=change-me"
```

## Configuração da API

```powershell
$env:Jwt__Key = "change-me-with-at-least-32-characters"
$env:Jwt__Issuer = "FiapCloudGames"
$env:Jwt__Audience = "FiapCloudGames.Client"
```

O código possui fallback apenas quando as chaves não existem. Como o
`appsettings.json` base declara ambas com string vazia, configure valores não
vazios explicitamente na execução local.

Opcional para CORS:

```powershell
$env:Cors__AllowedOrigins__0 = "http://localhost:3000"
$env:Cors__AllowedOrigins__1 = "https://localhost:3001"
```

## Aplicar migrations e seed

```powershell
$env:Admin__Name = "Administrador local"
$env:Admin__Email = "admin@example.com"
$env:Admin__Password = "change-me-now-1!"
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

Se `Admin__Email` e `Admin__Password` forem ambos omitidos, migrations rodam sem seed. Se apenas um estiver presente, o migrador falha.

O seed está associado somente ao `IdentityDbContext`. Por isso, ele também é
executado por `Update-Database -Context IdentityDbContext` e pelo comando
equivalente do `dotnet-ef`, inclusive quando não há migration pendente. O
`ON CONFLICT (email) DO NOTHING` torna o seed repetível.

## Executar API

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
```

Valide em outro terminal:

```powershell
Invoke-WebRequest https://localhost:7080/health
Invoke-WebRequest https://localhost:7080/swagger/v1/swagger.json
```

A Swagger UI fica em `https://localhost:7080/swagger/index.html`.

## User Secrets

A mensagem de validação sugere variável de ambiente ou user-secrets, mas o `.csproj` da API não possui `UserSecretsId`.

Use variáveis de ambiente para configurar a API.

## IDE e extensões

Nenhuma IDE ou extensão é exigida pelo repositório.

Recomendações:

- suporte C#/.NET;
- cliente HTTP;
- visualizador de Mermaid para os diagramas mantidos em Markdown.

## Diagnóstico rápido

```powershell
dotnet --list-sdks
dotnet tool restore
dotnet build FiapCloudGames.sln
```

Consulte [Troubleshooting](../operations/troubleshooting.md) se algum passo falhar.
