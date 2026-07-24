# Autenticação e autorização

## Fluxo

1. O cliente cadastra um usuário em `POST /api/users`.
2. Autentica em `POST /api/auth/login`.
3. A API valida e-mail, senha e estado ativo.
4. Um JWT assinado com HMAC-SHA256 é emitido por duas horas.
5. O cliente envia `Authorization: Bearer <token>`.
6. A API valida assinatura, emissor, audiência, expiração e papel.

O token contém identificadores de usuário, e-mail e papel. A tolerância de relógio
configurada é de um minuto.

## Papéis

| Papel | Acesso |
|---|---|
| `User` | perfil próprio e biblioteca autenticada |
| `Administrator` | operações administrativas de usuários, catálogo e promoções |

Endpoints sem `[Authorize]` permanecem públicos. Endpoints administrativos exigem
o papel `Administrator`.

## Configuração JWT

```json
{
  "Jwt": {
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGames.Client",
    "Key": "<segredo-com-pelo-menos-32-caracteres>"
  }
}
```

`Jwt:Key` é obrigatória e deve ter pelo menos 32 caracteres. Não grave uma chave
real em `appsettings*.json`, `.env.example`, documentação, teste ou histórico Git.
Use variável `Jwt__Key` ou o gerenciador de segredos da plataforma.

## Administrador inicial

O migrador executa o `AdminSeeder` quando `Admin__Email` e `Admin__Password` são
fornecidos juntos. Esse é o único caminho implementado para criar inicialmente um
administrador. O Compose exige essas variáveis; para execução local direta elas
são opcionais.

Exemplo apenas local:

```powershell
$env:Admin__Email = "admin.local@example.com"
$env:Admin__Password = "<senha-local-forte>"
$env:Admin__Name = "Administrador local"
```

## Respostas

- `401 Unauthorized`: credencial ausente, inválida, expirada ou login recusado.
- `403 Forbidden`: token válido, mas papel insuficiente.

## Limitações

- Não há refresh token, logout com revogação ou lista de bloqueio.
- Não há rotação automatizada da chave JWT.
- Não há recuperação de senha, MFA, confirmação de e-mail ou provedor externo.
- Não há policies além da verificação de papéis.
- `TODO: definir ciclo de vida de credenciais e rotação de segredos para produção.`
