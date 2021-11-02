#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 4999

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["BaSyx.Registry.Server.Http.App/BaSyx.Registry.Server.Http.App.csproj", "BaSyx.Registry.Server.Http.App/"]
RUN dotnet restore "BaSyx.Registry.Server.Http.App/BaSyx.Registry.Server.Http.App.csproj"
COPY . .
WORKDIR "/src/BaSyx.Registry.Server.Http.App"
RUN dotnet build "BaSyx.Registry.Server.Http.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BaSyx.Registry.Server.Http.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaSyx.Registry.Server.Http.App.dll"]