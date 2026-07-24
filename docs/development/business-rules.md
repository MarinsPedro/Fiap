# Regras de negócio

Este documento registra apenas regras observáveis no código atual. Limites impostos
exclusivamente pelo banco também são identificados, pois podem produzir um erro de
persistência em vez de um erro de domínio.

## Identity

- O e-mail é obrigatório, normalizado para minúsculas e validado antes do cadastro.
- O nome deve ter entre 2 e 120 caracteres.
- A senha de cadastro deve ter pelo menos 8 caracteres.
- Não podem existir dois usuários com o mesmo e-mail.
- Novos usuários recebem o papel `User`.
- O usuário administrador só é criado pelo `AdminSeeder`; não há endpoint público
  para elevar privilégios.
- A inativação de um usuário já inativo não altera novamente o estado.
- Um usuário inativo não consegue autenticar.
- A senha é armazenada com PBKDF2/SHA-256, 100.000 iterações, salt de 16 bytes e
  hash de 32 bytes.

## Catalog

- O título do jogo deve ter entre 2 e 160 caracteres.
- A categoria é obrigatória. O banco limita a coluna a 80 caracteres.
- O preço não pode ser negativo e é arredondado para duas casas decimais.
- A descrição é normalizada: espaços externos são removidos e `null` vira texto
  vazio. O banco limita a coluna a 4.000 caracteres.
- Um jogo é criado ativo.
- A listagem pública retorna somente jogos ativos, em ordem alfabética de título.

## Promotions

- O nome deve ter entre 2 e 120 caracteres.
- O desconto deve ser maior que zero e menor ou igual a 100, arredondado para duas
  casas decimais.
- O fim da promoção deve ser posterior ao início.
- Uma promoção deve conter pelo menos um jogo; IDs repetidos são eliminados.
- No cadastro, todos os jogos informados precisam existir e estar ativos.
- Uma promoção está ativa quando `StartedAt <= agora < EndsAt` e `EndedAt` é nulo.
- Encerrar uma promoção já encerrada é uma operação idempotente no domínio.
- Quando mais de uma promoção ativa alcança um jogo, é aplicado o maior desconto.
- O preço com desconto é arredondado para duas casas decimais.
- Não existe regra que impeça sobreposição de promoções.

## Library

- Usuário e jogo precisam existir e estar ativos no momento da aquisição.
- Cada usuário possui uma única biblioteca.
- O mesmo jogo não pode ser adicionado duas vezes à mesma biblioteca.
- O preço pago não pode ser negativo e é arredondado para duas casas decimais.
- Título e preço são gravados como snapshot no item da biblioteca.
- Se o título retornado pelo catálogo estiver ausente, a aplicação usa um texto de
  fallback.
- A aquisição não executa pagamento, reserva, estorno ou integração financeira.

## Consistência entre módulos

Os módulos consultam contratos de aplicação uns dos outros. Como cada
`DbContext` possui sua própria unidade de trabalho, uma operação que toca mais de
um módulo não possui transação distribuída. Eventos de domínio estão declarados
em contratos, mas ainda não são publicados.

`TODO: definir estratégia de consistência, publicação de eventos e compensação para
operações que atravessem módulos.`

## Pontos ainda não definidos

- `TODO: definir moeda e política de impostos para preços.`
- `TODO: definir política de reativação de usuário e jogo.`
- `TODO: definir política de concorrência para aquisições simultâneas.`
- `TODO: definir regras de cancelamento, reembolso e pagamento.`
- `TODO: definir se promoções futuras podem ser alteradas ou removidas.`

