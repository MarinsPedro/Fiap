# Validação

## Onde ocorre

O projeto não usa FluentValidation nem uma camada dedicada de validadores. A
validação atual está distribuída em quatro níveis:

1. `[ApiController]` valida binding e anotações dos contratos HTTP.
2. Serviços de aplicação validam existência, unicidade e permissões de negócio.
3. Entidades e value objects protegem invariantes do domínio.
4. Mapeamentos e migrations aplicam limites e constraints no banco.

Uma entrada inválida deve ser rejeitada o mais perto possível de sua fronteira,
mas toda invariável crítica também precisa ser protegida no domínio ou no banco.

## Respostas atuais

| Origem | Exceção/resultado | HTTP |
|---|---|---:|
| Binding de `[ApiController]` | `ModelState` inválido | 400 |
| Regra expressa como `ArgumentException` | Problem Details | 400 |
| Recurso ausente (`KeyNotFoundException`) | Problem Details | 404 |
| Estado de negócio inválido (`InvalidOperationException`) | Problem Details | 422 |
| Autenticação/autorização | challenge/forbid | 401/403 |
| Erro não tratado | Problem Details | 500 |

Algumas actions retornam `NotFound`, `Unauthorized` ou `Forbid` diretamente, de
modo que o formato pode não ser idêntico ao gerado pelo middleware.

## Limites a observar

Há limites que aparecem no domínio e outros apenas no banco. Por exemplo,
categoria e descrição do jogo possuem tamanho máximo no mapeamento/migration, mas
o domínio não aplica todos esses limites. Sem validação antecipada, o erro pode
aparecer apenas no `SaveChanges`.

`TODO: consolidar os limites dos contratos, domínio e banco e adicionar testes de
fronteira para cada campo.`

## Ao adicionar uma validação

- Use mensagens que expliquem o campo e a regra sem expor detalhes internos.
- Não dependa somente do controller; serviços também podem ser chamados por outra
  entrada no futuro.
- Preserve invariantes dentro da entidade/value object.
- Mantenha constraints para integridade e concorrência.
- Cubra valor válido, limites e falhas esperadas.
- Atualize [regras de negócio](business-rules.md) e
  [erros da API](../api/errors.md).

## Lacunas

- Não há padrão único de códigos de erro por campo.
- Não há localização de mensagens.
- Não há validação assíncrona dedicada.
- Violações de unicidade do banco não são convertidas em `409 Conflict`.

