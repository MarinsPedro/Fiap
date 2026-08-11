# ADR-0006: Tratamento global com Problem Details

## Status

Aceito e revisado.

## Contexto

Uma exceção por situação funcional aumenta o acoplamento e obriga o middleware
a conhecer regras dos módulos. Exceções genéricas, por outro lado, não carregam
semântica HTTP confiável.

Validações com vários campos precisam conservar uma estrutura diferente no
corpo HTTP, mas não exigem uma segunda classe de exceção.

## Decisão

- Manter uma única `AppException` em `Application.Common`.
- Representar a semântica por `AppErrorCategory`.
- Armazenar opcionalmente erros por campo dentro da própria `AppException`.
- Usar somente as categorias `Validation`, `Authentication`, `Forbidden`,
  `NotFound`, `Conflict` e `BusinessRule`.
- Converter a categoria em status, título e código apenas no middleware da API.
- Mapear qualquer outra exceção para 500 com detalhe sanitizado.
- Ignorar `OperationCanceledException` quando o cliente cancelou a requisição.
- Não definir URLs próprias para o campo opcional `type`.
- Usar `IProblemDetailsService` e `ValidationProblemDetails`.

## Limites

Somente serviços da camada Application lançam `AppException`. Entidades do
domínio não referenciam `Application.Common`; elas mantêm suas próprias
invariantes.

`UseStatusCodePages` permanece responsável por completar respostas vazias do
framework, como challenge, forbid e 404 de rota.

## Consequências

- o middleware não conhece Identity, Catalog, Library ou Promotions;
- novas regras reutilizam categorias existentes;
- todos os erros funcionais compartilham uma abstração;
- validação estruturada continua disponível por `Errors`;
- cancelamentos do cliente não poluem os logs como erro interno;
- exceções técnicas não são disfarçadas como falhas do cliente;
- testes de Application verificam categoria e mensagem;
- testes HTTP verificam o mapeamento para status e Problem Details.

## Alternativas consideradas

Uma exceção por situação, catálogos por módulo e uma exceção separada apenas
para validação foram rejeitados por aumentarem a estrutura sem benefício para o
contrato. Mapear exceções genéricas por tipo foi rejeitado por esconder bugs.
