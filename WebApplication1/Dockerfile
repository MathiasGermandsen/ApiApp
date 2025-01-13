# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy and restore dependencies (use layers effectively)
COPY ["WebApplication1/WebApplication1.csproj", "./"]
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
RUN dotnet publish "WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore --p:UseAppHost=false

# Use a smaller runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose the necessary ports
EXPOSE 80
EXPOSE 443

# Run the application
ENTRYPOINT ["dotnet", "WebApplication1.dll"]