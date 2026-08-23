# Diagramas

Os diagramas representam regras de direção e fluxo, não inventários completos de
classes ou operações.

## Módulos e host

```mermaid
flowchart LR
    Client["Cliente HTTP"] --> Api["API / Composition Root"]
    Api --> Identity
    Api --> Catalog
    Api --> Promotions
    Api --> Library
    Library -. Contracts .-> Identity
    Library -. Contracts .-> Catalog
    Library -. Contracts .-> Promotions
    Promotions -. Contracts .-> Catalog
```

## Camadas

```mermaid
flowchart LR
    HTTP --> Presentation
    Presentation --> Application
    Application --> Domain
    Application --> Contracts
    Infrastructure --> Application
    Infrastructure --> Domain
    API --> Presentation
    API --> Infrastructure
```

As setas representam dependências permitidas. Contracts externos são a única
entrada entre módulos.

## Pipeline HTTP

```mermaid
flowchart TD
    Request --> ClientLog["ClientErrorLoggingMiddleware"]
    ClientLog --> Exception["ExceptionHandlingMiddleware"]
    Exception --> Status["StatusCodePages"]
    Status --> Cors
    Cors --> Authentication
    Authentication --> Authorization
    Authorization --> Endpoint["OpenAPI, Health ou Controller"]
    Endpoint --> Application["Application Service"]
    Application --> Domain
    Application --> Contracts
    Application --> Repository
    Repository --> DbContext
    DbContext --> PostgreSQL
    PostgreSQL --> Response
```

O fluxo autoritativo e suas responsabilidades estão em
[request-flow.md](request-flow.md).

## Identidade atual

```mermaid
flowchart LR
    Claims["Claims do JWT"] --> Adapter["HttpCurrentUserContext"]
    Adapter --> Port["ICurrentUserContext"]
    Port --> Service["Application Service"]
```

Application conhece a porta, mas não conhece `HttpContext`.

## Persistência

```mermaid
flowchart TD
    Connection["ConnectionStrings:Database"] --> IdentityDb["Identity DbContext"]
    Connection --> CatalogDb["Catalog DbContext"]
    Connection --> PromotionsDb["Promotions DbContext"]
    Connection --> LibraryDb["Library DbContext"]
    Migrator["Executável de migrations"] --> IdentityDb
    Migrator --> CatalogDb
    Migrator --> PromotionsDb
    Migrator --> LibraryDb
    IdentityDb --> PostgreSQL
    CatalogDb --> PostgreSQL
    PromotionsDb --> PostgreSQL
    LibraryDb --> PostgreSQL
```
