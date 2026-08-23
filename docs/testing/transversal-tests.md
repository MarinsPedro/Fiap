# Padrão de testes transversais

## Objetivo

`ArchitectureTests`, `Api.IntegrationTests` e
`Database.IntegrationTests` protegem contratos globais, convenções,
configuração e comportamento compartilhado. Eles não são o principal local para
cenários específicos de features.

## Regra central

Uma feature nova não deve, por padrão, exigir alterações nos projetos
transversais quando segue as regras existentes. Suas invariantes e decisões
devem ser testadas em Domain e Application.

## Características esperadas

Um teste transversal deve cumprir ao menos uma destas funções:

- proteger várias features ao mesmo tempo;
- fiscalizar uma convenção global;
- detectar uma violação estrutural;
- validar um contrato compartilhado;
- descobrir automaticamente novos tipos, endpoints, módulos ou mappings;
- manter baixo custo de manutenção.

## Descoberta automática

Assemblies, projetos, controllers, `DbContext`, entidades e mappings devem ser
descobertos por convenção sempre que possível. Listas fixas de módulos são
aceitas somente quando não houver uma fonte confiável de metadados.

Uma descoberta automática deve falhar de forma clara quando não encontrar os
elementos esperados, evitando testes que passam sem validar nada.

## Quando alterar

Altere testes transversais quando mudar:

- uma fronteira arquitetural;
- uma categoria de projeto;
- o contrato global de erros;
- autenticação ou autorização global;
- OpenAPI, logging ou health check;
- convenção de schema, mapping ou migration.

Não altere apenas porque foi criado:

- endpoint que segue o padrão;
- entidade ou aggregate root;
- service;
- repository;
- migration compatível com a estratégia existente.

## Perguntas para revisão

Antes de adicionar um teste transversal, confirme:

1. A regra é global ou específica de feature?
2. Ela já está protegida em Domain/Application?
3. O teste pode descobrir novos elementos automaticamente?
4. Uma feature semelhante seria coberta sem alterar o teste?
5. A assertion protege contrato ou detalhe interno?
6. O benefício compensa o custo de manutenção?

Se o cenário descreve uma decisão ou regra de negócio específica, ele pertence
provavelmente a Domain ou Application.
