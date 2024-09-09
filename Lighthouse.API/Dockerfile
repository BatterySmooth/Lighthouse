FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Lighthouse.API/Lighthouse.API.csproj", "Lighthouse.API/"]
RUN dotnet restore "Lighthouse.API/Lighthouse.API.csproj"
COPY . .
WORKDIR "/src/Lighthouse.API"
RUN dotnet build "Lighthouse.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lighthouse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ARG ENVIRONMENT
COPY ./Lighthouse.AISListener/secrets.${ENVIRONMENT}.json ./secrets.json
RUN find . -type f -name "secrets.*.json" ! -name "secrets.json" -delete
ENTRYPOINT ["dotnet", "Lighthouse.API.dll"]
