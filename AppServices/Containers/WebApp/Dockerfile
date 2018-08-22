FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY AppServices/Containers/WebApp/WebApp.csproj AppServices/Containers/WebApp/
RUN dotnet restore AppServices/Containers/WebApp/WebApp.csproj
COPY . .
WORKDIR /src/AppServices/Containers/WebApp
RUN dotnet build WebApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApp.dll"]
