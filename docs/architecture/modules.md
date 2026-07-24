# Módulos

## Identity

Objetivo: manter usuários, credenciais, autenticação e papéis.

| Item | Implementação |
|---|---|
| Entidade/value object | `User`, `Email`, `UserRole` |
| Casos de uso | `CreateUserService`, `LoginService`, `GetUserService`, `DeactivateUserService` |
| Fachada pública | `IIdentityModule.GetUserAsync` |
| Endpoints | cadastro, login, próprio perfil, consulta administrativa, desativação |
| Persistência | `IdentityDbContext`, schema `identity`, tabela `users` |
| Dependências externas | EF Core, Npgsql, JWT Bearer |
| Configurações | `ConnectionStrings:Database`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` |
| Testes | `FiapCloudGames.Identity.UnitTests`, mappings e arquitetura |

Regras principais:

- e-mail é normalizado para minúsculas e validado;
- nome possui 2 a 120 caracteres;
- senha de cadastro possui no mínimo 8 caracteres;
- e-mail deve ser único;
- novo cadastro recebe role `User`;
- usuário inativo não autentica;
- primeiro `Administrator` pode ser criado pelo migrador.

Evento declarado: `UserDeactivatedIntegrationEvent`.

Estado atual: o evento não é emitido nem consumido.

## Catalog

Objetivo: manter o cadastro consultável de jogos.

| Item | Implementação |
|---|---|
| Entidade | `Game` |
| Casos de uso | criar, atualizar, obter e listar |
| Fachada pública | `ICatalogModule.GetGameAsync` |
| Endpoints | listagem/detalhe públicos; criação/atualização administrativas |
| Persistência | `CatalogDbContext`, schema `catalog`, tabela `games` |
| Dependências externas | EF Core e Npgsql |
| Configuração | `ConnectionStrings:Database` |
| Testes | `FiapCloudGames.Catalog.UnitTests`, mappings e arquitetura |

Regras principais:

- título possui 2 a 160 caracteres;
- categoria é obrigatória;
- preço base não pode ser negativo e é arredondado para duas casas;
- jogos novos iniciam ativos;
- listagem pública retorna somente ativos e ordena por título.

Risco: o mapping limita categoria a 80 e descrição a 4.000 caracteres, mas o Domain não valida esses limites antes do banco.

Evento declarado: `GameDeactivatedIntegrationEvent`.

Estado atual: o evento não é emitido nem consumido.

## Promotions

Objetivo: manter promoções por período e calcular o preço vigente.

| Item | Implementação |
|---|---|
| Agregado | `Promotion` com `PromotionGame` |
| Casos de uso | criar, listar ativas, encerrar e cotar preço |
| Fachada pública | `IPromotionsModule.GetPriceAsync` |
| Dependência entre módulos | `Catalog.Contracts` |
| Endpoints | listagem pública; criação/encerramento administrativos |
| Persistência | `PromotionsDbContext`, tabelas `promotions.promotions` e `promotions.promotion_games` |
| Testes | `FiapCloudGames.Promotions.UnitTests`, mappings e arquitetura |

Regras principais:

- nome possui 2 a 120 caracteres;
- desconto é maior que zero e menor ou igual a 100%;
- fim deve ser posterior ao início;
- ao menos um jogo distinto é obrigatório;
- criação aceita somente jogos existentes e ativos;
- promoção está ativa quando não encerrada e `StartsAtUtc <= agora < EndsAtUtc`;
- se houver mais de uma promoção vigente, o repository escolhe o maior desconto;
- preço final é arredondado para duas casas.

Evento declarado: `PromotionStartedIntegrationEvent`.

Estado atual: o evento não é emitido nem consumido. Não há regra que impeça promoções sobrepostas.

## Library

Objetivo: registrar jogos adquiridos e consultar a biblioteca do usuário autenticado.

| Item | Implementação |
|---|---|
| Agregado | `GameLibrary` com `LibraryGame` |
| Casos de uso | adquirir jogo e consultar biblioteca |
| Fachada pública | `ILibraryModule.GetLibraryAsync` |
| Dependências entre módulos | Identity, Catalog e Promotions Contracts |
| Endpoints | consulta e aquisição autenticadas |
| Persistência | `LibraryDbContext`, `library.game_libraries` e `library.library_games` |
| Testes | `FiapCloudGames.Library.UnitTests`, mappings e arquitetura |

Regras principais:

- usuário deve existir e estar ativo;
- jogo deve existir e estar ativo;
- a mesma biblioteca não recebe o mesmo jogo duas vezes;
- preço pago não pode ser negativo;
- preço, promoção e instante de aquisição são armazenados como snapshot;
- consulta ordena aquisições da mais recente para a mais antiga;
- jogo ausente no catálogo é exibido como `Jogo indisponível`.

Evento declarado: `GameAddedToLibraryIntegrationEvent`.

Estado atual: o evento não é emitido nem consumido. Não há integração com pagamento; adquirir significa registrar diretamente na biblioteca.

## API principal

Responsabilidades confirmadas:

- carregar Controllers por Application Parts;
- registrar todos os módulos;
- configurar JSON com enums como strings;
- configurar CORS;
- autenticar e autorizar;
- executar middleware de exceções;
- expor OpenAPI em Development;
- expor `/health`.

Detalhes: [README da API](../../src/Api/FiapCloudGames.Api/README.md).
