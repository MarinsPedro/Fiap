# Como contribuir

Este guia separa regras confirmadas pelo repositório de recomendações que ainda precisam de acordo da equipe.

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
- mudanças de comportamento devem atualizar testes e documentação.

## Branches e commits

Estado atual:

```text
TODO: estratégia oficial de branches não identificada no repositório.
TODO: convenção oficial de commits não identificada no repositório.
TODO: política de merge e número de aprovações não identificados no repositório.
```

Recomendação até a equipe formalizar:

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

`dotnet format` é uma validação recomendada a partir do `.editorconfig`; o repositório não contém pipeline que a imponha.

Verifique também:

- a funcionalidade está no módulo correto;
- Domain não ganhou dependência de framework;
- um mapeamento EF alterado possui migration EF Core e snapshot equivalentes no
  projeto central;
- requests, responses e status estão documentados;
- nenhum token, senha, connection string real ou `.env` foi adicionado;
- os links Markdown alterados resolvem;
- os READMEs do módulo e a documentação central estão coerentes.

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

## Critérios de aceite recomendados

- build sem warnings nem erros;
- testes existentes e novos aprovados;
- cenário de erro coberto;
- autorização revisada;
- migration compatível com o mapping;
- logs sem dados sensíveis;
- documentação e OpenAPI validados;
- revisão por outro desenvolvedor.

## Versionamento e release

Estado atual:

```text
TODO: estratégia de versionamento não identificada no repositório.
TODO: processo oficial de release não identificado no repositório.
```

Não crie tag, release ou publique imagem sem uma decisão explícita da equipe.

## Definition of Done da documentação

Toda mudança que altere comportamento, configuração, arquitetura ou operação deve cumprir:

- [ ] README do módulo atualizado.
- [ ] Documentação central atualizada.
- [ ] Exemplos de request e response atualizados.
- [ ] Configurações documentadas.
- [ ] Migration documentada, quando aplicável.
- [ ] Testes documentados.
- [ ] Diagrama atualizado, quando aplicável.
- [ ] Troubleshooting atualizado, quando aplicável.
- [ ] OpenAPI validado em `Development`.
- [ ] Links verificados.
