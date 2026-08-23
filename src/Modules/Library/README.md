# Módulo Library

## Responsabilidade

Library representa a coleção adquirida por um usuário e preserva o snapshot
necessário para explicar cada aquisição.

## Fronteira

Library mantém seu agregado e persistência internamente. Usuário, jogo e promoção
são referências lógicas; não existem entidades ou foreign keys atravessando os
bounded contexts.

## Regras duráveis

- o usuário atual é obtido por `ICurrentUserContext`;
- usuário e jogo precisam estar disponíveis na aquisição;
- o mesmo jogo não pode ser adquirido duas vezes pela mesma biblioteca;
- preço, promoção e instante são preservados como snapshot;
- o título continua sendo dado mestre de Catalog;
- aquisição não executa pagamento;
- leitura usa projeção sem tracking antes de produzir o resultado.

## Integrações

Library consulta Identity, Catalog e Promotions somente pelas interfaces de
Contracts. Cada módulo confirma sua própria unidade de trabalho; não há transação
distribuída.

## API e testes

O OpenAPI é a fonte de verdade para as operações HTTP. A suíte do módulo pode ser
executada com:

```powershell
dotnet test tests/Unit/FiapCloudGames.Library.UnitTests
```

Consulte [fluxo de requisição](../../../docs/architecture/request-flow.md) e
[regras de negócio](../../../docs/development/business-rules.md). Pagamento e
concorrência são acompanhados em
[DOC-003 e DOC-004](../../../docs/backlog.md).
