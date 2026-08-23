# Restrições e trade-offs arquiteturais

Este documento consolida restrições que orientam mudanças. Ele não substitui os
testes arquiteturais nem mantém um histórico formal de decisões.

## Restrições vigentes

| Restrição | Motivo | Fonte verificável |
|---|---|---|
| um host HTTP compõe módulos separados | reduzir custo operacional preservando bounded contexts | solution e projetos da API |
| comunicação entre módulos usa Contracts | impedir vazamento de entidades e implementações | referências de projeto e ArchitectureTests |
| Domain não depende de frameworks externos | preservar regras executáveis fora de HTTP e persistência | referências e ArchitectureTests |
| Application depende de abstrações | separar casos de uso de EF Core e ASP.NET Core | projetos Application |
| Presentation adapta HTTP | manter semântica de transporte fora da Application | controllers e mappings |
| migrations são executadas fora da API | não conceder evolução de schema ao processo HTTP | migrador e host |
| Problem Details é o contrato global de erro | oferecer respostas previsíveis sem expor falhas internas | middlewares e testes de integração |
| OpenAPI é exposto apenas em Development | reduzir a superfície exposta em Production | configuração do host |

Quando uma restrição puder ser automatizada, o teste é a proteção executável e
este texto explica sua intenção.

## Trade-offs conhecidos

- chamadas entre módulos compartilham processo e falham de forma síncrona;
- verificações cruzadas podem sofrer janela de concorrência;
- cada `DbContext` confirma sua própria transação;
- não há outbox, broker ou compensação automática;
- erros específicos de concorrência do banco não possuem tradução completa;
- o health check mede o processo e não a prontidão das dependências;
- os testes de banco não exercitam SQL contra PostgreSQL real.
