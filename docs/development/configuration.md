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

## Segredos

- Nunca use os placeholders de exemplo em ambiente compartilhado.
- Não registre valores de `Jwt:Key`, senha do banco, senha do administrador ou
  tokens.
- Prefira um secret store do provedor de implantação.
- Restrinja acesso aos segredos e defina rotação.
- O migrador possui `UserSecretsId`; a API não. Não há integração com um cofre.

## Ambientes

A execução local com `ASPNETCORE_ENVIRONMENT=Development` habilita
`/swagger/v1/swagger.json` e a Swagger UI em `/swagger/index.html`.

O código usa valores de fallback para issuer e audience apenas se as chaves não
existirem. Como o arquivo base declara essas chaves com string vazia, execuções
locais devem informar `Jwt__Issuer` e `Jwt__Audience` explicitamente.
