# Stage 1: Build the Vue/SPA frontend
FROM node:24-alpine AS frontend
WORKDIR /web-client
COPY web-client/package*.json ./
RUN npm ci
COPY web-client/ .
RUN npm run build

# Stage 2: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files and restore dependencies
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["BLL/BLL.csproj", "BLL/"]
COPY ["DAL/DAL.csproj", "DAL/"]
RUN dotnet restore "Presentation/Presentation.csproj"

# Copy all source code
COPY . .

# Copy the built frontend assets to wwwroot
COPY --from=frontend /web-client/dist ./Presentation/wwwroot/

# Build the application
WORKDIR "/src/Presentation"

RUN dotnet build "./Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "Presentation.dll"]