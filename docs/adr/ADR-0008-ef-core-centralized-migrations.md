# ADR-0008: EF Core Migrations centralizadas

## Status

Aceito.

## Contexto

Cada módulo possui um `DbContext` e o mapeamento relacional já é mantido em seu
projeto Infrastructure. A estratégia anterior exigia reproduzir manualmente
esses mapeamentos em FluentMigrator, criando risco de divergência.

O projeto de migrations pode depender das camadas Infrastructure dos módulos,
mas a API não deve aplicar mudanças de schema ao iniciar.

## Decisão

Usar EF Core Migrations no executável
`FiapCloudGames.Database.Migrations`.

- o projeto referencia os quatro projetos Infrastructure;
- migrations e snapshots ficam centralizados, separados por `DbContext`;
- uma `IDesignTimeDbContextFactory<TContext>` por contexto permite scaffolding
  com `dotnet-ef` e exige `ConnectionStrings__Database`;
- cada contexto possui sua própria tabela de histórico EF;
- os históricos EF ficam no schema técnico `infra`, fora de `public`;
- antes das migrations, o executável garante a existência do schema `infra`;
- o executável chama `Database.MigrateAsync()` em Identity, Catalog, Promotions
  e Library, nessa ordem;
- a API continua sem executar migrations no startup;
- o seed opcional de administrador ocorre depois das migrations.

## Consequências

- modelo, migration e snapshot são gerados pela mesma toolchain EF;
- o migrador passa a ter dependência direta de Infrastructure;
- alterações de mapping exigem uma migration no contexto correspondente;
- rollbacks podem ser feitos por contexto com `dotnet ef database update`;
- implantação continua executando o migrador antes da API;
- quatro históricos independentes precisam ser preservados;
- o usuário do migrador precisa de permissão para criar o schema `infra`.

## Alternativas consideradas

- manter FluentMigrator: rejeitado pela duplicação entre mapping e migration;
- migrations dentro de cada Infrastructure: rejeitado para manter um único
  artefato executável de banco;
- aplicar migrations no startup da API: rejeitado para não conceder permissão de
  alteração de schema ao processo HTTP.
