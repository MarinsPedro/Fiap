# Padrões de documentação

## Objetivo

A documentação deve permitir que uma pessoa configure, execute, altere, teste e
opere o projeto sem depender de conhecimento oral. Ela descreve o estado real do
repositório; propostas devem ser identificadas como recomendação ou `TODO`.

## Convenções

- Escreva em português do Brasil.
- Use nomes exatos de projetos, namespaces, rotas e configurações.
- Prefira links relativos entre documentos do repositório.
- Marque exemplos não executáveis como ilustrativos.
- Nunca inclua credenciais, tokens ou connection strings reais.
- Use `TODO: descrição objetiva` quando a informação ou implementação não existir.
- Registre decisões arquiteturais duráveis em `docs/adr/`.
- Atualize `CHANGELOG.md` para mudanças relevantes.

## Estrutura mínima de um guia

1. objetivo e escopo;
2. estado atual;
3. procedimento ou fluxo;
4. exemplos;
5. validação;
6. riscos, limitações e TODOs;
7. links relacionados.

## Diagramas

Use Mermaid quando o diagrama for melhor mantido como texto. Preserve rótulos
curtos e nomes reais. O arquivo editável de Event Storming permanece em
[`docs/EventStorming.drawio`](../EventStorming.drawio).

## Definition of Done da documentação

- [ ] README raiz ainda representa a execução real.
- [ ] Índice em `docs/README.md` aponta para os documentos novos.
- [ ] README do módulo foi atualizado quando seus contratos ou regras mudaram.
- [ ] Endpoints, status, autorização e exemplos estão coerentes com controllers.
- [ ] Configurações novas informam obrigatoriedade e forma segura de fornecimento.
- [ ] Migration e estratégia de implantação estão documentadas.
- [ ] Testes e comandos foram executados.
- [ ] Links Markdown locais foram verificados.
- [ ] Decisão arquitetural recebeu ADR, quando aplicável.
- [ ] Nenhum segredo real foi adicionado.

## Revisão periódica

Não há responsável nem periodicidade formal definidos.

`TODO: definir owners, frequência de revisão e automação de links/Markdown no CI.`
