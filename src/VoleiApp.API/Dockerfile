FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/VoleiApp.API/VoleiApp.API.csproj", "src/VoleiApp.API/"]
COPY ["src/VoleiApp.Application/VoleiApp.Application.csproj", "src/VoleiApp.Application/"]
COPY ["src/VoleiApp.Domain/VoleiApp.Domain.csproj", "src/VoleiApp.Domain/"]
COPY ["src/VoleiApp.Infrastructure/VoleiApp.Infrastructure.csproj", "src/VoleiApp.Infrastructure/"]
RUN dotnet restore "src/VoleiApp.API/VoleiApp.API.csproj"

COPY . .
WORKDIR "/src/src/VoleiApp.API"
RUN dotnet publish "VoleiApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VoleiApp.API.dll"]
