# Development Guidelines

## Coding Standards
- Follow standard C# and .NET coding conventions.
- Use meaningful names for variables, methods, and classes.
- Prefer `async`/`await` for I/O bound operations.
- Keep components small and focused on a single responsibility.

## Localization
- Sidekick supports multiple languages.
- Use the `Localization` directories within modules to add or update strings.
- Blazor components should use the `IStringLocalizer` or specific localization services.

## UI Styling
- Styles are managed through CSS, often using Tailwind-like classes.
- Ensure that UI elements are visible and legible against both PoE and PoE 2 backgrounds.

## Adding a New Module
1. Create a new project in `src/Sidekick.Modules.YourModuleName`.
2. Add a `StartupExtensions` class with an `AddSidekickYourModule` extension method.
3. Call `services.AddSidekickModule(typeof(StartupExtensions).Assembly)` within that method.
4. Register the module in `Program.cs` in `Sidekick.Wpf` (and other host projects if applicable).

## Keybinds
- Register new keybind handlers using `services.AddSidekickInputHandler<THandler>()`.
- Handlers should implement `IInputHandler`.
