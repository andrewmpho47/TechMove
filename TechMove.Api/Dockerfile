FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TechMove.Api/TechMove.Api.csproj TechMove.Api/
COPY TechMove.csproj ./
RUN dotnet restore TechMove.Api/TechMove.Api.csproj
COPY . .
RUN dotnet publish TechMove.Api/TechMove.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMove.Api.dll"]
