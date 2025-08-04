# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY VladTelegramBot/*.csproj ./VladTelegramBot/
RUN dotnet restore

COPY VladTelegramBot/. ./VladTelegramBot/
WORKDIR /app/VladTelegramBot
RUN dotnet publish -c Release -o out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/VladTelegramBot/out ./
ENTRYPOINT ["dotnet", "VladTelegramBot.dll"]
