# Dockerfile for Promotions.Api
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files to leverage Docker layer caching
COPY ["Promotions.Api/Promotions.Api.csproj", "Promotions.Api/"]
COPY ["Promotions.Application/Promotions.Application.csproj", "Promotions.Application/"]
COPY ["Promotions.Domain/Promotions.Domain.csproj", "Promotions.Domain/"]
COPY ["Promotions.Infrastructure/Promotions.Infrastructure.csproj", "Promotions.Infrastructure/"]
COPY ["Promotions.Contracts/Promotions.Contracts.csproj", "Promotions.Contracts/"]
COPY ["BuildingBlocks/Common.Logging/MyCompany.Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]

# Restore dependencies
RUN dotnet restore "Promotions.Api/Promotions.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/Promotions.Api"
RUN dotnet build "Promotions.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Promotions.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Promotions.Api.dll"]
