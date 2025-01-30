FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine
RUN apk add nodejs npm

WORKDIR /app
ENV HOME=/app
COPY . /app/
RUN dotnet build /p:Configuration="Linux-Release"

WORKDIR /app/src/Sidekick.Web
VOLUME /app/src/Sidekick.Web/sidekick
EXPOSE 5000
ENTRYPOINT ["/usr/bin/dotnet"]
CMD ["bin/Linux-Release/net8.0/Sidekick.dll", "--urls", "http://*:5000"]
