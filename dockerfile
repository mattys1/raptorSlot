FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore first to leverage docker cache
COPY ["CRUD.csproj", "."]
RUN dotnet restore "CRUD.csproj"

# copy everything and publish
COPY . .
RUN dotnet publish "CRUD.csproj" -c Release -o /app/publish /p:PublishTrimmed=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
# ENV ASPNETCORE_URLS=http://+:80

# ENTRYPOINT ["dotnet", "CRUD.dll"]
