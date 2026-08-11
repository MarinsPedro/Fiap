# Configuração

## Fontes

API e migrador usam o carregamento padrão do .NET. Arquivos
`appsettings.json`, arquivo específico do ambiente, variáveis de ambiente e
argumentos de linha de comando podem participar; fontes posteriores sobrescrevem
as anteriores. Variáveis hierárquicas usam `__`, por exemplo
`ConnectionStrings__Database`.

## Chaves da aplicação

| Chave | Componente | Obrigatória | Observação |
|---|---|---:|---|
| `ConnectionStrings:Database` | API e migrador | sim | conexão PostgreSQL |
| `Jwt:Key` | API | sim | segredo com ao menos 32 caracteres |
| `Jwt:Issuer` | API | recomendada | `appsettings.json` contém string vazia; configure valor não vazio |
| `Jwt:Audience` | API | recomendada | `appsettings.json` contém string vazia; configure valor não vazio |
| `Cors:AllowedOrigins` | API | não | lista de origens; política configurada pela API |
| `Logging:LogLevel:*` | API | não | níveis padrão do logging .NET |
| `AllowedHosts` | API | não | filtro de hosts do ASP.NET Core |
| `ASPNETCORE_ENVIRONMENT` | API | não | `Development` habilita OpenAPI |
| `ASPNETCORE_URLS` | API | não | URLs de escuta |
| `Admin:Email` | migrador | em par | habilita seed quando senha também existe |
| `Admin:Password` | migrador | em par | senha do administrador inicial |
| `Admin:Name` | migrador | não | nome do administrador inicial |

## Variáveis do Compose

O arquivo `.env.example` documenta as entradas esperadas pelo Compose:

- `POSTGRES_PASSWORD`;
- `JWT_KEY`;
- `ADMIN_EMAIL`, `ADMIN_PASSWORD`, `ADMIN_NAME`.

Nome do banco, usuário, portas, emissor e audiência estão fixados no
`docker-compose.yml`. Alterá-los exige editar o Compose.

Copie o exemplo e mantenha o arquivo real fora do Git:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

## Segredos

- Nunca use os placeholders de exemplo em ambiente compartilhado.
- Não registre valores de `Jwt:Key`, senha do banco, senha do administrador ou
  tokens.
- Prefira um secret store do provedor de implantação.
- Restrinja acesso aos segredos e defina rotação.
- O migrador possui `UserSecretsId`; a API não. Não há integração com um cofre.

`TODO: selecionar o gerenciador de segredos e documentar criação, rotação e
revogação por ambiente.`

## Ambientes

O Compose executa a API em `Production`; por isso o documento OpenAPI não fica
exposto nesse fluxo. A execução local com `ASPNETCORE_ENVIRONMENT=Development`
habilita `/swagger/v1/swagger.json` e a Swagger UI em
`/swagger/index.html`.

O código usa valores de fallback para issuer e audience apenas se as chaves não
existirem. Como o arquivo base declara essas chaves com string vazia, execuções
fora do Compose devem informar `Jwt__Issuer` e `Jwt__Audience` explicitamente.

Não existem arquivos ou contratos formais para staging/produção, nem matriz de
valores por ambiente.

`TODO: definir ambientes suportados, responsáveis por configuração e processo de
promoção entre ambientes.`
