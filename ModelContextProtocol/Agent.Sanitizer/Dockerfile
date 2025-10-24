FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Agent.Sanitizer/Agent.Sanitizer.csproj", "Agent.Sanitizer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Agent.Sanitizer/Agent.Sanitizer.csproj"
COPY . .
WORKDIR "/src/Agent.Sanitizer"
RUN dotnet build "Agent.Sanitizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Agent.Sanitizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Agent.Sanitizer.dll"]
