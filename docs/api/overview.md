# API e OpenAPI

`FiapCloudGames.Api` é a entrada HTTP do sistema. Controllers pertencem aos
módulos e são carregados no mesmo host através de Application Parts.

## Fonte de verdade do contrato HTTP

A especificação OpenAPI gerada pela aplicação é a referência atual para:

- rotas e métodos;
- parâmetros de rota e query;
- request e response bodies;
- códigos HTTP declarados;
- content types;
- requisitos de autenticação e autorização.

Não existe uma lista manual paralela de endpoints.

Em Development, consulte:

```text
GET /swagger/v1/swagger.json
GET /swagger
```

Nos perfis locais padrão:

- HTTP: `http://localhost:5080`;
- HTTPS: `https://localhost:7080`.

O Compose executa em Production e não publica o documento ou a interface.

## O que permanece documentado manualmente

Os documentos desta pasta registram contratos e regras transversais que não
devem depender da quantidade atual de controllers:

- [convenções HTTP e JSON](conventions.md);
- [autenticação e autorização](authentication.md);
- [Problem Details e diagnóstico](errors.md).

## Validação

Depois de alterar um contrato HTTP, inicie a API em Development, abra o OpenAPI e
confirme a operação afetada. Os testes de integração também validam convenções
globais do documento gerado.

A ausência de uma operação no OpenAPI indica problema de controller,
Application Part ou configuração; não deve ser compensada com uma lista manual.
