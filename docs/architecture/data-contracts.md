# Comunicação e contratos entre módulos

## Objetivo

Contracts define a API interna oferecida por um bounded context. Eles permitem
que outro módulo consulte dados sem acessar entidades, services internos,
repositories ou Infrastructure.

## Fluxo dentro de uma feature

```text
HTTP
  ↓
Request da Presentation
  ↓
Input da Application
  ↓
Domain e portas
  ↓
Result da Application
  ↓
Response da Presentation
  ↓
HTTP
```

Cada representação pertence à sua fronteira. Entidades não são contratos de
transporte.

## Fluxo entre módulos

```text
Application consumidora
  ↓
Query de Contracts do fornecedor
  ↓
Interface pública do módulo
  ↓
Snapshot imutável
```

O consumidor referencia apenas o projeto Contracts do fornecedor.

## Responsabilidades por tipo

| Tipo | Responsabilidade |
|---|---|
| Request | formato recebido por HTTP |
| Response | formato devolvido por HTTP |
| Input | dados necessários ao caso de uso |
| Result | saída da Application sem semântica HTTP |
| Query | intenção de consulta entre módulos |
| Snapshot | visão mínima e imutável devolvida a outro módulo |
| ReadModel | projeção de leitura interna à Application |
| Entity/Value Object | estado e invariantes do Domain |

Não crie um tipo vazio apenas para satisfazer a nomenclatura. Crie uma fronteira
quando ela protege responsabilidade ou acoplamento real.

## Regras

- Entity nunca é devolvida por Controller ou fachada de módulo.
- Request e Response não entram na Application.
- Result não é devolvido diretamente como contrato entre módulos.
- Contracts não dependem de Domain, Application, Infrastructure ou ASP.NET Core.
- Snapshots contêm somente os dados necessários aos consumidores.
- Consultas em lote devem ser preferidas quando evitarem N+1 entre módulos.
- Ausência e resultados parciais precisam ter semântica explícita.
- Contratos assíncronos propagam `CancellationToken`.

## Evolução

Uma mudança em Contracts afeta consumidores internos e deve ser tratada como
evolução de API, mesmo dentro do monólito. Prefira alterações compatíveis e
remova campos somente depois de revisar todos os consumidores.

Eventos só devem ser introduzidos quando existirem produtor, consumidor,
transporte, entrega e política de idempotência definidos. Até lá, não crie tipos
de evento especulativos.

A lista atual de Queries, Snapshots e métodos deve ser consultada nos projetos
Contracts e nos `ProjectReference`, não duplicada aqui.

## Proteção

ArchitectureTests verifica que os tipos e referências respeitam as fronteiras.
Execute:

```powershell
dotnet test tests/Architecture/FiapCloudGames.ArchitectureTests
```
