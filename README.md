### Atualização do README.md

Aqui está um README atualizado para sua aplicação core, considerando o uso de MongoDB e outras configurações que discutimos:

---

```markdown
# CoreAPI - Backend Centralizado para Aplicações

## Descrição

**CoreAPI** é o backend centralizado que servirá como núcleo para todas as aplicações desenvolvidas. O objetivo é fornecer uma base robusta, escalável e reutilizável, que gerencie autenticação, dados e serviços de forma eficiente. A aplicação utiliza MongoDB como banco de dados e é construída com .NET 8.0.

## Principais Funcionalidades

- **Gerenciamento de Usuários**: CRUD para usuários.
- **Configuração Centralizada**: Facilita a configuração de parâmetros globais.
- **Integração com MongoDB**: Banco de dados NoSQL flexível e escalável.
- **APIs Documentadas**: Utilização do Swagger para documentação.
- **Logs Estruturados**: Implementação de Serilog para monitoramento.
- **Autenticação JWT**: Proteção de endpoints com tokens seguros.

---

## Tecnologias Utilizadas

- **.NET 8.0**: Framework principal para desenvolvimento.
- **MongoDB**: Banco de dados NoSQL.
- **Swagger/OpenAPI**: Documentação interativa das APIs.
- **Serilog**: Monitoramento e geração de logs estruturados.
- **JWT (JSON Web Tokens)**: Implementação de autenticação segura.

---

## Estrutura do Projeto

```plaintext
CoreAPI/
├── Config/               # Configurações (e.g., MongoDB)
├── Controllers/          # Controladores da API
├── Models/               # Modelos e DTOs
├── Repositories/         # Repositórios de dados
├── Middleware/           # Middlewares personalizados
├── Logs/                 # Logs (gerados pelo Serilog)
├── Tests/                # Testes automatizados
└── Program.cs            # Arquivo principal de inicialização
```

---

## Configuração e Execução

### Pré-requisitos

- **.NET SDK 8.0** ou superior instalado.
- **MongoDB** em execução local ou em um servidor remoto.
- **Git** para versionamento.

### Configuração

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/CoreAPI.git
   cd CoreAPI
   ```

2. Configure o arquivo `appsettings.json`:
   ```json
   {
     "DatabaseSettings": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "CoreAPI"
     },
     "JwtSettings": {
       "Issuer": "yourissuer",
       "Audience": "youraudience",
       "SecretKey": "your_secret_key"
     }
   }
   ```

3. Restaure as dependências do projeto:
   ```bash
   dotnet restore
   ```

---

### Execução

1. Compile o projeto:
   ```bash
   dotnet build
   ```

2. Inicie o servidor:
   ```bash
   dotnet run
   ```

3. Acesse o Swagger para explorar os endpoints:
   ```
   http://localhost:5000/swagger
   ```

---

## Endpoints Principais

### **Usuários**

- `GET /api/users`: Lista todos os usuários.
- `GET /api/users/{id}`: Retorna um usuário específico.
- `POST /api/users`: Cria um novo usuário.
- `PUT /api/users/{id}`: Atualiza um usuário.
- `DELETE /api/users/{id}`: Remove um usuário.

---

## Logs

Os logs da aplicação são gerados automaticamente pelo **Serilog** e armazenados no diretório `Logs/` com divisão diária. Eles incluem informações sobre erros, requisições e respostas.

---

## Testes

1. Execute os testes automatizados:
   ```bash
   dotnet test
   ```

2. Os testes estão localizados no diretório `Tests/`.

---

## Contribuições

Contribuições são bem-vindas! Para colaborar:

1. Faça um fork do repositório.
2. Crie uma branch para suas alterações:
   ```bash
   git checkout -b minha-nova-feature
   ```
3. Faça commit de suas mudanças:
   ```bash
   git commit -m "Minha nova feature"
   ```
4. Envie suas alterações:
   ```bash
   git push origin minha-nova-feature
   ```
5. Abra um Pull Request.

---

## Licença

Este projeto está licenciado sob a **MIT License**. Consulte o arquivo `LICENSE` para mais informações.

---

**Desenvolvido por [Diego Luan](https://github.com/diegoluanfs).**
```

---