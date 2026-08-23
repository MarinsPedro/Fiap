# Pendências consolidadas

Este arquivo concentra decisões ainda não formalizadas. Os demais documentos
devem apontar para estes identificadores em vez de repetir listas locais.

Quando existir um issue tracker oficial, estas pendências devem ser migradas para
ele e este arquivo deve passar a apontar para as issues correspondentes.

| ID | Tema | Decisão necessária |
|---|---|---|
| `DOC-001` | governança do repositório | acesso, branch principal, commits, revisão, responsáveis, suporte e ferramenta de gestão |
| `DOC-002` | evolução da API | versionamento, compatibilidade, paginação, filtros, ordenação e idempotência HTTP |
| `DOC-003` | comércio digital | moeda, impostos, pagamento, cancelamento, reembolso e compensação financeira |
| `DOC-004` | consistência | concorrência, eventos, outbox, compensação e consistência entre módulos |
| `DOC-005` | segurança | ciclo de vida de credenciais, recuperação de senha, MFA, refresh/revogação de JWT e rotação de segredos |
| `DOC-006` | observabilidade | OpenTelemetry, métricas, tracing, retenção, alertas, SLI, SLO e health checks de dependências |
| `DOC-007` | entrega e operação | CI/CD, ambientes, registry, deploy, backup, restore e rollback |
| `DOC-008` | ambiente de desenvolvimento | sistemas operacionais, IDEs e versões mínimas de Docker/Compose suportadas |
| `DOC-009` | estratégia de testes | PostgreSQL real, isolamento, ciclo de vida e testes de migrations/SQL |
| `DOC-010` | ciclo de vida do produto | reativação de usuários/jogos e alteração ou remoção de promoções |
| `DOC-011` | automação documental | lint de Markdown, links, line endings e validação/geração de artefatos dinâmicos |

## Regras de manutenção

- Não duplique a mesma pendência em vários documentos.
- Adicione uma pendência somente quando houver uma decisão concreta a tomar.
- Remova o item quando a decisão estiver implementada e documentada.
- Registre mudanças relevantes de comportamento no `CHANGELOG.md`.
