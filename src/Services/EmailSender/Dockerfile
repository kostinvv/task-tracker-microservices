FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/EmailSender/TaskTracker.Services.EmailSender.Api/TaskTracker.Services.EmailSender.Api.csproj", "src/Services/EmailSender/TaskTracker.Services.EmailSender.Api/"]
COPY ["src/Services/EmailSender/TaskTracker.Services.EmailSender/TaskTracker.Services.EmailSender.csproj", "src/Services/EmailSender/TaskTracker.Services.EmailSender/"]
COPY ["src/Services/Shared/TaskTracker.Services.Shared/TaskTracker.Services.Shared.csproj", "src/Services/Shared/TaskTracker.Services.Shared/"]
RUN dotnet restore "src/Services/EmailSender/TaskTracker.Services.EmailSender.Api/TaskTracker.Services.EmailSender.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/EmailSender/TaskTracker.Services.EmailSender.Api"
RUN dotnet build "./TaskTracker.Services.EmailSender.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskTracker.Services.EmailSender.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTracker.Services.EmailSender.Api.dll"]
