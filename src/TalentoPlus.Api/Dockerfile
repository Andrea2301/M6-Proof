FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/TalentoPlus.Api/TalentoPlus.Api.csproj", "src/TalentoPlus.Api/"]
COPY ["src/TalentoPlus.Core/TalentoPlus.Core.csproj", "src/TalentoPlus.Core/"]
COPY ["src/TalentoPlus.Infrastructure/TalentoPlus.Infrastructure.csproj", "src/TalentoPlus.Infrastructure/"]
COPY ["src/TalentoPlus.Shared/TalentoPlus.Shared.csproj", "src/TalentoPlus.Shared/"]

RUN dotnet restore "src/TalentoPlus.Api/TalentoPlus.Api.csproj"

COPY . .

WORKDIR "/src/src/TalentoPlus.Api"
RUN dotnet build "TalentoPlus.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TalentoPlus.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.Api.dll"]
