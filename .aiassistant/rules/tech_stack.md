# Tech Stack

Sidekick is built using modern .NET technologies and a web-based UI layer.

## Core Technologies
- **.NET 8**: The primary framework for the application.
- **WPF (Windows Presentation Foundation)**: Used as the host for the desktop application on Windows.
- **WebView2**: Used within WPF to host the Blazor-based UI.
- **Blazor (Razor Components)**: The UI framework used for building the application's interface.
- **Serilog**: For logging.
- **Velopack**: For application updates and installation.
- **SharpHook**: For global keyboard and mouse hooks.

## UI/UX
- **Tailwind CSS**: Likely used for styling (implied by Node/NPM and CSS classes in Razor files).
- **ApexCharts**: Used for data visualization (e.g., in Wealth module).

## APIs and Integrations
- **Path of Exile Trade API**: For item pricing.
- **poe.ninja API**: For currency and item valuation data.
- **poe2scout.com**: Integration for PoE 2 specific data.
- **PoeWiki**: For in-game wiki access.
- **GitHub API**: For updates and repository information.

## Deployment
- **Docker**: Supported for running the web application in a containerized environment.
- **WASM**: There is a `Sidekick.Wasm` project, indicating experimental or planned support for WebAssembly.
