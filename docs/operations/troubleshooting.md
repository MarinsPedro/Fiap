# Troubleshooting

## SDK incompatível

**Sintoma:** `NETSDK1045` ou framework `net10.0` não reconhecido.  
**Diagnóstico:** execute `dotnet --version` e `dotnet --list-sdks`.  
**Correção:** instale um SDK .NET 10 compatível; a versão usada na validação está
registrada no README raiz.

## Restore falha

**Sintoma:** pacotes não encontrados, erro de feed ou certificado.  
**Diagnóstico:** execute
`dotnet restore FiapCloudGames.sln --configfile NuGet.Config`.  
**Correção:** valide acesso ao feed, proxy/certificado e o `NuGet.Config`; não
remova versões sem verificar compatibilidade.

## Conexão com banco recusada

**Sintoma:** endpoints de negócio falham, embora `/health` responda.  
**Diagnóstico:** confira `ConnectionStrings__Database`, host e porta a partir do
mesmo ambiente da API.  
**Correção:** valide se o PostgreSQL aceita conexões no host e na porta
configurados e se a credencial possui acesso ao banco informado.

## Migrator falha

**Sintoma:** o executável do migrador termina com código diferente de zero.

**Diagnóstico:** execute
`dotnet run --project src/Database/FiapCloudGames.Database.Migrations` e
inspecione a saída.

**Correção:** valide connection string, permissões, ordem/estado das migrations e
as três variáveis `ADMIN_*`. Não edite manualmente a tabela de versionamento.

## Migration pendente

**Sintoma:** erro de tabela/coluna inexistente.  
**Diagnóstico:** compare a versão implantada do código com as migrations
executadas e inspecione o log do migrador.  
**Correção:** execute o migrador compatível antes da API. Para manutenção isolada,
use `dotnet tool run dotnet-ef database update` com o `--context` correto.

Se um comando `dotnet-ef` informar que `ConnectionStrings:Database` não foi
encontrada, defina `ConnectionStrings__Database` antes de executá-lo. As quatro
factories de design-time exigem essa configuração e não possuem fallback.

## JWT não configurado

**Sintoma:** a API não inicia ou informa chave inválida.  
**Diagnóstico:** confira se `Jwt__Key` existe e possui ao menos 32 caracteres.  
**Correção:** forneça um segredo válido pelo ambiente, sem imprimi-lo no log.

## 401 Unauthorized

**Causas comuns:** token ausente, inválido, expirado, issuer/audience diferentes,
usuário inativo ou login incorreto.  
**Correção:** gere novo token em `/api/auth/login` e use
`Authorization: Bearer <token>`.

## 403 Forbidden

**Causa:** o token é válido, mas o papel não atende ao endpoint.  
**Correção:** use uma conta `Administrator` apenas para rotas administrativas; o
seed é feito pelo migrador.

## 404 Not Found

**Causa:** rota incorreta ou recurso inexistente/inativo.  
**Correção:** confirme método, rota e ID. A listagem de catálogo só mostra jogos
ativos.

## 409 Conflict

`AppException.Conflict` é mapeada para 409. Os casos atuais incluem e-mail já
cadastrado e tentativa de adquirir novamente um jogo da mesma biblioteca. Leia
`detail` e atualize a operação ou o estado consultado. Uma violação de unicidade
que ocorrer diretamente no banco, por exemplo em uma corrida entre requests,
não recebe tratamento específico e pode resultar em 500.

## 422 Unprocessable Entity

**Causa:** operação bem formada viola uma regra, como usuário/jogo inativo ou
período de promoção inválido. Aquisição duplicada é conflito 409.

**Correção:** leia `detail`, confira o estado atual e corrija a operação.

## 500 Internal Server Error

**Diagnóstico:** capture o `traceId` da resposta e procure o mesmo contexto nos
logs.  
**Correção:** trate a causa; não envie connection strings, tokens ou senhas em
chamados.

## CORS

**Sintoma:** navegador bloqueia a chamada, mas ferramentas HTTP funcionam.  
**Diagnóstico:** compare a origem exata com `Cors:AllowedOrigins`.  
**Correção:** configure a origem autorizada por ambiente; não use abertura ampla
sem revisão de segurança.

## OpenAPI não aparece

O JSON OpenAPI só é habilitado em `Development`. Execute a API localmente com
`ASPNETCORE_ENVIRONMENT=Development` e consulte a rota
`/swagger/v1/swagger.json`; a interface fica em `/swagger/index.html`.

## `/health` responde, mas a API de negócio falha

O check atual mede apenas o processo. Verifique PostgreSQL, migrations,
configuração e logs.

## Testes de integração passam sem banco

Esse é o comportamento atual: os testes inspecionam o host e metadados, sem abrir
PostgreSQL. Consulte [testes de integração](../testing/integration-tests.md).
