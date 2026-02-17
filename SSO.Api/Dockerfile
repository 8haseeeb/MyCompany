# Dockerfile for SSO.Api
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files to leverage Docker layer caching
COPY ["SSO.Api/SSO.Api.csproj", "SSO.Api/"]
COPY ["SSO.Application/SSO.Application.csproj", "SSO.Application/"]
COPY ["SSO.Domain/SSO.Domain.csproj", "SSO.Domain/"]
COPY ["SSO.Infrastructure/SSO.Infrastructure.csproj", "SSO.Infrastructure/"]
COPY ["SSO.Contracts/SSO.Contracts.csproj", "SSO.Contracts/"]
COPY ["BuildingBlocks/Common.Logging/MyCompany.Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]

# Restore dependencies
RUN dotnet restore "SSO.Api/SSO.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/SSO.Api"
RUN dotnet build "SSO.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SSO.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SSO.Api.dll"]
