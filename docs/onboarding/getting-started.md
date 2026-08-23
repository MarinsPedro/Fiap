# Primeiros passos

Objetivo: obter build, testes, banco, migration, API, health check, OpenAPI e login funcionando sem conhecimento prévio.

## 1. Acesso e clone

Para clonar o repositório público:

```powershell
git clone https://github.com/MarinsPedro/Fiap.git
Set-Location Fiap
```

Confirme que `FiapCloudGames.sln` está na pasta atual.

## 2. Instalar ferramentas

Obrigatórias:

- SDK .NET compatível com a política definida em `global.json`;
- uma instância PostgreSQL acessível para migrations e API;
- Git.

Verifique:

```powershell
dotnet --info
git --version
```

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
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=fiap_cloud_games;Username=postgres;Password=<senha-local>"
$env:Jwt__Key = "change-me-with-at-least-32-characters"
$env:Jwt__Issuer = "FiapCloudGames"
$env:Jwt__Audience = "FiapCloudGames.Client"
$env:Admin__Name = "Administrador local"
$env:Admin__Email = "admin@example.com"
$env:Admin__Password = "change-me-now-1!"
```

Substitua os valores ilustrativos e nunca reutilize credenciais de ambientes
compartilhados.

## 5. Aplicar migrations

```powershell
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
```

O migrador cria os schemas e tabelas e executa o seed do administrador quando as
variáveis `Admin__Email` e `Admin__Password` são fornecidas juntas.

## 6. Executar API em Development

```powershell
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
```

Com o perfil `https`:

```text
https://localhost:7080/swagger/v1/swagger.json
https://localhost:7080/swagger/index.html
https://localhost:7080/health
```

O primeiro endereço é o JSON OpenAPI; o segundo é a Swagger UI. O health check
deve retornar `Healthy`.

## 7. Fazer login

Use o e-mail e a senha definidos para o administrador no migrador:

```powershell
$body = @{
  email = "admin@example.com"
  password = "change-me-now-1!"
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

## Próximas leituras

1. [Visão arquitetural](../architecture/overview.md);
2. [Módulos](../architecture/modules.md);
3. [API e OpenAPI](../api/overview.md);
4. [Primeira tarefa](first-task.md).
