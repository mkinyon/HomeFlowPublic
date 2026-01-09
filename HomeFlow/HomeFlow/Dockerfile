# Use the ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Ensure SQLite database directory exists
VOLUME ["/app/LocalStorage"]

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HomeFlow/HomeFlow/HomeFlow.csproj", "HomeFlow/HomeFlow/"]
RUN dotnet restore "./HomeFlow/HomeFlow/HomeFlow.csproj"
COPY . .

RUN mkdir -p /src/HomeFlow/HomeFlow/LocalStorage

WORKDIR "/src/HomeFlow/HomeFlow"
RUN dotnet build "./HomeFlow.csproj" -c $BUILD_CONFIGURATION

# This stage is used to publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HomeFlow.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Running container
FROM base AS final

# Copy app files
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure SQLite database file is persisted
RUN mkdir -p /app/LocalStorage
RUN chmod 755 /app/LocalStorage
VOLUME ["/app/LocalStorage"]

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Configure Kestrel to use HTTP only
ENV ASPNETCORE_Kestrel__Endpoints__Http__Url=http://+:8080

# Drop privileges (if needed)
# USER $APP_UID  # Commented out as APP_UID might not be defined

ENTRYPOINT ["dotnet", "HomeFlow.dll"]