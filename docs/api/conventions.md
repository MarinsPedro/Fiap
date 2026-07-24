# Convenções da API

## Rotas

As rotas de negócio usam o prefixo `/api`. O health check usa `/health`. IDs são
GUIDs no segmento de rota e uma rota que não atende à constraint `:guid` resulta
em ausência de correspondência.

Não existe prefixo de versão. Alterações incompatíveis exigem uma estratégia
ainda não definida.

`TODO: definir política de versionamento e compatibilidade da API.`

## JSON

O serializador padrão do ASP.NET Core produz propriedades camelCase. Enums são
representados por texto por meio de `JsonStringEnumConverter`.

Datas são `DateTimeOffset` em UTC e aparecem no formato ISO 8601:

```json
{
  "startsAtUtc": "2026-07-23T12:00:00+00:00"
}
```

Valores monetários e percentuais são números JSON, não strings. A moeda não está
definida pelo contrato atual.

## Métodos e status

- `GET` retorna 200 ou 404 quando aplicável.
- `POST` de criação retorna 201.
- `PUT` atualiza o recurso completo aceito pelo contrato.
- `DELETE` de usuário apenas inativa e retorna 204.
- O encerramento de promoção usa `POST .../end` e retorna 204.

## Coleções

As listagens retornam arrays sem envelope, cursor ou metadados. Não há paginação,
filtro fornecido pelo cliente ou ordenação configurável. A lista de jogos retorna
somente ativos em ordem de título; promoções retornam as ativas conforme a regra
temporal.

`TODO: definir limites e paginação antes de crescimento significativo da base.`

## Idempotência

Não existe suporte a `Idempotency-Key`. Inativar novamente e encerrar novamente
são idempotentes no domínio, mas criação de usuário, jogo, promoção e aquisição
não têm uma chave de repetição HTTP.

## Headers

```http
Content-Type: application/json
Authorization: Bearer <token>
```

O header de autorização só é necessário nas rotas protegidas.

