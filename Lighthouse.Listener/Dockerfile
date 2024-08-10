FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Lighthouse.Listener/Lighthouse.Listener.csproj", "Lighthouse.Listener/"]
RUN dotnet restore "Lighthouse.Listener/Lighthouse.Listener.csproj"
COPY . .
WORKDIR "/src/Lighthouse.Listener"
RUN dotnet build "Lighthouse.Listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lighthouse.Listener.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lighthouse.Listener.dll"]
