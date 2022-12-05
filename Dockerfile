FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
WORKDIR /src
COPY ["RDSProxyDemo.csproj", "./"]
RUN dotnet restore
COPY . ./
WORKDIR "/src"
RUN dotnet build "RDSProxyDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RDSProxyDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RDSProxyDemo.dll"]