# ADR-0005: Autenticação centralizada no módulo Identity

## Status

Aceito.

## Contexto

A API precisa cadastrar usuários, validar senhas, emitir JWT e autorizar operações administrativas.

## Decisão

Identity implementa:

- PBKDF2-SHA256 para senhas;
- JWT HS256 com issuer, audience e chave configuráveis;
- validade de duas horas;
- roles `User` e `Administrator`;
- validação JWT registrada no container global.

## Consequências

- `Jwt:Key` com ao menos 32 caracteres é obrigatória no startup;
- não há refresh token ou revogação;
- desativar usuário não invalida token já emitido antes da expiração;
- outros módulos usam claims aplicadas pelo host.

## Alternativas consideradas

Provedor externo de identidade, cookies, chave assimétrica e refresh token não foram implementados nem avaliados em documento versionado.
