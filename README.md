# RO.DevTest API

## Visão Geral do Projeto

Este projeto é uma API RESTful desenvolvida em .NET 8.0 que fornece funcionalidades para gerenciar clientes, produtos e vendas. Ele utiliza uma arquitetura em camadas (Domínio, Aplicação, Infraestrutura, Persistência e WebApi) e se conecta a um banco de dados PostgreSQL para armazenamento de dados. A autenticação é gerenciada usando ASP.NET Core Identity e tokens JWT (JSON Web Tokens).

## Funcionalidades Principais

*   **Gerenciamento de Clientes:** Operações CRUD (Criar, Ler, Atualizar, Deletar) para clientes.
*   **Gerenciamento de Produtos:** Operações CRUD para produtos.
*   **Gerenciamento de Vendas:** Operações CRUD para vendas, incluindo o registro de itens de venda.
*   **Análise de Vendas:** Endpoint para obter uma análise agregada das vendas dentro de um período específico.
*   **Gerenciamento de Usuários:** Criação de usuários para acesso à API.
*   **Autenticação:** Sistema de autenticação baseado em JWT.

## Como Executar o Projeto (Usando Docker)

A maneira mais fácil de executar este projeto localmente é usando Docker e Docker Compose. Isso garante que tanto a API quanto o banco de dados PostgreSQL sejam configurados e executados corretamente em contêineres isolados.

### Pré-requisitos

*   **Docker:** Certifique-se de ter o Docker instalado em sua máquina. [Instruções de instalação do Docker](https://docs.docker.com/engine/install/)
*   **Docker Compose:** Certifique-se de ter o Docker Compose instalado. Geralmente, ele vem junto com o Docker Desktop, mas pode ser necessário instalá-lo separadamente em algumas distribuições Linux. [Instruções de instalação do Docker Compose](https://docs.docker.com/compose/install/)

### Passos para Execução

1.  **Obtenha os Arquivos:** Certifique-se de ter a pasta `back-end` contendo o código-fonte da API, o `Dockerfile` e o `docker-compose.yml`.
2.  **Navegue até o Diretório:** Abra um terminal ou prompt de comando e navegue até o diretório `back-end` do projeto:
    ```bash
    cd path/to/your/project/RO.DevTest/back-end
    ```
3.  **Construa e Execute os Contêineres:** Execute o seguinte comando para construir as imagens Docker (se ainda não existirem) e iniciar os contêineres em segundo plano (`-d`):
    ```bash
    docker-compose up -d --build
    ```
    *Nota: Pode ser necessário usar `sudo` dependendo da configuração do Docker em seu sistema Linux.*

4.  **Aguarde a Inicialização:** O Docker Compose iniciará dois contêineres: `rodevtest_db` (PostgreSQL) e `rodevtest_api` (a API .NET). Pode levar alguns instantes para que ambos os serviços estejam totalmente operacionais, especialmente na primeira execução, pois as imagens precisam ser baixadas e construídas.
5.  **Acesse a API:** Após a inicialização bem-sucedida, a API estará acessível em `http://localhost:5000`.

### Configuração do Ambiente Docker

*   **API:** O serviço `app` no `docker-compose.yml` constrói a imagem da API usando o `Dockerfile` e a executa. Ele mapeia a porta 5000 do seu host para a porta 8080 dentro do contêiner onde a API está rodando.
*   **Banco de Dados:** O serviço `db` utiliza a imagem oficial do PostgreSQL. A string de conexão no `docker-compose.yml` está configurada para que a API se conecte a este serviço (`Server=db`). Os dados do PostgreSQL são persistidos usando um volume Docker chamado `postgres_data`, garantindo que os dados não sejam perdidos ao reiniciar os contêineres.
*   **Variáveis de Ambiente:** Configurações importantes, como a string de conexão e as chaves JWT, são passadas para o contêiner da API através de variáveis de ambiente definidas no `docker-compose.yml`.

## Como Usar a API

O URL base da API é `http://localhost:5000`.

A API utiliza Swagger/OpenAPI para documentação interativa, que geralmente está disponível em `http://localhost:5000/swagger` quando o ambiente está configurado como `Development` (que é o padrão no `docker-compose.yml`).

### Autenticação

A API utiliza autenticação JWT. Para acessar endpoints protegidos, você precisará obter um token JWT (geralmente através de um endpoint de login, que pode precisar ser implementado ou está disponível através das configurações padrão do Identity) e incluí-lo no cabeçalho `Authorization` de suas requisições como `Bearer <seu_token>`.

O endpoint para criar usuários está disponível:

*   `POST /api/users/CreateUser`

### Exemplos de Chamadas à API

Você pode usar ferramentas como `curl`, Postman ou a interface do Swagger para interagir com a API.

**1. Criar um Novo Usuário:**

*   **Método:** `POST`
*   **URL:** `http://localhost:5000/api/users/CreateUser`
*   **Corpo (JSON):**
    ```json
    {
      "username": "testuser",
      "email": "test@example.com",
      "password": "Password123"
    }
    ```
*   **Resposta Esperada (Exemplo):** Status `201 Created` com detalhes do usuário criado ou resultado da operação.

**2. Obter Lista de Produtos (com Paginação):**

*   **Método:** `GET`
*   **URL:** `http://localhost:5000/api/product?PageNumber=1&PageSize=10`
*   **Resposta Esperada (Exemplo):** Status `200 OK` com uma lista paginada de produtos.
    ```json
    {
      "items": [
        {
          "id": "c3d4e5f6-abcd-1234-5678-abcdef123456",
          "name": "Produto Exemplo 1",
          "description": "Descrição do produto exemplo 1",
          "price": 19.99,
          "itemSales": []
        },
        {
          "id": "a1b2c3d4-efgh-9876-5432-fedcba987654",
          "name": "Produto Exemplo 2",
          "description": "Descrição do produto exemplo 2",
          "price": 25.50,
          "itemSales": []
        }
      ],
      "currentPage": 1,
      "totalPages": 5,
      "pageSize": 10,
      "totalCount": 45,
      "hasPrevious": false,
      "hasNext": true
    }
    ```

**3. Criar um Novo Produto:**

*   **Método:** `POST`
*   **URL:** `http://localhost:5000/api/product`
*   **Corpo (JSON):**
    ```json
    {
      "name": "Novo Produto",
      "description": "Descrição detalhada do novo produto.",
      "price": 99.90
    }
    ```
*   **Resposta Esperada (Exemplo):** Status `201 Created` com os detalhes do produto criado.

**4. Obter Análise de Vendas:**

*   **Método:** `GET`
*   **URL:** `http://localhost:5000/api/sale/analysis?startDate=2024-01-01&endDate=2024-12-31`
*   **Resposta Esperada (Exemplo):** Status `200 OK` com os dados da análise.
    ```json
    {
      "totalSalesValue": 1500.75,
      "numberOfSales": 50,
      "averageSaleValue": 30.015,
      "topSellingProducts": [
        {
          "productId": "a1b2c3d4-efgh-9876-5432-fedcba987654",
          "productName": "Produto Exemplo 2",
          "totalQuantitySold": 25,
          "totalValueSold": 637.50
        }
        // ... outros produtos
      ]
    }
    ```

**5. Criar uma Nova Venda:**

*   **Método:** `POST`
*   **URL:** `http://localhost:5000/api/sale`
*   **Corpo (JSON):**
    ```json
    {
      "customerId": "f1e2d3c4-b5a6-7890-1234-abcdefabcdef", // ID de um cliente existente
      "observations": "Venda de teste.",
      "items": [
        {
          "productId": "c3d4e5f6-abcd-1234-5678-abcdef123456", // ID de um produto existente
          "quantity": 2,
          "unitPrice": 19.99 // O preço unitário pode ser validado/obtido do produto no backend
        },
        {
          "productId": "a1b2c3d4-efgh-9876-5432-fedcba987654",
          "quantity": 1,
          "unitPrice": 25.50
        }
      ]
    }
    ```
*   **Resposta Esperada (Exemplo):** Status `201 Created` com os detalhes da venda criada.

### Outros Endpoints

A API inclui endpoints CRUD completos para Clientes (`/api/customer`), Produtos (`/api/product`) e Vendas (`/api/sale`). Consulte a documentação do Swagger (`/swagger`) para obter detalhes sobre todos os endpoints disponíveis, seus parâmetros e respostas esperadas.

