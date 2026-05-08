# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY DevideDemo.csproj .
RUN dotnet restore

# Копируем весь код и публикуем
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ===== СТАДИЯ 2: runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Устанавливаем wget и curl для healthcheck (с очисткой кэша)
RUN apt-get update && apt-get install -y --no-install-recommends wget curl \
    && rm -rf /var/lib/apt/lists/*

# Копируем собранное приложение
COPY --from=build /app/publish .

# Открываем порт
EXPOSE 8080

# Переменная окружения для порта
ENV ASPNETCORE_URLS=http://+:8080

# Точка входа
ENTRYPOINT ["dotnet", "DevideDemo.dll"]