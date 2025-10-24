FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Agent.LogAnalyzer/Agent.LogAnalyzer.csproj", "Agent.LogAnalyzer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Agent.LogAnalyzer/Agent.LogAnalyzer.csproj"
COPY . .
WORKDIR "/src/Agent.LogAnalyzer"
RUN dotnet build "Agent.LogAnalyzer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Agent.LogAnalyzer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Agent.LogAnalyzer.dll"]
