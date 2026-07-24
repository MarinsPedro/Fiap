# ADR-0001: Monólito modular

## Status

Aceito.

## Contexto

Identity, Catalog, Promotions e Library fazem parte do mesmo produto, mas possuem responsabilidades e dados distintos. A solution precisa de uma única API operacional sem misturar as camadas dos domínios.

## Decisão

Hospedar todos os módulos em `FiapCloudGames.Api` e separar cada módulo em Domain, Contracts, Application, Infrastructure e Presentation. Usar um `DbContext` e schema por módulo.

## Consequências

- um processo e um deploy da API;
- chamadas entre módulos podem ser locais;
- fronteiras exigem disciplina e testes arquiteturais;
- uma mudança em qualquer módulo recompila/publica o mesmo host;
- futura extração exige substituir implementações de Contracts.

## Alternativas consideradas

Não há registro versionado de avaliação formal de microserviços ou monólito em camadas.

```text
TODO: confirmar com a equipe quais alternativas foram avaliadas originalmente.
```
