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
sudo pacman -S --needed xsel webkit2gtk-4.1
```

#### Ubuntu

```sh
sudo apt install xsel libwebkit2gtk-4.1-0
```

#### Fedora

```sh
sudo dnf install xsel webkit2gtk4.1
```

### Download and run

```sh
wget https://github.com/Sidekick-Poe/Sidekick/releases/latest/download/Sidekick-linux-stable.AppImage
chmod +x Sidekick-linux-stable.AppImage
./Sidekick-linux-stable.AppImage
```

### KDE Plasma on Wayland

The first time you price check, KDE asks whether Sidekick can control input devices ("Remote Control"). Sidekick needs this to copy the item from the game, so click **Allow** (check "Always allow" to not be asked again). If you clicked **Deny** or closed the prompt, restart Sidekick to be asked again.

## Development
We accept most PR and ideas. If you want a feature included, create an issue and we will discuss it. We are also available on [Discord](https://discord.gg/R9HyCpV).
#### Running the code:
1. Install the **.NET 10 SDK**
2. Install **Node** and **NPM** (the build uses them to generate the stylesheets and scripts)
3. Clone the repository
4. Open **Sidekick.sln** with your IDE of choice. Recommended IDEs are: **Visual Studio 2026**, **Rider** or **Visual Studio Code**
5. Run one of the two startup projects:
   - **Sidekick.Avalonia**: the desktop application.
   - **Sidekick.Web**: the same interface as a web application, opened in your browser.

   In **Visual Studio Code**, pick **Sidekick (desktop)** or **Sidekick (web)** in the Run and Debug panel and press F5. With the **dotnet CLI**, use `dotnet run --project src/Sidekick.Avalonia` or `dotnet run --project src/Sidekick.Web`.

Closing the desktop window only hides it in the system tray. Exit Sidekick from the tray icon before building again, otherwise the build fails because its files are in use.

#### Implementation Notes
The interface is a Blazor web application. The desktop application (Avalonia) hosts it in a native WebView (WebView2 on Windows, WebKitGTK on Linux). Development can also be done using the Web project.

#### Running webapp in docker
1. Clone the repository
2. Build an image using:
```powershell copy
docker build --tag sidekickpoe:latest --file Dockerfile .
```
3. Run it with:
```powershell copy
docker run -p 5000:5000 -v ./sidekick-data:/app/sidekick sidekickpoe:latest
```
4. Access through http://localhost:5000

#### Running webapp with docker compose (requires [Docker Compose](https://docs.docker.com/compose/install/))
1. Clone the repository
2. Build and run the project with `docker compose up` 
3. Access through: http://localhost:5000

Sidekick has no login, so anyone who can reach the published port can use it, including your settings. To keep it on your own machine only, publish the port as `127.0.0.1:5000:5000`. To use another port, change the first number, for example `8080:5000` for http://localhost:8080.

## Notice
This product isn't affiliated with or endorsed by Grinding Gear Games in any way.

## Thanks
#### Community
[Contributors](https://github.com/Sidekick-Poe/Sidekick/graphs/contributors), [Path of Exile Trade](https://www.pathofexile.com/trade), [poe2scout.com](https://poe2scout.com/), [poe.ninja](https://poe.ninja/), [poeprices.info](https://www.poeprices.info/), [poewiki.net](https://www.poewiki.net/), [poedb.tw](https://poedb.tw/us/), [Awakened PoE Trade](https://github.com/SnosMe/awakened-poe-trade), [POE-TradeMacro (Original Idea)](https://github.com/PoE-TradeMacro/POE-TradeMacro)

#### Technology
[FuzzySharp](https://github.com/JakeBayer/FuzzySharp), [NotificationIcon.NET](https://github.com/Bip901/NotificationIcon.NET), [SharpHook](https://github.com/TolikPylypchuk/SharpHook), [TextCopy](https://github.com/CopyText/TextCopy)
