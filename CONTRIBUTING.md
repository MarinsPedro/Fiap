# Como contribuir

Este guia documenta as regras técnicas e o fluxo de contribuição do repositório.

## Preparar o ambiente

Siga [Primeiros passos](docs/onboarding/getting-started.md). Antes de alterar código:

```powershell
dotnet restore FiapCloudGames.sln
dotnet build FiapCloudGames.sln --no-restore
dotnet test FiapCloudGames.sln --no-build --no-restore
```

## Regras técnicas confirmadas

- o código deve compilar para `net10.0`;
- nullable reference types, implicit usings e análise de estilo estão habilitados;
- warnings são tratados como erros;
- versões de pacote pertencem a `Directory.Packages.props`;
- arquivos C# usam namespace file-scoped e indentação de quatro espaços;
- Application não referencia Infrastructure;
- comunicação entre módulos usa somente `Contracts`;
- migrations pertencem ao projeto central;
- segredos não devem ser versionados;
- toda operação assíncrona exposta recebe `CancellationToken`;
- mudanças de comportamento devem atualizar testes; documentação manual muda
  quando houver nova regra, contrato transversal ou procedimento.

## Branches e commits

Use este fluxo:

1. crie uma branch curta a partir da branch principal, por exemplo `feature/catalog-filter`;
2. mantenha commits pequenos e descritivos;
3. evite misturar refatoração sem relação com a mudança;
4. não faça rebase ou force-push em branches compartilhadas sem combinar com os demais autores.

## Antes de abrir o pull request

Execute:

```powershell
dotnet restore FiapCloudGames.sln
dotnet build FiapCloudGames.sln --no-restore
dotnet test FiapCloudGames.sln --no-build --no-restore
dotnet format FiapCloudGames.sln --verify-no-changes --no-restore
```

`dotnet format` valida o código a partir do `.editorconfig`.

Verifique também:

- a funcionalidade está no módulo correto;
- Domain não ganhou dependência de framework;
- um mapeamento EF alterado possui migration EF Core e snapshot equivalentes no
  projeto central;
- requests, responses, status e autorização aparecem corretamente no OpenAPI;
- nenhum token, senha ou connection string real foi adicionado;
- os links Markdown alterados resolvem;
- o documento autoritativo foi atualizado quando uma regra ou procedimento mudou.

## Template de pull request

O repositório não contém template automatizado. Use este modelo no corpo do PR:

```markdown
## Objetivo

Descreva o problema e o resultado esperado.

## Mudanças

- mudança 1;
- mudança 2.

## Impactos

- [ ] API/contrato público
- [ ] regra de negócio
- [ ] banco/migration
- [ ] configuração/segredo
- [ ] operação/deploy
- [ ] documentação

## Validação

Comandos executados e resultados reais.

## Evidências

Requests/responses, logs sanitizados ou capturas quando aplicável.

## Rollback

Como desfazer a mudança de código e de banco.
```

## Critérios de aceite

- build sem warnings nem erros;
- testes existentes e novos aprovados;
- cenário de erro coberto;
- autorização revisada;
- migration compatível com o mapping;
- logs sem dados sensíveis;
- documentação e OpenAPI validados;
- revisão por outro desenvolvedor.

## Definition of Done da documentação

Toda mudança que altere regra, configuração, arquitetura ou operação deve cumprir:

- [ ] O documento autoritativo foi atualizado, quando necessário.
- [ ] Nenhum inventário manual de endpoints, services, testes ou migrations foi criado.
- [ ] Configurações e procedimentos novos foram documentados.
- [ ] Migration e estratégia de implantação foram explicadas, quando aplicável.
- [ ] Diagramas conceituais continuam coerentes, quando afetados.
- [ ] Troubleshooting foi atualizado para novos modos de falha, quando aplicável.
- [ ] OpenAPI validado em `Development`.
- [ ] Links verificados.
