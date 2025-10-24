FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Agent.ReportGenerator/Agent.ReportGenerator.csproj", "Agent.ReportGenerator/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Agent.ReportGenerator/Agent.ReportGenerator.csproj"
COPY . .
WORKDIR "/src/Agent.ReportGenerator"
RUN dotnet build "Agent.ReportGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Agent.ReportGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Agent.ReportGenerator.dll"]
