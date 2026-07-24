# Diagramas

## Visão geral

```mermaid
flowchart LR
    Client["Cliente"] --> Api["FiapCloudGames.Api"]
    Api --> IP["Identity.Presentation"]
    Api --> CP["Catalog.Presentation"]
    Api --> PP["Promotions.Presentation"]
    Api --> LP["Library.Presentation"]
    IP --> IA["Identity.Application"]
    CP --> CA["Catalog.Application"]
    PP --> PA["Promotions.Application"]
    LP --> LA["Library.Application"]
```

## Dependências entre módulos

```mermaid
flowchart LR
    LA["Library.Application"] --> IC["Identity.Contracts"]
    LA --> CC["Catalog.Contracts"]
    LA --> PC["Promotions.Contracts"]
    PA["Promotions.Application"] --> CC
```

## Fluxo HTTP

```mermaid
flowchart TD
    Request --> Exception["ExceptionHandlingMiddleware"]
    Exception --> Cors
    Cors --> Authentication
    Authentication --> Authorization
    Authorization --> Controller
    Controller --> Service["Application Service"]
    Service --> Domain
    Service --> Repository
    Repository --> DbContext
    DbContext --> PostgreSQL
    PostgreSQL --> Response
```

## Autenticação

```mermaid
sequenceDiagram
    participant Client as Cliente
    participant Auth as AuthenticationController
    participant Login as LoginService
    participant Users as IUserRepository
    participant Hash as IPasswordHasher
    participant Jwt as ITokenGenerator
    Client->>Auth: POST /api/auth/login
    Auth->>Login: LoginInput
    Login->>Users: GetByEmailAsync
    Login->>Hash: Verify
    Login->>Login: verifica IsActive
    Login->>Jwt: Generate
    Jwt-->>Client: JWT HS256, exp +2h
```

## Persistência

```mermaid
flowchart TD
    Connection["ConnectionStrings:Database"] --> IdentityDb["IdentityDbContext<br/>identity"]
    Connection --> CatalogDb["CatalogDbContext<br/>catalog"]
    Connection --> PromotionsDb["PromotionsDbContext<br/>promotions"]
    Connection --> LibraryDb["LibraryDbContext<br/>library"]
    Migrator["FluentMigrator"] --> PostgreSQL[("PostgreSQL")]
    IdentityDb --> PostgreSQL
    CatalogDb --> PostgreSQL
    PromotionsDb --> PostgreSQL
    LibraryDb --> PostgreSQL
```

## Casos de uso

```mermaid
flowchart LR
    User["User"] --> Register["Cadastrar usuário"]
    User --> Login["Autenticar"]
    User --> Browse["Consultar catálogo"]
    User --> Acquire["Adquirir jogo"]
    User --> OwnLibrary["Consultar biblioteca"]
    Admin["Administrator"] --> ManageUsers["Desativar usuário"]
    Admin --> ManageGames["Criar/atualizar jogo"]
    Admin --> ManagePromotions["Criar/encerrar promoção"]
```

## Criação de endpoint

```mermaid
flowchart TD
    Choose["Escolher módulo"] --> Domain["Regra no Domain"]
    Domain --> Contract["Input/output ou Contract"]
    Contract --> App["Application Service"]
    App --> Repo["Repository se necessário"]
    Repo --> Mapping["Mapping EF"]
    Mapping --> Migration["Migration FluentMigrator"]
    Migration --> Controller["Action no Controller"]
    Controller --> DI["Registro na DI"]
    DI --> Tests["Testes"]
    Tests --> Docs["Docs/OpenAPI"]
```

O arquivo editável de Event Storming está em [EventStorming.drawio](../EventStorming.drawio).
