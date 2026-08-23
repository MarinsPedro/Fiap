# Módulos

Os módulos representam bounded contexts. Este documento registra
responsabilidades, fronteiras e regras duráveis; services, controllers e rotas
devem ser descobertos no código e no OpenAPI.

## Identity

Responsável por identidade, credenciais, autenticação, papéis e ciclo de vida do
usuário.

Regras que devem permanecer verdadeiras:

- e-mail é validado e normalizado;
- senha nunca é armazenada em texto puro;
- novos cadastros não recebem privilégios administrativos;
- usuário inativo não pode autenticar;
- acesso ao usuário atual ocorre por abstração da Application;
- dados internos não são expostos diretamente a outros módulos.

Identity oferece consultas internas por Contracts. Não depende das camadas
internas de outros módulos.

## Catalog

Responsável pelo cadastro consultável de jogos, seus dados descritivos, preço
base e disponibilidade.

Regras que devem permanecer verdadeiras:

- o agregado protege título, descrição, categoria e preço;
- preço base não pode ser negativo;
- operações públicas de catálogo respeitam a disponibilidade;
- consumidores recebem snapshots, não a entidade `Game`;
- alterações persistentes permanecem no schema de Catalog.

Catalog oferece consultas individuais e em lote por Contracts. Essas operações
evitam que consumidores acessem seu `DbContext`.

## Promotions

Responsável por vigência de promoções, associação com jogos e cálculo do melhor
desconto aplicável.

Regras que devem permanecer verdadeiras:

- período e percentual precisam formar uma promoção válida;
- jogos informados são validados pela fronteira pública de Catalog;
- sobreposição não quebra o cálculo: vence o maior desconto vigente;
- encerramento altera o estado da promoção sem compartilhar o agregado;
- consumidores recebem uma cotação imutável por Contracts.

Promotions depende apenas de Catalog.Contracts para consultar jogos.

## Library

Responsável por registrar a biblioteca do usuário e o snapshot de cada aquisição.

Regras que devem permanecer verdadeiras:

- usuário e jogo precisam estar disponíveis no momento da aquisição;
- a mesma biblioteca não recebe o mesmo jogo duas vezes;
- preço, promoção e instante são preservados como snapshot;
- título é enriquecido por consulta ao Catalog e não duplicado como dado mestre;
- aquisição não representa pagamento;
- Application obtém o usuário autenticado por `ICurrentUserContext`.

Library consome Contracts de Identity, Catalog e Promotions. Cada chamada é
independente; não existe transação distribuída entre esses módulos.

## Host HTTP

A API é o composition root. Ela registra módulos, adapta a identidade atual,
configura autenticação, autorização, serialização, CORS, logging, Problem Details,
OpenAPI e health checks.

A lista atual de operações HTTP e seus requisitos de autorização pertence ao
OpenAPI gerado em Development.

## Evolução

Uma mudança pertence a um módulo quando sua linguagem, regra e dado são daquele
bounded context. Integrações novas devem preservar Contracts ou explicitar uma
mudança de arquitetura.
