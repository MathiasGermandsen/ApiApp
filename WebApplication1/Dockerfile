# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy only the project file and restore dependencies
COPY ["WebApplication1/WebApplication1.csproj", "WebApplication1/"]
RUN dotnet restore "WebApplication1/WebApplication1.csproj"

# Copy the rest of the source code and build the application
COPY . .
WORKDIR "/src/WebApplication1"
RUN dotnet build "WebApplication1.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
WORKDIR /src/WebApplication1
RUN dotnet publish "WebApplication1.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:PublishTrimmed=true /p:PublishSingleFile=true /p:EnableCompressionInSingleFile=true

# Stage 3: Final (Minimal Runtime Image)
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0 AS final
WORKDIR /app

# Copy only the published output from the previous stage
COPY --from=publish /app/publish .

# Expose port 80 and set the entry point
EXPOSE 80
ENTRYPOINT ["./WebApplication1"]
