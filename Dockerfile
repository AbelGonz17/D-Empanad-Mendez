# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DMendez.Api/DMendez.Api.csproj", "src/DMendez.Api/"]
COPY ["src/DMendez.Application/DMendez.Application.csproj", "src/DMendez.Application/"]
COPY ["src/DMendez.Infrastructure/DMendez.Infrastructure.csproj", "src/DMendez.Infrastructure/"]
COPY ["src/DMendez.domain/DMendez.Domain.csproj", "src/DMendez.domain/"]
RUN dotnet restore "./src/DMendez.Api/DMendez.Api.csproj"
COPY . .
WORKDIR "/src/src/DMendez.Api"
RUN dotnet build "./DMendez.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DMendez.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DMendez.Api.dll"]
