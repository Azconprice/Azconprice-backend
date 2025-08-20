# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./*.sln ./
COPY API/*.csproj ./API/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Persistence/*.csproj ./Persistence/

RUN dotnet restore

COPY . .

# Bu sətir migrationlar üçün connection stringi ötürür
ARG DB_CONNECTION_STRING
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y libgdiplus

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]