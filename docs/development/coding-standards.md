# Padrões de código

## Regras automatizadas

`Directory.Build.props` aplica a todos os projetos:

- target framework `net10.0`;
- nullable reference types habilitados;
- implicit usings habilitados;
- warnings tratados como erros.

O `.editorconfig` define codificação, final de linha, indentação e convenções
analisáveis pelo ecossistema .NET. A referência efetiva é sempre o arquivo do
repositório.

Antes de enviar:

```powershell
dotnet format FiapCloudGames.sln --verify-no-changes --no-restore
dotnet build FiapCloudGames.sln --no-restore
dotnet test FiapCloudGames.sln --no-build --no-restore
```

## Organização

- Domínio não referencia Application, Infrastructure ou API.
- Application orquestra casos de uso e declara portas.
- Infrastructure implementa persistência e integrações.
- Contracts contém Queries, Snapshots e fachadas compartilháveis.
- Presentation converte Request em Input e Result em Response.
- Application Services não recebem ou devolvem objetos HTTP.
- Entidades nunca atravessam a API ou a fronteira entre módulos.
- API expõe a entrada HTTP e registra os módulos.
- Código de um módulo não acessa o `DbContext` de outro.

Consulte [Camadas](../architecture/layers.md) e
[Dependências](../architecture/dependencies.md).

## Convenções observadas

- namespaces acompanham projeto, módulo e camada;
- tipos públicos usam PascalCase;
- contratos assíncronos usam `Task` e recebem `CancellationToken`;
- injeção de dependência é feita por métodos de extensão de cada camada;
- APIs usam controllers com `[ApiController]` e rotas explícitas;
- Application e Presentation agrupam tipos por feature e mantêm services,
  contratos de dados e mappings em arquivos separados;
- entidades protegem invariantes em construtores e métodos;
- dependências externas são encapsuladas por interfaces de Application.

## Evite

- adicionar referência de Domain para EF Core ou ASP.NET Core;
- consultar diretamente tabelas de outro schema;
- inserir segredo ou credencial real no código;
- colocar regra de negócio exclusivamente no controller;
- editar o schema manualmente sem gerar migration e snapshot EF Core;
- capturar `Exception` sem tratamento ou contexto;
- expor entidades persistidas diretamente como contrato HTTP.

## Mudanças estruturais

Uma mudança nas fronteiras de módulo, comunicação, autenticação, persistência ou
tratamento global de erros deve atualizar a documentação de arquitetura e, quando
for uma decisão durável, receber um [ADR](../adr/README.md).
