# Docker

## Serviços

O `docker-compose.yml` cria:

| Serviço | Imagem/build | Porta | Dependência |
|---|---|---:|---|
| `database` | `postgres:17-alpine` | `5432` | nenhuma |
| `migrator` | Dockerfile .NET runtime 10 | — | banco saudável |
| `api` | Dockerfile ASP.NET runtime 10 | `8080` | migrador concluído |

O banco usa o volume nomeado `fiap_cloud_games_data`. Os Dockerfiles compilam em
imagem SDK .NET 10 e executam em uma imagem runtime menor, com o usuário
`$APP_UID`.

## Iniciar

```powershell
Copy-Item .env.example .env
```

Substitua todos os placeholders do `.env` por valores exclusivamente locais e
então execute:

```powershell
docker compose up --build
```

A sequência esperada é:

1. PostgreSQL inicia e responde ao `pg_isready`;
2. o migrador aplica migrations e cria o administrador;
3. a API inicia em `http://localhost:8080`;
4. `GET http://localhost:8080/health` retorna sucesso.

O Compose usa o ambiente `Production`; o JSON OpenAPI fica desabilitado.

## Inspeção

```powershell
docker compose ps
docker compose logs database
docker compose logs migrator
docker compose logs api
```

Para acompanhar:

```powershell
docker compose logs -f api
```

## Parar

Preservando os dados:

```powershell
docker compose down
```

Apagando também o banco local:

```powershell
docker compose down --volumes
```

O segundo comando é destrutivo: remove o volume nomeado e os dados locais não
podem ser recuperados pelo Compose.

## Limitações

- portas, banco, usuário, issuer e audience estão fixados no arquivo Compose;
- não há TLS, reverse proxy ou limitação de recursos;
- não há imagem publicada em registry;
- o health check do Compose cobre o PostgreSQL, mas não o serviço API;
- não há backup automatizado do volume.

