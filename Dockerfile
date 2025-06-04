FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY MagicPacket/MagicPacket.csproj MagicPacket/
RUN dotnet restore MagicPacket/MagicPacket.csproj
COPY MagicPacket/ MagicPacket/
WORKDIR /src/MagicPacket
RUN dotnet publish MagicPacket.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MagicPacket.dll"]
