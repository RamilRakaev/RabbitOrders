FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifications.Worker/Notifications.Worker.csproj", "Notifications.Worker/"]
COPY ["Orders.Application/Orders.Application.csproj", "Orders.Application/"]
COPY ["Orders.Infrastructure.RabbitMq/Orders.Infrastructure.RabbitMq.csproj", "Orders.Infrastructure.RabbitMq/"]
RUN dotnet restore "./Notifications.Worker/Notifications.Worker.csproj"
COPY . .
WORKDIR "/src/Notifications.Worker"
RUN dotnet build "./Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.Worker.dll"]
