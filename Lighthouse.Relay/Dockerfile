FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Lighthouse.Relay/Lighthouse.Relay.csproj", "Lighthouse.Relay/"]
RUN dotnet restore "Lighthouse.Relay/Lighthouse.Relay.csproj"
COPY . .
WORKDIR "/src/Lighthouse.Relay"
RUN dotnet build "Lighthouse.Relay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lighthouse.Relay.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ARG ENVIRONMENT
COPY ./Lighthouse.Relay/secrets.${ENVIRONMENT}.json ./secrets.json
ENTRYPOINT ["dotnet", "Lighthouse.Relay.dll"]
