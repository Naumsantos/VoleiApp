# 🏐 VoleiApp — Sorteio e Gestão de Times de Vôlei

Backend em **ASP.NET Core** para organizar peladas de vôlei com sorteio de times, reservas, substituições e histórico de partidas.

> Objetivo do projeto: transformar o processo manual (papel) em uma aplicação real, evoluindo com boas práticas de arquitetura e engenharia de software.

---

## ✅ Status atual

O projeto foi reorganizado para uma base mais profissional com:

- **Clean Architecture (camadas)**
- **Injeção de Dependência (DI)**
- **Repository Pattern**
- **Services via interfaces**
- **Persistência com SQLite (EF Core)**

---

## 🧱 Arquitetura

Estrutura atual:

- `src/VoleiApp.API`  
  Camada de entrada HTTP (controllers, Program, composição da aplicação)
- `src/VoleiApp.Application`  
  Casos de uso, serviços de aplicação, DTOs e contratos de serviço
- `src/VoleiApp.Domain`  
  Entidades e contratos de domínio (interfaces de repositório)
- `src/VoleiApp.Infrastructure`  
  Persistência com EF Core (`VoleiContext`) e implementações de repositório
- `tests/`  
  Projetos reservados para testes unitários e de integração (evolução contínua)

---

## 🛠️ Tecnologias

- C#
- ASP.NET Core
- Entity Framework Core
- SQLite
- OpenAPI (Swagger)

---

## ▶️ Como executar localmente

### Pré-requisitos
- .NET SDK instalado (compatível com o `TargetFramework` do projeto)
- CLI do .NET

### Passos

1. Clone o repositório:
   ```bash
   git clone https://github.com/Naumsantos/VoleiApp.git
   cd VoleiApp
   ```

2. Restaure os pacotes:
   ```bash
   dotnet restore
   ```

3. Compile:
   ```bash
   dotnet build
   ```

4. Execute a API:
   ```bash
   dotnet run --project src/VoleiApp.API
   ```

5. Acesse OpenAPI/Swagger (ambiente de desenvolvimento):
   - URL padrão conforme saída do terminal (ex.: `https://localhost:xxxx/openapi`)

---

## ⚙️ Configuração

`src/VoleiApp.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=voleiapp.db"
  }
}
```

---

## 📌 Endpoints atuais (resumo)

### Sorteio
- `POST /api/sorteio/sortear`
  - Realiza sorteio com base na configuração enviada (atletas + tamanho do time)

- `POST /api/sorteio/salvar`
  - Salva uma partida com Time A e Time B

> Observação: a regra avançada por posição (2 atacantes + 1 meio + 1 levantador com fallback) será implementada na próxima etapa (PR 2).

---

## 🧠 Modelo de domínio (atual)

Principais entidades:

- `Atleta`
- `Time`
- `Partida`
- `Substituicao`
- `SorteioConfig` (e DTOs correlatos na Application)

---

## 🔌 Contratos e DI

### Repositórios (Domain)
- `IAtletaRepository`
- `IPartidaRepository`

### Serviços (Application)
- `ISorteioService`

### Implementações (Infrastructure/Application)
- `AtletaRepository`
- `PartidaRepository`
- `SorteioService`

Registrados via DI no `Program.cs`.

---

## 🗺️ Roadmap (próximas evoluções)

### PR 2 (negócio)
- Regra de formação por posição:
  - **2 Atacantes + 1 Meio + 1 Levantador**
- Fallback quando faltar posição
- Retorno explicável (times incompletos, fallback aplicado, reservas)

### PR 3 (qualidade)
- Testes unitários do algoritmo de sorteio
- Testes de integração dos endpoints críticos

### Evoluções futuras
- Autenticação/autorização
- Métricas e observabilidade
- Ranking/nível por atleta
- Regras de balanceamento avançado

---

## 🤝 Contribuição

Contribuições são bem-vindas via Issue/PR com contexto claro da mudança.

---

Feito com foco em evolução contínua e arquitetura limpa. 🚀