#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-jammy AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build
WORKDIR /src
COPY ["TestSocketServer/TestSocketServer.csproj", "TestSocketServer/"]
RUN dotnet restore "TestSocketServer/TestSocketServer.csproj"
COPY . .
WORKDIR "/src/TestSocketServer"
RUN dotnet build "TestSocketServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestSocketServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestSocketServer.dll"]