# ADR-0007: OpenAPI somente em Development

## Status

Aceito.

## Contexto

A especificação facilita desenvolvimento, mas o host de produção não deve expô-la sem decisão explícita.

## Decisão

Registrar os serviços do Swagger no container e habilitar `UseSwagger()` e
`UseSwaggerUI()` somente quando `app.Environment.IsDevelopment()`. O documento
usa a rota padrão `/swagger/v1/swagger.json` e a interface usa
`/swagger/index.html`.

## Consequências

- perfis locais `http` e `https` abrem a Swagger UI pelo `launchUrl` `swagger`;
- Docker Compose usa Production e não expõe o JSON nem a interface;
- o Swagger UI é disponibilizado somente em `Development`;
- consumidores externos precisam obter a especificação em Development ou por artefato gerado fora do fluxo atual.

## Alternativas consideradas

Exposição autenticada em produção e geração estática no pipeline não foram implementadas.
