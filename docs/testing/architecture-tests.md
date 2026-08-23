# Testes de arquitetura

## Objetivo

`FiapCloudGames.ArchitectureTests` funciona como barreira automática contra
dependências e estruturas que violam a arquitetura modular.

## Descoberta

Projetos e assemblies são descobertos por convenções como:

```text
FiapCloudGames.*.Domain
FiapCloudGames.*.Application
FiapCloudGames.*.Infrastructure
FiapCloudGames.*.Presentation
FiapCloudGames.*.Contracts
```

Aggregate roots são inferidos a partir das entidades expostas pelas interfaces
de repository do Domain. Entidades, controllers, services e projetos unitários
também são encontrados automaticamente.

Um novo módulo que segue a estrutura entra nas regras por padrão.

## Fronteiras protegidas

- Domain permanece independente de camadas externas e frameworks proibidos.
- Um Domain não acessa o Domain de outro módulo.
- Application não depende de Infrastructure, Presentation ou API.
- Comunicação entre módulos ocorre por Contracts.
- Infrastructure permanece dentro do próprio módulo.
- Presentation não depende de Domain, Infrastructure ou Contracts.
- Controllers não expõem retornos internos de Application/Contracts.
- Contracts permanecem independentes de implementações e frameworks web/dados.
- Entidades não possuem setters públicos.
- Aggregate roots não possuem construtores públicos.
- Migrations não dependem de Presentation.
- `ProjectReference` de produção segue a matriz arquitetural.

## Proteção dos UnitTests

Os projetos `*.UnitTests` são descobertos na pasta `tests/Unit`. Regras
estruturais impedem referências ou usos de:

- Infrastructure;
- Presentation;
- API e `WebApplicationFactory`;
- Database.Migrations;
- EF Core e `DbContext`.

Assim, uma mudança na persistência ou no host não pode acoplar os testes de
Application a detalhes externos.

## Convenções e limitações

As regras dependem da convenção de nomes dos projetos e assemblies. Alterar a
estrutura `FiapCloudGames.<Modulo>.<Camada>` exige atualizar a decisão
arquitetural e os testes que a fiscalizam.

Testes baseados em nomes de `Service`, `Controller`, `Contracts` ou namespaces
devem ser mantidos apenas quando o nome representar uma convenção oficial da
solução.

## Evolução

Adicione uma regra somente para uma restrição estrutural objetiva. Mudanças
duráveis na matriz de dependências devem ser refletidas na documentação de
arquitetura e, quando necessário, em um ADR.
