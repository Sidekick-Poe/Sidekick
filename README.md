# [![](https://sidekick-poe.github.io/assets/images/orb_exalted.png)](#) Sidekick

[![](https://img.shields.io/github/v/release/Sidekick-Poe/Sidekick?style=flat-square)](https://github.com/Sidekick-Poe/Sidekick/releases/latest) [![Download Statistics](https://img.shields.io/github/downloads/Sidekick-Poe/Sidekick/latest/total?style=flat-square&color=15803d)](https://tooomm.github.io/github-release-stats/?username=Sidekick-Poe&repository=Sidekick) [![Download Statistics](https://img.shields.io/github/downloads/Sidekick-Poe/Sidekick/total?style=flat-square&color=22c55e)](https://tooomm.github.io/github-release-stats/?username=Sidekick-Poe&repository=Sidekick) [![](https://img.shields.io/discord/664252463188279300?color=%23738AD6&label=Discord&style=flat-square)](https://discord.gg/R9HyCpV)

A Path of Exile and Path of Exile 2 companion tool. Price check items, check for dangerous map modifiers, and more!

[![Website](https://img.shields.io/badge/Website-6b6ebe?style=for-the-badge)](https://sidekick-poe.github.io/) [![Website](https://img.shields.io/badge/Download-00BCD4?style=for-the-badge)](https://github.com/Sidekick-Poe/Sidekick/releases/latest)

[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/sidekickpoe)

## Running Sidekick AppImage on Linux

### Prerequisites
Sidekick requires the following packages to be installed

#### Arch-based

```sh
sudo pacman -S --needed \
  xsel webkit2gtk-4.1 dotnet-runtime-8.0 \
  libx11 libxext libxrandr libxtst libxkbcommon libxt libxinerama \
  libxcb libsm libice libxau libxdmcp util-linux-libs libbsd libmd
```

#### Ubuntu

```sh
sudo apt install \
  xsel libwebkit2gtk-4.1-0 dotnet-runtime-8.0 \
  libx11-6 libxrandr2 libxext6 libxtst6 libxkbcommon0 libxt6 libxinerama1 \
  libxcb1 libsm6 libice6 libxau6 libxdmcp6 libuuid1 libbsd0 libmd0
```

#### Debian

```sh
sudo apt install \
  xsel libwebkit2gtk-4.0-37 dotnet-runtime-8.0 \
  libx11-6 libxrandr2 libxext6 libxtst6 libxkbcommon0 libxt6 libxinerama1 \
  libxcb1 libsm6 libice6 libxau6 libxdmcp6 libuuid1 libbsd0 libmd0
```

#### Fedora

```sh
sudo dnf install \
  xsel webkit2gtk4.1 dotnet-runtime-8.0 \
  libX11 libXext libXrandr libXtst libxkbcommon libXt libXinerama \
  libxcb libSM libICE libXau libXdmcp libuuid libbsd libmd
```

### Download and run

```sh
wget https://github.com/Sidekick-Poe/Sidekick/releases/latest/download/Sidekick-linux-stable.AppImage
chmod +x Sidekick-linux-stable.AppImage
./Sidekick-linux-stable.AppImage
```

## Development
We accept most PR and ideas. If you want a feature included, create an issue and we will discuss it. We are also available on [Discord](https://discord.gg/R9HyCpV).
#### Running the code:
1. Install **.NET SDK**
2. Install **Node** and **NPM** (for the stylesheets)
3. Clone the repository
4. Open **.sln** with your IDE of choice. Recommended IDEs are: **Visual Studio 2022**, **Rider** or **Visual Studio Code**
5. Run the **WPF** project (VS2022 or Rider) or **Web** project. Can also be done with **dotnet CLI**

#### Implementation Notes
The application is a web application that is running inside a WebView2 provided by WPF. Development can also be done using the Web project.

#### Running webapp in docker
1. Clone the repository
2. Build an image using:
```powershell copy
docker build --tag sidekickpoe:latest --file Dockerfile .
```
4. Run it with:
```powershell copy
docker run -p 5000:5000 -v ./sidekick-data:/app/src/Sidekick.Web/sidekick sidekickpoe:latest
```
5. Access through http://localhost:5000

#### Running webapp with docker compose (requires [Docker Compose](https://docs.docker.com/compose/install/))
1. Clone the repository
2. Build and run the project with `docker compose up` 
3. Access through: http://localhost:5000

## Notice
This product isn't affiliated with or endorsed by Grinding Gear Games in any way.

## Thanks
#### Community
[Contributors](https://github.com/Sidekick-Poe/Sidekick/graphs/contributors), [Path of Exile Trade](https://www.pathofexile.com/trade), [poe2scout.com](https://poe2scout.com/), [poe.ninja](https://poe.ninja/), [poeprices.info](https://www.poeprices.info/), [poewiki.net](https://www.poewiki.net/), [poedb.tw](https://poedb.tw/us/), [Awakened PoE Trade](https://github.com/SnosMe/awakened-poe-trade), [POE-TradeMacro (Original Idea)](https://github.com/PoE-TradeMacro/POE-TradeMacro)

#### Technology
[FuzzySharp](https://github.com/JakeBayer/FuzzySharp), [NotificationIcon.NET](https://github.com/Bip901/NotificationIcon.NET), [SharpHook](https://github.com/TolikPylypchuk/SharpHook), [TextCopy](https://github.com/CopyText/TextCopy)
