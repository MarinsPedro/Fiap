# ADR-0003: Comunicação entre módulos por Contracts

## Status

Aceito.

## Contexto

Library precisa consultar usuário, jogo e promoção. Promotions precisa consultar jogos sem depender de implementações de outros módulos.

## Decisão

Expor interfaces e DTOs em projetos `Contracts`. Application consumidora referencia somente esses projetos. IDs representam relações entre domínios; não há foreign keys entre módulos.

## Consequências

- entidades e Infrastructure permanecem privadas ao módulo;
- implementações podem ser substituídas mantendo a interface;
- DTOs públicos precisam de evolução compatível;
- chamadas atuais são síncronas e compartilham o processo;
- tipos de evento declarados ainda não possuem transporte.

## Alternativas consideradas

Referência direta entre Applications/Infrastructure é rejeitada pelos limites e parcialmente pelos testes. Mensageria não foi implementada.
