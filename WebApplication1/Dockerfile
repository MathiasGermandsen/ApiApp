FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["WebApplication1/WebApplication1.csproj", "WebApplication1/"]
RUN dotnet restore "WebApplication1/WebApplication1.csproj"

COPY . .
WORKDIR "/src/WebApplication1"
RUN dotnet build "WebApplication1.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src/WebApplication1
RUN dotnet publish "WebApplication1.csproj" -c Release -o /app/publish /p:UseAppHost=true

FROM alpine:3.18 AS compress
RUN apk add --no-cache upx
WORKDIR /compressed

COPY --from=publish /app/publish .

RUN find . -type f -name "*.dll" -exec upx --best --lzma {} \;

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=compress /compressed .

EXPOSE 80
ENTRYPOINT ["dotnet", "WebApplication1.dll"]