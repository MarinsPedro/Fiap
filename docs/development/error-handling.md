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

| Categoria | Status | Tipo |
|---|---:|---|
| `Validation` | 400 | `validation` |
| `Authentication` | 401 | `unauthorized` |
| `Forbidden` | 403 | `forbidden` |
| `NotFound` | 404 | `not-found` |
| `Conflict` | 409 | `conflict` |
| `BusinessRule` | 422 | `business-rule` |

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

Quando `Errors` possui itens, o middleware acrescenta a extensão `errors` ao
mesmo `ApiProblemDetails` usado pelas demais falhas:

```json
{
  "type": "urn:fiap-cloud-games:problem:validation",
  "title": "Um ou mais dados são inválidos",
  "status": 400,
  "detail": "Verifique os dados informados.",
  "traceId": "18904cfedc6a6bcb08f53c175daec39d",
  "errors": [
    {
      "message": "O título é obrigatório.",
      "field": "title"
    },
    {
      "message": "O preço não pode ser negativo.",
      "field": "basePrice"
    }
  ]
}
```

Erros automáticos de JSON e `ModelState` usam o mesmo tipo e formato. Em JSON
malformado, a resposta contém somente uma mensagem genérica, sem `field`.

## Middleware

`ExceptionHandlingMiddleware` conhece apenas:

```text
DomainRuleViolationException → 422 business-rule
AppException                 → categoria convertida em HTTP
Exception                    → 500 sanitizado
```

Cancelamentos iniciados pelo cliente não são convertidos em 500. Antes de
escrever uma falha, o middleware limpa a resposta ainda não iniciada.

Exceções genéricas como `InvalidOperationException`, `KeyNotFoundException`,
`ArgumentException` e `UnauthorizedAccessException` não recebem significado
funcional e permanecem como 500.

As respostas 4xx são registradas uma única vez pelo
`ClientErrorLoggingMiddleware`: 400, 401, 404, 409 e 422 em `Information`; 403 e
429 em `Warning`. O log contém metadados HTTP, mas não inclui o corpo, mensagens
de validação ou valores informados pelo cliente. Falhas 500 continuam em
`Error`, com a exceção completa, enquanto o cliente recebe somente
`Não foi possível concluir a operação.`.

O campo `type` utiliza URNs próprios, estáveis e sem códigos específicos por
regra. Uma migração para URLs deve ocorrer somente quando houver documentação
pública e permanente para os tipos.

## Limite arquitetural

`AppException` pertence à Application. Entidades do domínio não dependem de
`Application.Common`; suas invariantes internas continuam usando validação
própria e lançam `DomainRuleViolationException`. A API traduz essa exceção em
`422 Unprocessable Entity` sem conhecer regras específicas dos módulos.

`UseStatusCodePages` complementa respostas vazias produzidas por challenge,
forbid e rota inexistente.

Consulte também [Erros da API](../api/errors.md) e
[Validação](validation.md).
