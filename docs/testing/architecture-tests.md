# Testes de arquitetura

O projeto `FiapCloudGames.ArchitectureTests` usa NetArchTest para tornar algumas
fronteiras executáveis.

## Regras atuais

1. Cada um dos quatro assemblies Domain não pode depender de ASP.NET Core,
   Entity Framework Core ou Infrastructure.
2. Library.Application não pode depender de Application ou Infrastructure dos
   demais módulos; a comunicação deve ocorrer por Contracts.
3. O assembly de migrations não pode referenciar assemblies dos módulos.

Como a primeira regra é uma teoria executada para quatro assemblies, são seis
execuções no total.

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

- Não há teste impedindo referência direta entre todas as combinações de módulos.
- Não há regra para garantir a direção completa Domain → Application →
  Infrastructure/API.
- Não há regra para convenções de controllers, handlers ou repositórios.

`TODO: ampliar a matriz de dependências permitidas sem bloquear referências
legítimas a Contracts.`

