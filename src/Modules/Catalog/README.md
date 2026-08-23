# Módulo Catalog

## Responsabilidade

Catalog é responsável pelo cadastro consultável de jogos, dados descritivos,
categoria, preço base e disponibilidade.

## Fronteira

Catalog é a fonte de dados mestre do jogo. Outros módulos consultam snapshots por
Contracts e não acessam sua entidade, repository ou persistência.

## Regras duráveis

- título e categoria são normalizados e validados;
- descrição respeita o limite do domínio;
- preço base não pode ser negativo;
- jogos novos iniciam disponíveis;
- operações públicas de catálogo respeitam o estado do jogo;
- resultados de leitura não expõem entidades rastreadas;
- alterações persistentes permanecem no schema de Catalog.

## Integrações

Promotions consulta jogos para validar a abrangência de promoções. Library
consulta jogos durante aquisição e leitura. Ambas dependem apenas de
Catalog.Contracts.

## API e testes

O OpenAPI é a fonte de verdade para operações e contratos HTTP. A suíte de Domain
e Application pode ser executada com:

```powershell
dotnet test tests/Unit/FiapCloudGames.Catalog.UnitTests
```

Consulte [regras de negócio](../../../docs/development/business-rules.md).
Pendências de escala e ciclo de vida estão em
[DOC-002 e DOC-010](../../../docs/backlog.md).
