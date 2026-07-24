# ADR-0004: Controllers carregados por Application Parts

## Status

Aceito.

## Contexto

Controllers pertencem aos projetos Presentation dos módulos, não ao executável da API. O MVC precisa descobri-los em assemblies externos.

## Decisão

Chamar `AddApplicationPart` para os assemblies identificados por:

- `IdentityPresentationAssemblyReference`;
- `CatalogPresentationAssemblyReference`;
- `LibraryPresentationAssemblyReference`;
- `PromotionsPresentationAssemblyReference`.

## Consequências

- Controllers permanecem junto ao módulo;
- adicionar módulo Presentation exige novo Application Part no `Program.cs`;
- esquecer o registro faz as rotas não aparecerem.

## Alternativas consideradas

Manter Controllers na API não é o padrão atual. Descoberta automática de assemblies não foi implementada.
