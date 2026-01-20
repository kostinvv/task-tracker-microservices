FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Scheduler/TaskTracker.Services.Scheduler/TaskTracker.Services.Scheduler.csproj", "src/Services/Scheduler/TaskTracker.Services.Scheduler/"]
COPY ["src/Services/Shared/TaskTracker.Services.Shared/TaskTracker.Services.Shared.csproj", "src/Services/Shared/TaskTracker.Services.Shared/"]
RUN dotnet restore "src/Services/Scheduler/TaskTracker.Services.Scheduler/TaskTracker.Services.Scheduler.csproj"
COPY . .
WORKDIR "/src/src/Services/Scheduler/TaskTracker.Services.Scheduler"
RUN dotnet build "./TaskTracker.Services.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskTracker.Services.Scheduler.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTracker.Services.Scheduler.dll"]
