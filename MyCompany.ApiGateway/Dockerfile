# Dockerfile for MyCompany.ApiGateway
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files to leverage Docker layer caching
COPY ["MyCompany.ApiGateway/MyCompany.ApiGateway.csproj", "MyCompany.ApiGateway/"]
COPY ["BuildingBlocks/Common.Logging/MyCompany.Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]

# Restore dependencies
RUN dotnet restore "MyCompany.ApiGateway/MyCompany.ApiGateway.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/MyCompany.ApiGateway"
RUN dotnet build "MyCompany.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyCompany.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyCompany.ApiGateway.dll"]
