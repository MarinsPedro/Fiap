# Criar uma feature HTTP

Este guia descreve o fluxo arquitetural esperado. Ele não fornece uma
implementação completa para copiar, porque nomes de classes, dependências e
operações evoluem com o código.

Use uma feature semelhante no módulo escolhido como referência concreta.

## Fluxo

```text
Request HTTP
    ↓
Presentation
Controller + Request/Response + mapping
    ↓
Application
Service da feature + Input/Result
    ↓
Domain e portas de persistência/integração
    ↓
Infrastructure
Repository, query e DbContext
```

O resultado percorre o caminho inverso. Entidades não são devolvidas diretamente
pela API.

## 1. Escolher o módulo

A regra e o dado devem pertencer ao mesmo bounded context:

- identidade e credenciais: Identity;
- jogo e preço base: Catalog;
- desconto e vigência: Promotions;
- aquisição e biblioteca: Library.

Se a feature precisar de outro módulo, consuma somente Contracts do fornecedor.

## 2. Definir comportamento e invariantes

Antes do contrato HTTP, descreva:

- resultado esperado;
- estados válidos e inválidos;
- autorização necessária;
- efeitos persistentes;
- conflitos e falhas;
- dependências entre módulos;
- comportamento em repetição e concorrência.

Invariantes que mantêm uma entidade válida pertencem ao Domain. Coordenação,
existência de recursos e políticas do caso de uso pertencem à Application.

## 3. Implementar a Application

Uma feature normalmente:

1. recebe Input ou argumentos sem semântica HTTP;
2. consulta repositories ou portas por abstrações;
3. coordena Domain e Contracts externos;
4. confirma a unidade de trabalho do próprio módulo;
5. devolve Result sem tipos de Presentation;
6. propaga `CancellationToken`.

Application não depende de `DbContext`, `HttpContext`, Controller, Request ou
Response. A identidade atual é obtida por `ICurrentUserContext`.

Siga a convenção vigente encontrada no código. Não introduza outro padrão de
Command/Handler apenas em uma feature isolada.

## 4. Adaptar HTTP em Presentation

Presentation:

- declara Request e Response;
- aplica validações simples do contrato;
- converte Request para Input;
- chama o serviço da feature;
- converte Result para Response;
- declara rota, status e autorização;
- mantém regra de negócio fora do Controller.

Criações devem produzir um `Location` resolvível quando existir leitura do
recurso. Erros usam o contrato global Problem Details.

A lista atual de rotas e schemas não é mantida neste guia: ela pertence ao
OpenAPI.

## 5. Implementar persistência quando necessária

Interfaces de repository pertencem ao Domain quando representam acesso ao
agregado. Portas de leitura específicas podem pertencer à Application.

Infrastructure implementa essas abstrações, aplica tracking apenas na escrita e
não expõe `IQueryable` para outras camadas.

Se o mapping mudar, gere uma migration EF Core no contexto correto, revise
`Up`, `Down` e snapshot e considere compatibilidade de implantação. Consulte
[database-migrations.md](database-migrations.md).

## 6. Registrar dependências

Registre services da Application no módulo e implementações técnicas em
Infrastructure. Um novo módulo Presentation também precisa ser descoberto pelo
host.

O container deve conseguir construir a aplicação sem criar dependência de
Application para Infrastructure.

## 7. Erros e validação

Use as categorias existentes de `AppException`. Não crie uma exceção ou código
HTTP para cada regra.

- contrato/model binding inválido: 400;
- autenticação ausente ou recusada: 401;
- autorização insuficiente: 403;
- recurso ausente: 404;
- conflito: 409;
- regra não processável: 422;
- falha inesperada: 500 sanitizado.

Consulte [validação](validation.md), [tratamento de erros](error-handling.md) e
[contrato público](../api/errors.md).

## 8. Logging

Use `ILogger<T>` com templates estruturados. Registre identificadores e efeitos
úteis, nunca senha, token, e-mail sem necessidade, connection string ou body de
autenticação.

Preserve a correlação da requisição e deixe o middleware registrar respostas 4xx
e falhas inesperadas. Consulte
[logging e monitoramento](../operations/logging-monitoring.md).

## 9. Testes

Cubra comportamento específico em Domain e Application:

- caminho feliz;
- fronteiras;
- falhas semânticas;
- efeitos e ausência de persistência quando o fluxo falha.

Testes transversais só mudam quando a feature altera uma regra global, o
pipeline, o contrato de erros, a arquitetura ou a convenção de persistência.

Execute:

```powershell
dotnet build FiapCloudGames.sln
dotnet test FiapCloudGames.sln --no-build --no-restore
dotnet format FiapCloudGames.sln --verify-no-changes --no-restore
```

O relatório do comando é a fonte do estado atual da suíte.

## 10. Validar OpenAPI

Inicie a API em Development e confirme no OpenAPI:

- rota e método;
- parâmetros e body;
- respostas declaradas;
- content types;
- requisito de segurança.

Não atualize uma lista manual de endpoints.

## Checklist

- [ ] A feature pertence ao bounded context correto.
- [ ] Domain preserva estado válido.
- [ ] Application depende de abstrações.
- [ ] Integrações usam Contracts.
- [ ] Presentation apenas adapta HTTP.
- [ ] Entidades não atravessam fronteiras.
- [ ] Persistência e migration estão sincronizadas.
- [ ] Autenticação e autorização foram revisadas.
- [ ] Erros seguem Problem Details.
- [ ] Logging não expõe dados sensíveis.
- [ ] Testes específicos cobrem o comportamento.
- [ ] OpenAPI representa o contrato resultante.
- [ ] Documentação manual mudou apenas se uma regra ou procedimento mudou.
