FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/ ./
RUN dotnet restore
WORKDIR "/app/DotNetCore.SslSocket.Server"
RUN dotnet publish -c Release -o "publish"
RUN mkdir publish/Certs
COPY src//DotNetCore.SslSocket.Server/Certs/ publish/Certs/

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env "/app/DotNetCore.SslSocket.Server/publish" .
EXPOSE 6667
ENTRYPOINT ["dotnet", "DotNetCore.SslSocket.Server.dll"]