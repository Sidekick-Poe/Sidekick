# Architecture

Sidekick follows a modular and service-oriented architecture, leveraging .NET's Dependency Injection (DI) system.

## Modular System
The project is divided into several modules, each with a specific responsibility. Modules are typically located in `src/Sidekick.Modules.*`.

- **Sidekick.Common**: Contains core interfaces, shared services, and base logic.
- **Sidekick.Apis.***: Individual API clients for external services.
- **Sidekick.Modules.***: Feature-specific modules (e.g., Items, Maps, Wealth, Trade).

## Dependency Injection and Initialization
Modules register themselves in the DI container using `StartupExtensions` or `ServiceCollectionExtensions`.

- **IInitializableService**: Services that require initialization at startup should implement this interface and be registered using `AddSidekickInitializableService`.
- **AddSidekickModule**: Registers an assembly as a Sidekick module.
- **AddSidekickInputHandler**: Registers a handler for global inputs (keybinds).

## UI Architecture
The UI is built with Blazor components.
- **Views**: Typically located in the `Views` directory of a module.
- **Components**: Reusable UI elements.
- **Overlays**: Sidekick uses transparent WPF windows to host Blazor-based overlays on top of the game.

## Communication
- **Mediator Pattern**: Often used for decoupling components (though check if `MediatR` is actually used or if it's custom).
- **Service Layer**: Modules expose functionality through services (e.g., `TradeService`).
