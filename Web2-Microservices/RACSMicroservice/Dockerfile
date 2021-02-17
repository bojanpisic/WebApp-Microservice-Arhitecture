#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["RACSMicroservice/RACSMicroservice.csproj", "RACSMicroservice/"]
COPY ["EventBus/EventBus.csproj", "EventBus/"]
COPY ["EventBusRabbitMQ/EventBusRabbitMQ.csproj", "EventBusRabbitMQ/"]
RUN dotnet restore "RACSMicroservice/RACSMicroservice.csproj"
COPY . .
WORKDIR "/src/RACSMicroservice"
RUN dotnet build "RACSMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RACSMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RACSMicroservice.dll"]