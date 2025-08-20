# -------- BUILD STAGE --------
# .NET SDK 8.0-a əsaslanan Docker obrazı istifadə edilir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Layihə faylları kopyalanır (daha sürətli bərpa üçün)
COPY ./*.sln ./
COPY API/*.csproj ./API/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Persistence/*.csproj ./Persistence/

# Asılılıqlar bərpa edilir
RUN dotnet restore

# Bütün layihə kodları kopyalanır
COPY . .

# Entity Framework alətləri quraşdırılır (migrationlar üçün lazımdır)
# BU ADDIM MİGRATİONLARI QAÇIRMAK ÜÇÜN ÇOX ÖNƏMLİDİR!
RUN dotnet tool install --global dotnet-ef --version 8.0.0

# Qovluqlar dəyişdirilir və migrationlar tətbiq edilir
# İstifadəçi istəyinə uyğun olaraq əlavə edilib, lakin tövsiyə edilmir.
WORKDIR /src/API
RUN dotnet ef database update --project ../Persistence --startup-project ./

# Layihə nəşr edilir (publish)
WORKDIR /src
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
# .NET ASP.NET Runtime 8.0 obrazı əsas götürülür
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Nəşr olunmuş proyekt faylları kopyalanır
# Bu sətir sadəcə lazımi faylları köçürür
COPY --from=build /app/publish .

# Tətbiqin işə düşmə nöqtəsi
ENTRYPOINT ["dotnet", "API.dll"]