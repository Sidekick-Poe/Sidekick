FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine

# Dependencies
RUN apk add nodejs npm

# Build
WORKDIR /app
ENV HOME=/app
# Sidekick stores its data in the ApplicationData folder, which is $XDG_CONFIG_HOME on Linux, so data lands in /app/sidekick.
ENV XDG_CONFIG_HOME=/app
COPY . .
RUN dotnet publish src/Sidekick.Web/Sidekick.Web.csproj -c Release -o ./publish

# Run
EXPOSE 5000
VOLUME /app/sidekick
ENTRYPOINT ["/usr/bin/dotnet", "/app/publish/Sidekick.Web.dll", "--urls", "http://0.0.0.0:5000"]
