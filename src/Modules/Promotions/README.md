# Módulo Promotions

## Responsabilidade

Promotions é responsável por campanhas, período de vigência, associação com
jogos e cálculo do melhor desconto aplicável.

## Fronteira

O agregado de promoção permanece interno. Outros módulos recebem apenas cotações
imutáveis por Promotions.Contracts.

## Regras duráveis

- percentual precisa estar no intervalo aceito pelo domínio;
- término precisa ocorrer depois do início;
- a promoção abrange ao menos um jogo distinto;
- jogos são validados pela fronteira de Catalog;
- atividade depende de período e encerramento;
- em sobreposição, aplica-se o maior desconto vigente;
- preço calculado respeita a precisão monetária do domínio.

## Integrações

Promotions depende apenas de Catalog.Contracts para consultar jogos. Library usa
Promotions.Contracts para obter a cotação vigente. Não há evento, broker ou
outbox.

## API e testes

Operações administrativas e públicas atuais devem ser consultadas no OpenAPI. A
suíte do módulo pode ser executada com:

```powershell
dotnet test tests/Unit/FiapCloudGames.Promotions.UnitTests
```

Consulte [regras de negócio](../../../docs/development/business-rules.md).
Pendências de ciclo de vida e consistência estão em
[DOC-004 e DOC-010](../../../docs/backlog.md).
