#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt update
RUN apt-get install -y ffmpeg=7:4.3.5-0+deb11u1
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build 
WORKDIR /src
COPY ["LivestreamBot/LivestreamBot.csproj", "LivestreamBot/"]
RUN dotnet restore "LivestreamBot/LivestreamBot.csproj"
COPY . .
WORKDIR "/src/LivestreamBot"
RUN dotnet build "LivestreamBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LivestreamBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LivestreamBot.dll"]