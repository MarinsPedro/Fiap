# Dados de teste

## Princípios

- Usar apenas os dados necessários para expressar o cenário.
- Fixar datas quando o tempo fizer parte do comportamento esperado.
- Usar `TimeProvider` controlado nos serviços de Application.
- Gerar identificadores únicos apenas quando a identidade não fizer parte da regra.
- Nunca usar e-mails pessoais, tokens, chaves ou credenciais reais.
- Manter placeholders de configuração claramente identificados como teste.
- Evitar fixtures compartilhadas quando elas escondem a intenção do cenário.

## UnitTests

Domain e Application constroem seus dados em memória. Builders e fixtures só
devem ser introduzidos quando removem repetição relevante sem esconder valores
importantes para a regra.

Fakes devem começar vazios e receber explicitamente o estado necessário. Spies
devem registrar apenas efeitos observáveis usados pelas assertions.

## API

`FiapCloudGamesApiFactory` fornece configurações isoladas ao host por
`UseSetting`, sem modificar variáveis de ambiente do processo. Tokens de teste
usam chave, issuer e audience exclusivos da suíte.

Endpoints auxiliares da própria suíte podem provocar comportamentos
transversais, como uma exceção inesperada, sem depender de uma feature ou
repository específico. Eles devem ficar fora do OpenAPI público.

## Metadados do banco

Os testes de Database usam uma connection string fictícia apenas para configurar
o provedor Npgsql. Nenhuma conexão é aberta, nenhum dado é persistido e não há
necessidade de limpeza de banco.

Se futuramente testes com PostgreSQL real forem aprovados, essa será uma mudança
de estratégia. O isolamento, ciclo de vida e destino desses testes deverão ser
definidos antes da implementação.
