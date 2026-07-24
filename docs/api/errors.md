# Erros da API

## Problem Details

Exceções tratadas pelo middleware retornam `application/problem+json` com formato
semelhante a:

```json
{
  "status": 422,
  "title": "Regra de negócio inválida",
  "detail": "O jogo já pertence à biblioteca.",
  "instance": "/api/library/games/22222222-2222-2222-2222-222222222222",
  "traceId": "00-..."
}
```

Os títulos exatos do middleware são `Não autenticado`, `Recurso não encontrado`,
`Requisição inválida`, `Regra de negócio inválida` e `Erro interno`. O detalhe
vem da exceção nos erros abaixo de 500; no erro interno ele é substituído por uma
mensagem segura. Respostas produzidas diretamente pelo controller ou autenticação
podem vir sem o mesmo corpo.

## Status relevantes

| Status | Significado no projeto |
|---:|---|
| 400 | binding ou argumento inválido |
| 401 | credencial ausente/inválida ou login recusado |
| 403 | papel insuficiente |
| 404 | rota ou recurso não encontrado |
| 422 | regra de negócio não atendida |
| 500 | falha inesperada |

Não há mapeamento atual para 409.

## Diagnóstico

1. Registre método, rota, status e horário.
2. Capture o `traceId`, se presente.
3. Não copie token, senha ou connection string.
4. Consulte os logs da mesma execução.
5. Verifique o estado do recurso e a regra documentada.

Veja [tratamento de erros](../development/error-handling.md) e
[troubleshooting](../operations/troubleshooting.md).
