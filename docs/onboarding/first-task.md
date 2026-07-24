# Primeira tarefa guiada

Exercício seguro: documentar com teste o comportamento existente que remove espaços do título e da categoria de um jogo.

Essa tarefa não muda a API nem o banco. Ela ensina localização, Domain, testes, documentação e validação.

## 1. Localizar o módulo

O comportamento pertence a Catalog:

```text
src/Modules/Catalog/FiapCloudGames.Catalog.Domain/Entities/Game.cs
tests/Unit/FiapCloudGames.Catalog.UnitTests/GameTests.cs
src/Modules/Catalog/README.md
```

Leia `Game.Update`: `title.Trim()` e `category.Trim()` já existem.

## 2. Criar o teste

Adicione a `GameTests`:

```csharp
[Fact]
public void CreateShouldTrimTitleAndCategory()
{
    // Arrange e Act
    var game = Game.Create("  Cloud Quest  ", "Aventura", "  RPG  ", 99.90m);

    // Assert
    Assert.Equal("Cloud Quest", game.Title);
    Assert.Equal("RPG", game.Category);
}
```

O teste usa o padrão atual: xUnit, sem biblioteca de mocks e teste direto de entidade.

## 3. Executar o projeto de teste

```powershell
dotnet test tests/Unit/FiapCloudGames.Catalog.UnitTests/FiapCloudGames.Catalog.UnitTests.csproj
```

Resultado esperado: todos os testes do projeto aprovados.

## 4. Atualizar documentação

Confirme que `src/Modules/Catalog/README.md` menciona a normalização. Se não mencionar, adicione a regra sem copiar todo o guia central.

## 5. Validar a solution

```powershell
dotnet build FiapCloudGames.sln
dotnet test FiapCloudGames.sln --no-build
dotnet format FiapCloudGames.sln --verify-no-changes --no-restore
```

## 6. Preparar o pull request

Siga [CONTRIBUTING.md](../../CONTRIBUTING.md). No PR, descreva:

- comportamento documentado pelo teste;
- arquivo alterado;
- comando e resultado;
- ausência de impacto em API/banco.

```text
TODO: branch base, convenção de nome e aprovadores devem ser confirmados com a equipe.
```

## Checklist

- [ ] Localizei Domain, teste e README do Catalog.
- [ ] Li a implementação antes de escrever o teste.
- [ ] O teste falha se a normalização for removida.
- [ ] O projeto de teste passou.
- [ ] A solution compilou e todos os testes passaram.
- [ ] A documentação do módulo está coerente.
- [ ] Nenhum arquivo gerado, segredo ou alteração sem relação foi incluído.
- [ ] O PR informa resultados reais.
