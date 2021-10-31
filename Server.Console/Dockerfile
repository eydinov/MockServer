FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Server.Console/MockServer.csproj", "Server.Console/"]
COPY ["Server.Host/MockServer.Host.csproj", "Server.Host/"]
COPY ["Environment/MockServer.Environment.csproj", "Environment/"]
RUN dotnet restore "Server.Console/MockServer.csproj"
COPY . .
WORKDIR "/src/Server.Console"
RUN dotnet build "MockServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockServer.dll"]