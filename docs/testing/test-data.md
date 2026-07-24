# Dados de teste

## Estado atual

Os testes unitários criam seus dados dentro do próprio caso com valores
determinísticos ou `Guid.NewGuid()`. O teste de API define variáveis de ambiente
de teste no `WebApplicationFactory`. O teste de mapeamento usa uma connection
string fictícia e não abre conexão.

Não existem fixtures compartilhadas de domínio, builders, snapshots, dumps de
banco ou massa versionada.

## Princípios

- Dados mínimos para expressar o cenário.
- Identificadores únicos quando houver persistência.
- Datas fixas ou relógio controlável para regras temporais.
- Sem e-mails pessoais, tokens ou credenciais reais.
- Senhas de exemplo identificadas explicitamente como teste/local.
- Limpeza previsível para testes que criarem recursos externos.

## Exemplo de cenário

```csharp
var now = new DateTimeOffset(2026, 7, 23, 12, 0, 0, TimeSpan.Zero);
var gameId = Guid.Parse("11111111-1111-1111-1111-111111111111");
var promotion = Promotion.Create(
    "Promoção de teste",
    25m,
    now.AddHours(-1),
    now.AddHours(1),
    [gameId]);
```

Datas fixas tornam o caso repetível. Evite usar `UtcNow` quando o instante fizer
parte do resultado esperado.

## Futuro banco efêmero

Quando houver teste PostgreSQL:

1. inicie uma instância isolada;
2. aplique migrations;
3. gere dados por API ou builders oficiais;
4. execute o cenário;
5. limpe banco/volume;
6. nunca reutilize o banco de desenvolvimento.

`TODO: criar builders somente quando a repetição justificar a abstração e definir
uma interface de relógio para regras temporais.`

