FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine
RUN apk add nodejs npm

WORKDIR /app
ENV HOME=/app
COPY . /app/
RUN sed -i '/Sidekick.Wpf/,+1d' Sidekick.sln
RUN dotnet build --configuration Release

WORKDIR /app/src/Sidekick.Web
VOLUME /app/src/Sidekick.Web/sidekick
EXPOSE 5000
ENTRYPOINT ["/usr/bin/dotnet"]
CMD ["bin/Release/net8.0/Sidekick.dll", "--urls", "http://*:5000"]
