FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Tasks/TaskTracker.Services.Tasks.Api/TaskTracker.Services.Tasks.Api.csproj", "src/Services/Tasks/TaskTracker.Services.Tasks.Api/"]
COPY ["src/Services/Tasks/TaskTracker.Services.Tasks.ApplicationCore/TaskTracker.Services.Tasks.ApplicationCore.csproj", "src/Services/Tasks/TaskTracker.Services.Tasks.ApplicationCore/"]
COPY ["src/Services/Shared/TaskTracker.Services.Shared/TaskTracker.Services.Shared.csproj", "src/Services/Shared/TaskTracker.Services.Shared/"]
COPY ["src/Services/Tasks/TaskTracker.Services.Tasks.Infrastructure/TaskTracker.Services.Tasks.Infrastructure.csproj", "src/Services/Tasks/TaskTracker.Services.Tasks.Infrastructure/"]
RUN dotnet restore "src/Services/Tasks/TaskTracker.Services.Tasks.Api/TaskTracker.Services.Tasks.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Tasks/TaskTracker.Services.Tasks.Api"
RUN dotnet build "./TaskTracker.Services.Tasks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskTracker.Services.Tasks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTracker.Services.Tasks.Api.dll"]
