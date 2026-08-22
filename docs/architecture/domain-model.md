# Modelo de domínio

## Princípios aplicados

O modelo usa DDD tático dentro de um monólito modular:

- cada módulo representa um bounded context;
- toda alteração de estado ocorre por uma raiz de agregado;
- objetos de valor mantêm conceitos e validações próprios;
- o Domain não depende de HTTP, EF Core, Application ou Contracts;
- referências entre módulos são IDs e DTOs, nunca entidades compartilhadas;
- tempo de negócio entra explicitamente no domínio e vem de `TimeProvider`;
- consultas de leitura podem usar portas próprias da Application sem expor
  tracking ou `IQueryable`.

O Building Block `FiapCloudGames.Domain.Common` contém apenas conceitos
transversais de domínio:

- `DomainRuleViolationException`, falha de invariante interna ao domínio com
  uma mensagem descritiva.

`FiapCloudGames.Application.Common` contém `IUnitOfWork` e a taxonomia de erros
da aplicação. O domínio não referencia esse projeto.

## Bounded contexts e agregados

| Contexto | Raiz de agregado | Entidades internas | Objetos de valor | Responsabilidade |
|---|---|---|---|---|
| Identity | `User` | — | `Email`, `Password` | identidade, credencial, papel e estado do usuário |
| Catalog | `Game` | — | `GamePrice` | cadastro, preço base e disponibilidade do jogo |
| Promotions | `Promotion` | `PromotionGame` | `DiscountPercentage` | vigência, abrangência e cálculo de desconto |
| Library | `GameLibrary` | `LibraryGame` | `AcquisitionPrice` | coleção do usuário e snapshot da aquisição |

`PromotionGame` e `LibraryGame` não são repositories ou agregados
independentes. Seu ciclo de vida pertence, respectivamente, a `Promotion` e
`GameLibrary`.

## Invariantes por agregado

### User

- e-mail válido e normalizado;
- nome entre 2 e 120 caracteres;
- senha de cadastro com ao menos 8 caracteres, letras, números e caracteres
  especiais;
- hash de senha obrigatório;
- papel pertencente a `UserRole`;
- criação em UTC;
- desativação idempotente.

### Game

- título entre 2 e 160 caracteres;
- descrição com até 4.000 caracteres;
- categoria obrigatória com até 80 caracteres;
- preço não negativo, arredondado para duas casas;
- criação em UTC;
- ciclo de vida expresso por `Activate` e `Deactivate`.

### Promotion

- nome entre 2 e 120 caracteres;
- desconto maior que zero e até 100%;
- início e fim em UTC, com fim posterior ao início;
- ao menos um jogo válido e distinto;
- encerramento em UTC e nunca anterior à criação;
- cálculo de preço delegado a `DiscountPercentage`.

### GameLibrary

- usuário válido e criação em UTC;
- um jogo não pode aparecer duas vezes;
- preço de aquisição não negativo;
- promoção opcional, mas nunca `Guid.Empty`;
- aquisição em UTC e nunca anterior à criação da biblioteca;
- inclusão expressa por `AcquireGame`.

## Application e coordenação

As invariantes que mantêm uma entidade válida pertencem ao Domain. A
Application coordena regras que atravessam agregados ou bounded contexts:

- existência e atividade de usuário e jogo;
- unicidade de e-mail consultada no banco;
- escolha da promoção vigente;
- persistência pela unidade de trabalho;
- transformação de falhas semânticas em `AppException`.

Validação de entrada pode antecipar mensagens 400 sem substituir as invariantes
do Domain.

## Persistência

Cada raiz possui um repository no próprio Domain e uma implementação EF na
Infrastructure. Repositories de escrita retornam agregados rastreados; detalhes
de tracking não aparecem na interface de domínio.

Objetos de valor são persistidos por `ValueConverter` nas mesmas colunas
escalares:

| Objeto de valor | Coluna |
|---|---|
| `Email` | `identity.users.email` |
| `Password` | não é persistido; somente o hash resultante é armazenado |
| `GamePrice` | `catalog.games.base_price` |
| `DiscountPercentage` | `promotions.promotions.discount_percent` |
| `AcquisitionPrice` | `library.library_games.price_paid` |

`ILibraryQueries` separa a consulta da biblioteca do repository do agregado,
devolve `LibraryGameReadModel` definido na Application e usa `AsNoTracking` na
implementação.

## Comunicação entre contextos

`Contracts` contém somente fachadas, Queries e Snapshots síncronos. Promotions
consulta Catalog; Library consulta Identity, Catalog e Promotions. Não existem eventos
de integração, dispatcher, outbox ou consumidores implementados.

Um evento só deve ser adicionado quando houver produtor, política de entrega e
consumidor definidos. Se a comunicação se tornar assíncrona, a decisão deve
incluir idempotência, outbox e consistência eventual.

## Proteções automatizadas

Os testes de arquitetura verificam:

- isolamento de Domain, Contracts, Application e Infrastructure;
- ausência de setters públicos nas entidades;
- construtores não públicos nas raízes;
- comunicação da Library apenas pelos Contracts externos;
- isolamento de Presentation no projeto de migrations.
