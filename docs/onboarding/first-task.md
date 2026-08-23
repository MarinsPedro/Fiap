# Primeira contribuição

A primeira contribuição deve ser pequena, verificável e vinculada a uma
necessidade real. Não existe um teste ou arquivo fixo para copiar: esse tipo de
exercício envelhece quando o código evolui.

## 1. Selecionar o trabalho

Escolha uma tarefa pequena aprovada pela equipe, como:

- ampliar a cobertura de uma fronteira arquitetural;
- melhorar uma mensagem ou validação;
- corrigir um link ou procedimento;
- realizar refatoração local que preserve contratos.

## 2. Localizar a fonte

Use busca e as fontes dinâmicas:

```powershell
rg --files src tests docs
rg -n "<termo-da-feature>" src tests docs
dotnet sln FiapCloudGames.sln list
```

Para uma operação HTTP, consulte também o OpenAPI em Development.

Identifique o módulo, a camada, o teste mais próximo e o documento autoritativo
da regra antes de alterar qualquer arquivo.

## 3. Validar o estado inicial

```powershell
dotnet build FiapCloudGames.sln
dotnet test FiapCloudGames.sln --no-build --no-restore
```

Registre o resultado real. Não copie uma quantidade de testes para a
documentação.

## 4. Fazer a menor mudança coerente

Preserve as fronteiras:

- invariantes em Domain;
- coordenação em Application;
- implementação técnica em Infrastructure;
- adaptação HTTP em Presentation;
- comunicação externa somente por Contracts.

Evite refatorações sem relação com a tarefa.

## 5. Testar

Crie ou ajuste o cenário no projeto de teste do módulo. O teste deve falhar sem a
mudança e passar com ela.

Depois execute novamente build, suíte completa e formatação.

## 6. Atualizar documentação somente quando necessário

Não atualize Markdown apenas porque nasceu uma classe, service, controller ou
teste.

Atualize documentação quando a mudança alterar:

- regra de negócio;
- fronteira arquitetural;
- contrato transversal;
- configuração;
- procedimento de desenvolvimento ou operação.

O OpenAPI deve refletir automaticamente mudanças do contrato HTTP.

## 7. Preparar a revisão

No pull request, informe objetivo, comportamento, riscos, comandos executados e
resultados. Siga [CONTRIBUTING.md](../../CONTRIBUTING.md).

## Checklist

- [ ] A tarefa foi aprovada e tem escopo pequeno.
- [ ] Localizei a fonte de verdade antes de editar.
- [ ] Build e testes iniciais passaram.
- [ ] A mudança respeita módulo e camada.
- [ ] O cenário automatizado protege o comportamento.
- [ ] Build, testes e formatação finais passaram.
- [ ] A documentação não ganhou inventário duplicado.
- [ ] Nenhum segredo ou arquivo gerado indevido foi incluído.
