# Testes de arquitetura

O projeto `FiapCloudGames.ArchitectureTests` usa NetArchTest para tornar algumas
fronteiras executáveis.

## Regras atuais

1. Domain não depende de frameworks nem das camadas externas.
2. Application não depende de Infrastructure ou Presentation.
3. Services da Application não devolvem tipos de Contracts.
4. Infrastructure não depende de Presentation.
5. Presentation não referencia Contracts diretamente.
6. Actions não devolvem tipos de Application ou Contracts.
7. Contracts não depende de implementações, Domain ou frameworks.
8. Tipos de Contracts são `Query`, `Snapshot` ou interfaces `I*Module`.
9. As quatro raízes não têm construtores públicos.
10. Entidades de domínio não expõem setters públicos.
11. Library.Application usa apenas Contracts dos demais módulos.
12. Migrations pode referenciar Infrastructure, mas não Presentation.

```powershell
dotnet test tests/Architecture/FiapCloudGames.ArchitectureTests
```

## Ao evoluir a arquitetura

- Adicione um teste para uma regra estrutural objetiva.
- Evite testes frágeis baseados apenas em nomes quando uma relação de dependência
  puder ser inspecionada.
- Atualize [dependências](../architecture/dependencies.md).
- Registre um [ADR](../adr/README.md) se a mudança alterar uma decisão durável.

## Lacunas

- não há regra estrutural para nomes de repositories;
- a matriz de referências de projeto ainda pode ser complementada por análise
  direta dos `.csproj`.
