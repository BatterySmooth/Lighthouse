FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Lighthouse/Lighthouse.csproj", "Lighthouse/"]
RUN dotnet restore "Lighthouse/Lighthouse.csproj"
COPY . .
WORKDIR "/src/Lighthouse"
RUN dotnet build "Lighthouse.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lighthouse.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lighthouse.dll"]
