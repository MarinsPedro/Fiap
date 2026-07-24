# Módulo Identity

## Propósito e responsabilidades

Identity gerencia cadastro, consulta e inativação de usuários, validação de
credenciais, hash de senha, emissão de JWT e papéis.

## Camadas

| Projeto | Responsabilidade |
|---|---|
| `FiapCloudGames.Identity.Domain` | `User`, `Email`, papel e repositório de domínio |
| `FiapCloudGames.Identity.Application` | casos de uso e portas de senha/token/unidade de trabalho |
| `FiapCloudGames.Identity.Contracts` | `UserSummary`, `IIdentityModule` e evento declarado |
| `FiapCloudGames.Identity.Infrastructure` | EF Core, repositório, PBKDF2 e JWT |
| `FiapCloudGames.Identity.Presentation` | controllers de usuários e autenticação |

## Endpoints

- `POST /api/users` — público;
- `POST /api/auth/login` — público;
- `GET /api/users/me` — autenticado;
- `GET /api/users/{id}` — Administrator;
- `DELETE /api/users/{id}` — Administrator.

Detalhes em [endpoints](../../../docs/api/endpoints.md).

## Persistência

`IdentityDbContext` é dono do schema `identity` e da tabela `users`. O módulo não
acessa tabelas de outro schema.

## Integrações

Expõe `IIdentityModule` para consulta interna por contrato. Library consome esse
contrato. `UserDeactivatedIntegrationEvent` está declarado, mas não é publicado
nem consumido.

## Regras principais

- e-mail normalizado e único;
- nome entre 2 e 120 caracteres;
- senha de cadastro com pelo menos 8 caracteres;
- papel padrão `User`;
- usuário inativo não autentica;
- PBKDF2/SHA-256 para armazenamento de senha.

Veja [regras de negócio](../../../docs/development/business-rules.md) e
[autenticação](../../../docs/development/authentication-authorization.md).

## Testar

```powershell
dotnet test tests/Unit/FiapCloudGames.Identity.UnitTests
```

## Evolução

- `TODO: definir recuperação de senha, confirmação de e-mail e MFA.`
- `TODO: implementar publicação do evento de inativação.`
- `TODO: definir refresh/revogação e rotação de JWT.`

