# Módulo Identity

## Responsabilidade

Identity é responsável por identidade, credenciais, autenticação, papéis e ciclo
de vida do usuário.

## Fronteira

O módulo preserva suas regras e persistência internamente. Outros módulos podem
consultar dados mínimos de identidade apenas por Contracts; não recebem a
entidade de usuário, hash de senha, repository ou `DbContext`.

Identity não depende das camadas internas de outros módulos.

## Regras duráveis

- e-mail é validado e normalizado;
- senha precisa atender à política do domínio;
- senha nunca é persistida em texto puro;
- novos usuários recebem papel sem privilégio administrativo;
- usuário inativo não autentica;
- o administrador inicial é criado fora da API pública;
- Application obtém o usuário atual por `ICurrentUserContext`;
- regras de identidade não dependem de ASP.NET Core.

## Persistência e segurança

O schema de Identity pertence exclusivamente ao módulo. Hash de senha e emissão
de JWT são implementações técnicas de Infrastructure, acessadas por abstrações da
Application.

Consulte [autenticação e autorização](../../../docs/development/authentication-authorization.md)
e [regras de negócio](../../../docs/development/business-rules.md).

## API e testes

Operações e contratos HTTP atuais devem ser consultados no OpenAPI. Casos de uso
e cenários existentes devem ser consultados no código e executados com:

```powershell
dotnet test tests/Unit/FiapCloudGames.Identity.UnitTests
```
