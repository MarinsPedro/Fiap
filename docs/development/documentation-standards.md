# Padrões de documentação

## Princípio central

A documentação manual explica por que o sistema foi desenhado de determinada
forma, quais contratos e regras devem permanecer verdadeiros e como trabalhar no
projeto.

Se uma informação pode ser obtida de modo confiável pelo código, OpenAPI, build
ou testes, não a duplique em Markdown.

## Tipos de documentação

### Manual e estável

Use para:

- arquitetura e fronteiras;
- regras de negócio;
- contratos transversais;
- procedimentos de desenvolvimento e operação;
- riscos, trade-offs e critérios de decisão.

### Dinâmica

Consulte diretamente a fonte para:

- rotas, requests, responses e autorização por endpoint: OpenAPI;
- SDK selecionado: `global.json` e `dotnet --version`;
- projetos e dependências: solution e arquivos de projeto;
- testes existentes e seu resultado: `dotnet test`;
- migrations existentes: projeto de migrations.

Não registre contagens ou listas completas desses itens em documentos manuais.

### Gerada

Quando uma visualização dinâmica for necessária em Markdown, produza-a por
automação e marque o arquivo como gerado e não editável. O repositório ainda não
possui esse fluxo; a decisão está em [DOC-011](../backlog.md).

## Uma informação, um documento autoritativo

Escolha um documento para explicar cada regra. Os demais devem criar links em
vez de copiar o mesmo conteúdo. Exemplos:

- pipeline HTTP: `architecture/request-flow.md`;
- contrato de erros: `api/errors.md`;
- migrations: `development/database-migrations.md`;
- logging: `operations/logging-monitoring.md`.

## Convenções

- Escreva em português do Brasil.
- Use nomes exatos quando eles forem necessários para localizar a fonte.
- Prefira links relativos dentro do repositório.
- Marque exemplos ilustrativos como tal.
- Nunca inclua credenciais, tokens ou connection strings reais.
- Concentre pendências em [docs/backlog.md](../backlog.md).
- Atualize `CHANGELOG.md` quando houver mudança relevante de comportamento.

## Definition of Done

- [ ] A documentação explica uma regra, decisão, contrato ou procedimento útil.
- [ ] Informações dinâmicas apontam para sua fonte de verdade.
- [ ] Não foi criada outra lista manual de endpoints, testes, services ou migrations.
- [ ] O documento autoritativo foi atualizado sem copiar conteúdo para vários lugares.
- [ ] Exemplos ainda representam o padrão arquitetural vigente.
- [ ] Links locais foram verificados.
- [ ] Nenhum segredo real foi adicionado.
- [ ] Pendências novas foram consolidadas no backlog.

## Automação desejada

Lint de Markdown, validação de links, line endings e geração/validação do OpenAPI
devem ser incorporados ao fluxo de CI quando ele existir. Consulte
[DOC-011](../backlog.md).
