# Deployment

## Estado atual

O repositório oferece Dockerfiles e Compose para execução local, mas não define
uma plataforma de implantação. Não há registry, domínio, TLS, manifests,
infraestrutura como código ou pipeline.

## Artefatos

Publicação manual da API:

```powershell
dotnet publish src/Api/FiapCloudGames.Api/FiapCloudGames.Api.csproj `
  -c Release `
  -o artifacts/api
```

Publicação do migrador:

```powershell
dotnet publish src/Database/FiapCloudGames.Database.Migrations/FiapCloudGames.Database.Migrations.csproj `
  -c Release `
  -o artifacts/migrator
```

Os diretórios são exemplos locais; `artifacts/` não constitui um mecanismo de
distribuição definido pelo projeto.

## Ordem de implantação

1. Faça backup conforme o procedimento do ambiente.
2. Valide build, testes e imagem.
3. Disponibilize as configurações e segredos.
4. Execute o migrador uma única vez.
5. Confirme o término bem-sucedido.
6. Implante a API compatível com o schema.
7. Verifique `/health` e um smoke test de negócio.
8. Monitore erros e latência.

Migrations expansivas e compatíveis entre versões são preferíveis quando houver
mais de uma réplica ou atualização gradual.

## Rollback

O executável aplica somente migrations pendentes. O EF Core permite voltar um
contexto com `dotnet tool run dotnet-ef database update <migration> --context
<DbContext>`, mas o repositório ainda não possui automação ou estratégia
operacional aprovada para isso.

`TODO: definir versionamento, backup/restore, matriz de compatibilidade e
procedimento testado de rollback.`

## Requisitos antes de produção

- plataforma e topologia;
- PostgreSQL gerenciado e backups;
- HTTPS e gestão de domínio/certificado;
- secret store e rotação;
- replicas, limites e autoscaling;
- probes de readiness/liveness;
- observabilidade e alertas;
- CI/CD, registry e política de versões;
- teste de carga e análise de segurança.
