# Testes unitários

## Localização

Cada domínio possui um projeto xUnit:

- `FiapCloudGames.Identity.UnitTests`;
- `FiapCloudGames.Catalog.UnitTests`;
- `FiapCloudGames.Promotions.UnitTests`;
- `FiapCloudGames.Library.UnitTests`.

Os oito casos atuais verificam normalização de e-mail e dinheiro, mudança de
estado, rejeição de preço/período inválido, cálculo de desconto, prevenção de
duplicidade e snapshot de aquisição.

## Padrão atual

Os testes constroem entidades diretamente e seguem arrange/act/assert sem
dependência externa:

```csharp
[Fact]
public void CreateShouldRejectNegativePrice()
{
    Assert.Throws<InvalidOperationException>(() =>
        Game.Create("Cloud Quest", "Aventura", "RPG", -0.01m));
}
```

## Serviços e dependências

Não existe biblioteca de mocks instalada e os projetos unitários atuais
referenciam somente seus respectivos domínios. Portanto, o repositório ainda não
possui exemplo executável de mock para serviços de aplicação.

Ao adicionar esses testes:

1. referencie o projeto Application correspondente;
2. prefira fakes pequenos para repositórios e unidades de trabalho;
3. ou adote uma biblioteca de mocks por decisão explícita;
4. valide retorno, erro e quantidade/argumentos das chamadas.

Modelo conceitual:

```csharp
// Exemplo futuro: adapte aos contratos reais antes de adicionar ao projeto.
var repository = new FakeGameRepository(existingGame: null);
var unitOfWork = new SpyUnitOfWork();
var service = new GameService(repository, unitOfWork);

var created = await service.CreateAsync(request, CancellationToken.None);

Assert.Equal("Cloud Quest", created.Title);
Assert.Equal(1, repository.AddCalls);
Assert.Equal(1, unitOfWork.SaveChangesCalls);
```

Para a falha, configure o fake com um registro existente, confira a exceção
esperada e confirme que `AddCalls` e `SaveChangesCalls` continuam em zero. O
snippet é deliberadamente ilustrativo: `FakeGameRepository`, `SpyUnitOfWork` e a
assinatura mostrada não existem hoje.

## Boas práticas

- Um comportamento relevante por teste.
- Nomes que expressem cenário e resultado.
- Datas controladas quando a regra depende do tempo.
- Valores de fronteira e falhas junto ao caminho feliz.
- Nenhuma conexão, rede ou variável global em teste unitário.

`TODO: adicionar testes unitários para serviços Application e token/senha, com
fakes ou estratégia de mocks aprovada.`

