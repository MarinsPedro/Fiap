# Tratamento de erros

## Estrutura

Toda falha funcional da camada Application usa uma única exceção:

```text
FiapCloudGames.Application.Common
└── Exceptions
    ├── AppErrorCategory.cs
    └── AppException.cs
```

`AppErrorCategory` contém somente categorias HTTP estáveis. Uma nova regra de
negócio utiliza uma categoria existente e não exige outra exceção, código ou
alteração no middleware.

| Categoria | Status | Código |
|---|---:|---|
| `Validation` | 400 | `validation_error` |
| `Authentication` | 401 | `authentication_error` |
| `Forbidden` | 403 | `forbidden` |
| `NotFound` | 404 | `not_found` |
| `Conflict` | 409 | `conflict` |
| `BusinessRule` | 422 | `business_rule_violation` |

Exemplos:

```csharp
throw AppException.NotFound(
    "O jogo informado não foi encontrado.");

throw AppException.BusinessRule(
    "A promoção não pode ser aplicada a um jogo inativo.");
```

## Validação estruturada

`AppException.Validation(errors)` recebe várias mensagens por campo. A mesma
exceção também pode representar uma validação simples com
`AppException.Validation(message)`.

Quando `Errors` possui itens, o middleware produz `ValidationProblemDetails`:

```json
{
  "title": "Um ou mais dados são inválidos",
  "status": 400,
  "detail": "Um ou mais dados informados são inválidos.",
  "instance": "/api/games",
  "code": "validation_error",
  "traceId": "00-...",
  "errors": {
    "title": [
      "O título é obrigatório."
    ],
    "basePrice": [
      "O preço não pode ser negativo."
    ]
  }
}
```

Erros automáticos de JSON e `ModelState` usam o mesmo código e formato.

## Middleware

`ExceptionHandlingMiddleware` conhece apenas:

```text
DomainRuleViolationException → 422 domain_rule_violation
AppException                 → categoria convertida em HTTP
Exception                    → 500 sanitizado
```

Cancelamentos iniciados pelo cliente não são convertidos em 500. Antes de
escrever uma falha, o middleware limpa a resposta ainda não iniciada.

Exceções genéricas como `InvalidOperationException`, `KeyNotFoundException`,
`ArgumentException` e `UnauthorizedAccessException` não recebem significado
funcional e permanecem como 500.

Falhas funcionais são registradas em nível Information, sem stack trace. Falhas
500 são registradas em Error com a exceção completa, mas o cliente recebe apenas
`Ocorreu um erro interno inesperado.`.

O campo opcional `type` não utiliza URLs próprias fictícias; quando presente,
fica sob responsabilidade do writer padrão do ASP.NET Core.

## Limite arquitetural

`AppException` pertence à Application. Entidades do domínio não dependem de
`Application.Common`; suas invariantes internas continuam usando validação
própria e lançam `DomainRuleViolationException`. A API traduz essa exceção em
`422 Unprocessable Entity` sem conhecer regras específicas dos módulos.

`UseStatusCodePages` complementa respostas vazias produzidas por challenge,
forbid e rota inexistente.

Consulte também [Erros da API](../api/errors.md) e
[Validação](validation.md).
