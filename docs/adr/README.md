# Architecture Decision Records

Architecture Decision Record (ADR) registra uma decisão arquitetural relevante, seu contexto e suas consequências.

## Quando criar

Crie um ADR quando a mudança:

- altera fronteiras ou dependências entre módulos;
- escolhe ou substitui banco, framework ou protocolo;
- muda autenticação, autorização ou tratamento de erros;
- altera estratégia de migrations, deploy ou observabilidade;
- introduz compatibilidade ou custo de longo prazo.

Não use ADR para correção local sem impacto arquitetural.

## Numeração e nome

Use sequência de quatro dígitos:

```text
ADR-0008-titulo-curto.md
```

Não reutilize número removido. Referencie ADR substituído no documento novo.

## Status

- **Proposto**: em discussão;
- **Aceito**: decisão vigente e comprovada;
- **Substituído**: outra decisão tomou seu lugar;
- **Descontinuado**: não se aplica mais e não possui substituto direto.

Não reescreva silenciosamente uma decisão aceita. Atualize consequências menores ou crie novo ADR para mudança de direção.

## Decisões atuais

| ADR | Status | Tema |
|---|---|---|
| [0001](ADR-0001-modular-monolith.md) | Aceito | Monólito modular |
| [0002](ADR-0002-centralized-migrations.md) | Aceito | Migrations centralizadas |
| [0003](ADR-0003-module-communication.md) | Aceito | Comunicação por Contracts |
| [0004](ADR-0004-application-parts.md) | Aceito | Controllers via Application Parts |
| [0005](ADR-0005-identity-authentication.md) | Aceito | Autenticação no módulo Identity |
| [0006](ADR-0006-problem-details.md) | Aceito | Tratamento global com Problem Details |
| [0007](ADR-0007-development-openapi.md) | Aceito | OpenAPI somente em Development |

## Template

Copie [template.md](template.md), atribua o próximo número e preencha todas as seções.
