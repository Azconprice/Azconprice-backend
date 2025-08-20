# -------- BUILD STAGE --------
# .NET SDK 8.0-a əsaslanan Docker obrazı istifadə edilir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proyekt faylları (csproj və s.) əlavə edilir
# Bu, `dotnet restore` əmrinin daha sürətli işləməsini təmin edir, çünki dəyişməyən layihə faylları keşdə saxlanılır
COPY *.sln .
COPY API/*.csproj ./API/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Persistence/*.csproj ./Persistence/

# Asılılıqlar bərpa edilir
RUN dotnet restore

# Bütün layihə kodları kopyalanır
COPY . .

# Proyekt nəşr edilir (publish)
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
# .NET ASP.NET Runtime 8.0 obrazı əsas götürülür
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Əlavə paketlər quraşdırılır (məsələn, migrationlar üçün lazımdır)
# Bu sətir xətaların qarşısını alır
RUN apt-get update && apt-get install -y libgdiplus

# Nəşr olunmuş proyekt faylları kopyalanır
COPY --from=build /app/publish .

# Tətbiqə verilənlər bazası migrationları tətbiq etmək üçün xüsusi entrypoint scripti yaradılır
# Bu script tətbiq işə düşməzdən əvvəl migrationları tətbiq edir
COPY --from=build /src/Persistence/ ./Persistence/
COPY --from=build /src/API/ ./API/
COPY --from=build /src/Domain/ ./Domain/
COPY --from=build /src/Application/ ./Application/

# Tətbiqin işə düşmə nöqtəsi
ENTRYPOINT ["dotnet", "API.dll", "migrate"]