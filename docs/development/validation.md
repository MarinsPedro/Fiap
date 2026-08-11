# Validação

## Onde ocorre

A validação é distribuída em quatro níveis:

1. `[ApiController]` e Data Annotations validam binding e contratos HTTP.
2. Serviços de Application validam regras específicas do caso de uso, como
   política de senha, existência, autorização e conflitos.
3. Entidades e value objects protegem invariantes internas sem depender de
   Application.
4. Mapeamentos e migrations aplicam limites e constraints no banco.

## Respostas

| Origem | Resultado | HTTP |
|---|---|---:|
| JSON ou `ModelState` inválido | `ValidationProblemDetails` | 400 |
| entrada inválida no caso de uso | `AppException.Validation` | 400 |
| credenciais inválidas | `AppException.Authentication` | 401 |
| identidade sem permissão | `AppException.Forbidden` | 403 |
| recurso ausente | `AppException.NotFound` | 404 |
| duplicidade/conflito | `AppException.Conflict` | 409 |
| regra de negócio inválida | `AppException.BusinessRule` | 422 |
| invariante de domínio violada | `DomainRuleViolationException` | 422 |
| erro não tratado | Problem Details sanitizado | 500 |

## Vários erros por campo

Validadores devem acumular falhas independentes antes de interromper a
operação:

```csharp
if (errors.Count > 0)
{
    throw AppException.Validation(
        errors.ToDictionary(
            item => item.Key,
            item => item.Value.ToArray(),
            StringComparer.OrdinalIgnoreCase));
}
```

`AppException.Errors` é somente leitura e `HasErrors` informa se o middleware
deve produzir `ValidationProblemDetails`.

## Value object `Email`

`Email.Create` protege a invariável do domínio. Para fluxos em que formato
inválido é esperado, como login, use `Email.TryCreate`.

O endereço é normalizado com `Trim()` e `ToLowerInvariant()`, validado com
`MailAddress.TryCreate` e limitado a 254 caracteres. Cadastro e login usam o
mesmo valor normalizado; a unicidade continua garantida pelo banco.

## Ao adicionar uma validação

- Use chaves em `camelCase` e mensagens claras.
- Preserve invariantes no domínio e constraints no banco.
- Use uma das categorias existentes de `AppException`.
- Não adicione uma categoria para cada regra de negócio.
- Não converta exceções técnicas em erros do cliente.
- Cubra categoria, mensagem e erros estruturados nos testes.

Veja [tratamento de erros](error-handling.md) e
[erros da API](../api/errors.md).
