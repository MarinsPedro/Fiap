# FIAP Cloud Games

API HTTP para identidade e autenticação de usuários, catálogo de jogos,
promoções e biblioteca de jogos adquiridos.

O projeto é uma implementação de referência em monólito modular sobre ASP.NET
Core e PostgreSQL. Não inclui frontend, carrinho, pagamento ou mensageria.

## Fontes de verdade

Informações que o repositório consegue produzir automaticamente não são
duplicadas neste README:

| Informação | Fonte |
|---|---|
| rotas, métodos, contratos e respostas HTTP | OpenAPI gerado pela API |
| política de seleção do SDK | `global.json` |
| versões de dependências | `Directory.Packages.props` e projetos `*.csproj` |
| projetos da solução | `dotnet sln FiapCloudGames.sln list` |
| migrations disponíveis | projeto `FiapCloudGames.Database.Migrations` |
| estado da suíte de testes | `dotnet test FiapCloudGames.sln` |

Essa separação evita transformar a documentação em uma fotografia manual do
código.

## Arquitetura

A aplicação é um monólito modular. Identity, Catalog, Promotions e Library
compartilham o processo HTTP e o banco físico, mas preservam projetos, regras,
contratos e schemas próprios.

Cada módulo separa responsabilidades de Domain, Application, Contracts,
Infrastructure e Presentation. Comunicação entre módulos usa somente Contracts;
entidades e implementações internas não atravessam essas fronteiras.

Building Blocks concentram conceitos transversais e permanecem restritos à
camada correspondente:

```text
Domain       → Domain.Common
Application  → Application.Common
Presentation → Presentation.Common
```

Consulte a [visão arquitetural](docs/architecture/overview.md), as
[fronteiras dos módulos](docs/architecture/modules.md) e o
[fluxo de requisição](docs/architecture/request-flow.md).

## Preparar o ambiente

Pré-requisitos:

- Git;
- um SDK .NET compatível com a política definida em `global.json`;
- Docker com Compose para o fluxo local recomendado.

Verifique o SDK efetivamente selecionado na sua máquina:

```powershell
dotnet --version
dotnet --info
```

Crie a configuração local e substitua todos os valores de exemplo:

```powershell
Copy-Item .env.example .env
```

O `.env` não deve ser versionado. Nunca reutilize credenciais de ambientes
compartilhados.

## Compilar e testar

```powershell
dotnet restore FiapCloudGames.sln
dotnet build FiapCloudGames.sln --no-restore
dotnet test FiapCloudGames.sln --no-build --no-restore
```

O relatório do comando de testes é a fonte do estado atual da suíte; quantidades
de testes não são mantidas em Markdown.

## Executar com Docker

```powershell
docker compose up --build -d
docker compose ps
docker compose logs migrator
docker compose logs api
```

O Compose inicia PostgreSQL, executa o migrador e só então inicia a API. Valide o
processo em `http://localhost:8080/health`.

O Compose usa `Production`; por isso não expõe OpenAPI nem Swagger UI.

## Executar em Development

Suba o banco, configure `ConnectionStrings__Database`, `Jwt__Key`,
`Jwt__Issuer` e `Jwt__Audience` na sessão atual e execute:

```powershell
dotnet run --project src/Database/FiapCloudGames.Database.Migrations
dotnet run --project src/Api/FiapCloudGames.Api --launch-profile https
```

Durante o desenvolvimento:

- Swagger UI: `https://localhost:7080/swagger`;
- documento OpenAPI: `https://localhost:7080/swagger/v1/swagger.json`;
- health check: `https://localhost:7080/health`.

A especificação OpenAPI é a referência para rotas, autenticação por operação,
parâmetros, requests, responses e códigos HTTP.

## Documentação

O [índice central](docs/README.md) organiza a documentação por objetivo:

- [onboarding](docs/onboarding/getting-started.md);
- [arquitetura](docs/architecture/overview.md);
- [contratos da API](docs/api/overview.md);
- [desenvolvimento](docs/development/creating-an-endpoint.md);
- [testes](docs/testing/overview.md);
- [operações](docs/operations/README.md);
- [pendências consolidadas](docs/backlog.md);
- [contribuição](CONTRIBUTING.md).

## Limites operacionais

O health check atual mede o processo, não o PostgreSQL. Os testes transversais de
banco inspecionam metadados do EF Core sem executar PostgreSQL real. Não há
pipeline de CI/CD, plataforma de produção, backup automatizado, métricas, APM ou
política de rollback aprovada.

Essas lacunas são acompanhadas no [backlog documental](docs/backlog.md).
