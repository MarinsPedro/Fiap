# Testes unitários

## Escopo

Cada módulo possui um projeto `*.UnitTests` dividido em:

```text
Domain/
Application/
```

Não são criados UnitTests para Infrastructure, Contracts ou Presentation.
Contracts podem ser usados apenas quando forem necessários para exercer uma
porta consumida pela Application.

## Domain

Testes de Domain validam:

- invariantes;
- limites e valores de fronteira;
- objetos de valor;
- transições de estado;
- cálculos;
- proteção do agregado;
- comportamento temporal com instante controlado.

Eles usam objetos reais e não conhecem repositories, HTTP, EF Core ou mocks de
infraestrutura.

## Application

Testes de Application validam:

- decisões do caso de uso;
- autenticação e autorização da aplicação;
- coordenação entre portas;
- retorno e erros semânticos;
- efeitos relevantes;
- ausência de persistência quando o fluxo falha.

Repositories e integrações são tratados como portas externas e substituídos por
fakes, stubs ou spies pequenos. Interações só são verificadas quando representam
um efeito relevante do caso de uso.

Alterações internas de persistência não devem exigir mudanças nesses testes.

## Dependências permitidas

Projetos unitários podem referenciar:

- Domain do módulo;
- Application do módulo;
- Contracts necessários;
- BuildingBlocks necessários;
- bibliotecas de teste.

Eles não podem referenciar:

- Infrastructure;
- Presentation;
- API;
- Database.Migrations;
- EF Core ou `DbContext`;
- `WebApplicationFactory`.

Essas restrições são fiscalizadas por `ArchitectureTests`.

## Padrão de escrita

- Nome: `Metodo_Cenario_ResultadoEsperado`.
- Arrange, Act e Assert visíveis.
- Um comportamento relevante por teste.
- Datas fixas ou `TimeProvider` controlado.
- Caminho feliz, fronteiras e falhas significativas.
- Sem rede, banco, filesystem ou variável global.
- Sem assertions sobre detalhes privados, logger ou implementação do repository.

## Fakes e spies

Um fake deve implementar somente o comportamento necessário para o cenário. Um
spy registra apenas efeitos que façam parte do contrato do caso de uso, como
adicionar um agregado ou confirmar a unidade de trabalho.

Evite criar uma infraestrutura de testes genérica prematuramente. Compartilhe um
test double apenas quando a repetição trouxer custo real e a abstração continuar
simples.
