FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

# Dependencies
RUN apk add nodejs npm

# Build
WORKDIR /app
ENV HOME=/app
COPY . .
RUN dotnet publish src/Sidekick.Web/Sidekick.Web.csproj -c Release -o ./publish

# Run
EXPOSE 5000
VOLUME /app/sidekick
ENTRYPOINT ["/usr/bin/dotnet", "/app/publish/Sidekick.dll", "--urls", "http://0.0.0.0:5000"]
