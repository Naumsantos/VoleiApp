# 🏐 VoleiApp - Sorteio de Times de Vôlei

Este projeto é um aplicativo backend em .NET 8 para sorteio e gerenciamento de times de vôlei. Ele permite:

- Cadastro de atletas e suas posições
- Sorteio automático de times com base nas posições
- Controle de reservas e substituições de jogadores
- Registro e histórico de partidas

---

## 📦 Tecnologias utilizadas

- ASP.NET Core 8.0
- Entity Framework Core (InMemory)
- Swagger/OpenAPI
- C#

---

## 🚀 Como rodar o projeto

1. Clone o repositório:
   ```bash
   git clone https://github.com/Naumsantos/voleiapp.git
   cd voleiapp
   ```

2. Abra o projeto no **Visual Studio 2022+** ou **VS Code** com o SDK .NET 8 instalado.

3. Execute o projeto:
   ```bash
   dotnet run
   ```

4. Acesse a documentação Swagger:
   ```
   http://localhost:5000/swagger
   ```

---

## 📚 Funcionalidades disponíveis

### Atletas

- `GET /api/atletas`: Lista todos os atletas
- `POST /api/atletas`: Cadastra um novo atleta
- `PUT /api/atletas/{id}`: Atualiza um atleta existente
- `DELETE /api/atletas/{id}`: Remove um atleta

### Sorteio de Times

- `POST /api/sorteio/sortear`: Realiza o sorteio com base em:
  ```json
  {
    "numeroDeTimes": 4,
    "jogadoresPorTime": 4
  }
  ```
- `POST /api/sorteio/substituir/{idDoTime}`: Substitui jogadores do time perdedor com os reservas

### Partidas

- `POST /api/partidas`: Registra uma nova partida com substituições
- `GET /api/partidas`: Lista partidas registradas
- `GET /api/partidas/{id}`: Retorna detalhes de uma partida

---

## 🧠 Estrutura do Projeto

- `Models/`: Modelos principais (`Atleta`, `Time`, `Partida`, `Substituicao`, etc.)
- `Data/`: Contexto do banco de dados `VoleiContext`
- `Services/`: `SorteioService` para lógica de sorteio e substituição
- `Controllers/`: API REST organizada por domínio
- `Program.cs`: Configuração do pipeline e serviços da aplicação

---

## 📌 Próximas etapas (planejado)

- Implementar CRUD completo para Times
- Persistência real (SQLite, PostgreSQL, etc.)
- Autenticação e perfis (admin, jogador)
- Estatísticas de desempenho por atleta
- Aplicativo mobile (Blazor/MAUI ou Flutter)

---

> Feito com 💡 por Naum Santos Mourão — contribuição, crítica ou fork são bem-vindos!