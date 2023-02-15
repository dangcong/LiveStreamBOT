#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt update
RUN apt-get install -y procps
RUN apt-get install -y ffmpeg
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build 
WORKDIR /src
COPY ["LiveStreamBOT/LiveStreamBOT.csproj", "LiveStreamBOT/"]
RUN dotnet restore "LiveStreamBOT/LiveStreamBOT.csproj"
COPY . .
WORKDIR "/src/LiveStreamBOT"
RUN dotnet build "LiveStreamBOT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LiveStreamBOT.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LiveStreamBOT.dll"]