# Autenticação da API

## Contrato

A API usa Bearer JWT. Identity valida a credencial, emite o token e o host aplica
autenticação e autorização antes dos controllers.

O OpenAPI é a fonte de verdade para descobrir quais operações são públicas,
autenticadas ou restritas a Administrator.

## Fluxo

```text
credencial
→ Identity valida e-mail, senha e estado do usuário
→ token JWT assinado é emitido
→ cliente envia Authorization: Bearer <token>
→ host valida assinatura, issuer, audience e expiração
→ autorização avalia autenticação e role
```

O token contém identificador do usuário, e-mail, papel, instante de emissão e
identificador do token. A validade implementada é de duas horas e a tolerância de
relógio na validação é de um minuto.

## Papéis

- `User`: identidade autenticada e operações da própria conta;
- `Administrator`: operações administrativas.

Novos cadastros recebem `User`. O administrador inicial é criado pelo migrador;
não há operação pública para elevar privilégios.

## Identidade atual

Application não acessa `HttpContext` diretamente. O host adapta claims para
`ICurrentUserContext`, e os serviços obtêm o identificador autenticado por essa
abstração.

## Uso

Após obter um token pela operação de login indicada no OpenAPI:

```http
Authorization: Bearer <token>
```

Respostas sem credencial válida usam 401. Um token válido sem a role necessária
usa 403. Ambas seguem o contrato descrito em [erros](errors.md).

## Limitações

Não há refresh token, revogação, MFA, recuperação de senha ou confirmação de
e-mail.
