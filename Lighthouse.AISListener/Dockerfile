FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Lighthouse.AISListener/Lighthouse.AISListener.csproj", "Lighthouse.AISListener/"]
RUN dotnet restore "Lighthouse.AISListener/Lighthouse.AISListener.csproj"
COPY . .
WORKDIR "/src/Lighthouse.AISListener"
RUN dotnet build "Lighthouse.AISListener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lighthouse.AISListener.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ARG ENVIRONMENT
COPY ./Lighthouse.AISListener/secrets.${ENVIRONMENT}.json ./secrets.json
RUN find . -type f -name "secrets.*.json" ! -name "secrets.json" -delete
ENTRYPOINT ["dotnet", "Lighthouse.AISListener.dll"]
