#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TeamspeakRconExporter/TeamspeakRconExporter.csproj", "TeamspeakRconExporter/"]
RUN dotnet restore "TeamspeakRconExporter/TeamspeakRconExporter.csproj"
COPY . .
WORKDIR "/src/TeamspeakRconExporter"
RUN dotnet build "TeamspeakRconExporter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamspeakRconExporter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "TeamspeakRconExporter.dll"]