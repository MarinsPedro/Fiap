# Primeiros passos

Objetivo: obter build, testes, banco, migration, API, health check, OpenAPI e login funcionando sem conhecimento prévio.

## 1. Acesso e clone

```text
TODO: URL e processo de concessão de acesso ao repositório não identificados.
```

Depois de receber a URL:

```powershell
git clone <URL_DO_REPOSITORIO>
Set-Location <PASTA_CLONADA>
```

Confirme que `FiapCloudGames.sln` está na pasta atual.

## 2. Instalar ferramentas

Obrigatórias:

- .NET SDK 10.0.302 ou patch compatível;
- Docker com Compose, para o fluxo recomendado;
- Git.

Verifique:

```powershell
dotnet --info
docker version
docker compose version
git --version
```

O repositório não define IDE oficial.

```text
TODO: confirmar IDEs e sistemas operacionais oficialmente suportados.
```

Visual Studio, Rider ou VS Code são opções compatíveis com projetos .NET, mas constituem recomendação, não regra do repositório.

## 3. Restaurar, compilar e testar

```powershell
dotnet restore FiapCloudGames.sln
dotnet build FiapCloudGames.sln --no-restore
dotnet test FiapCloudGames.sln --no-build --no-restore
```

Como saber se funcionou:

- restore termina sem `NU*`;
- build informa zero erros e zero warnings;
- test informa zero falhas.

Veja [Testes](../testing/overview.md) para comandos por categoria.

## 4. Configurar segredos locais

```powershell
Copy-Item .env.example .env
```

Edite `.env` e altere todos os valores. O arquivo é ignorado pelo Git.

| Variável | Regra |
|---|---|
| `POSTGRES_PASSWORD` | senha do usuário PostgreSQL do Compose |
| `JWT_KEY` | pelo menos 32 caracteres |
| `ADMIN_NAME` | nome do administrador inicial |
| `ADMIN_EMAIL` | e-mail válido |
| `ADMIN_PASSWORD` | pelo menos 8 caracteres |

Nunca reutilize credenciais de produção.

## 5. Executar pelo Compose

```powershell
docker compose up --build -d
docker compose ps
docker compose logs migrator
```

O fluxo é:

1. database fica healthy;
2. migrator cria schemas/tabelas e termina;
3. API inicia na porta 8080.

Valide:

```powershell
Invoke-WebRequest http://localhost:8080/health
```

O corpo esperado é `Healthy`.

## 6. Executar API em Development

O Compose configura Production e não expõe OpenAPI. Para Development, mantenha o banco do Compose e rode migrador/API localmente conforme [Ambiente local](local-environment.md).

Com o perfil `https`:

```text
https://localhost:7080/openapi/v1.json
https://localhost:7080/health
```

Não há interface Swagger UI, somente JSON.

## 7. Fazer login

Use o e-mail e a senha definidos para o administrador no migrador:

```powershell
$body = @{
  email = "admin@example.com"
  password = "change-me-now"
} | ConvertTo-Json

$login = Invoke-RestMethod `
  -Method Post `
  -Uri https://localhost:7080/api/auth/login `
  -ContentType "application/json" `
  -Body $body

$token = $login.accessToken
```

Teste uma rota administrativa:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri https://localhost:7080/api/games `
  -Headers @{ Authorization = "Bearer $token" } `
  -ContentType "application/json" `
  -Body '{"title":"Cloud Quest","description":"Aventura","category":"RPG","basePrice":99.90}'
```

## 8. Encerrar

```powershell
docker compose down
```

Isso preserva o volume do banco. Para exclusão do volume, leia o alerta em [Docker](../operations/docker.md).

## Próximas leituras

1. [Visão arquitetural](../architecture/overview.md);
2. [Módulos](../architecture/modules.md);
3. [Endpoints](../api/endpoints.md);
4. [Primeira tarefa](first-task.md).
