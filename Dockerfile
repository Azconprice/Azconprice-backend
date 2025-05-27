# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY API ./API
COPY Application ./Application
COPY Domain ./Domain
COPY Infrastructure ./Infrastructure
COPY Persistence ./Persistence
COPY Azconprice-backend.sln .

WORKDIR /src/API

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]
