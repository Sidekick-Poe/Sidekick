# Sidekick Agent Guidelines

This file documents the architectural decisions and coding conventions for the Sidekick project. Follow these guidelines when making changes.

## Project Structure

The solution is organized into three layers:

- **`Sidekick.Common`** — Shared abstractions and interfaces only. No implementations. Contains interfaces (`IProcessProvider`, `IClipboardProvider`, etc.), base types, extension methods on `IServiceCollection`, and cross-cutting concerns (cache, browser, folder providers).
- **`Sidekick.Common.*`** — Infrastructure implementations. Platform-specific code (Windows/Linux), settings, dialogs, database, and UI abstractions live here. Each project extends `Sidekick.Common` with concrete implementations.
- **`Sidekick.Game.*`** — Domain logic. Item parsing, trade filters, price checking, and game-specific providers.
- **`Sidekick.Apis.*`** — External API clients. Each API (PoeWiki, PoeNinja, PoePriceInfo, etc.) has its own project to keep dependencies isolated.
- **`Sidekick.Modules.*`** — Feature modules. Each module (Items, Chat, Logs, Wealth, etc.) is self-contained with its own `StartupExtensions.cs`, localization resources, and keybind handlers.
- **`Sidekick.Avalonia`** — Desktop shell. Hosts the Blazor UI via Avalonia WebView. Contains `Program.cs` and Linux-specific dependency checks.
- **`Sidekick.Web`** — Web shell. Blazor WebAssembly host project.

### Naming Convention

All projects follow the `Sidekick.<Layer>.<Feature>` naming pattern. Test projects mirror their source project: `Sidekick.<Layer>.<Feature>.Tests`.

## Platform Layer Rules

The platform layer (`Sidekick.Common.Platform`) provides OS-specific implementations for clipboard, input, process detection, and game log reading.

### Platform Isolation

- **Windows-only code** lives in `Sidekick.Common.Platform/Windows/`. This includes P/Invoke wrappers (`DllImport/`) and Windows-specific implementations (`Processes/ProcessProvider.cs`, `WindowsHotkeyBlocker.cs`).
- **Linux-only code** lives in `Sidekick.Common.Platform/Linux/`. This includes `WaylandSession`, `LinuxCommand`, `WaylandClipboard`, `ProcessProvider`, `LinuxHotkeyBlocker`, `LinuxDependencyChecker`, and the `X11/` subfolder.
- **X11 wrapper isolation** — All X11 interactions are encapsulated in `Linux/X11/`: `X11Connection.cs` (display connection, key grabs, window tracking), `Xlib.cs` (P/Invoke wrappers for libX11), and `Libc.cs` (P/Invoke wrappers for libc functions like poll, pipe, setenv). The X11 layer is pure — it knows nothing about Sidekick's hotkey format or the game. Sidekick-specific services (`LinuxHotkeyBlocker`, `ProcessProvider`) depend on `X11Connection`, not on raw X11 calls. Hotkey text to X11 key symbol mapping lives inside `LinuxHotkeyBlocker`, not in the X11 layer.
- **Cross-platform code** lives at the root of the platform layer or in feature-specific folders (`Clipboard/`, `Input/`, `GameLogs/`, `Interprocess/`).
- **Never scatter platform-specific code** across the shared abstractions project (`Sidekick.Common`). If a type is Linux-only or Windows-only, it belongs in `Sidekick.Common.Platform`.

### Platform-Specific Service Pattern

When a service behaves differently on each platform, define an interface and provide platform-specific implementations:
- **`IHotkeyBlocker`** — On Windows, `WindowsHotkeyBlocker` is a no-op (the global hook blocks hotkeys). On Linux, `LinuxHotkeyBlocker` uses `X11Connection` to grab keys while the game has focus.
- Both implementations extend `IInitializableService` for async startup.
- The orchestrator registers the correct implementation based on the platform.

### Platform Orchestration

All platform detection, initialization, and service selection is centralized in `PlatformOrchestrator`:

- **`PlatformOrchestrator.Initialize()`** — Call this once at the start of `Program.Main`, before building the app. It performs all platform-specific startup (e.g., setting environment variables for native libraries on Linux, initializing the Wayland session). It's a no-op on non-Linux platforms.
- **`PlatformOrchestrator.AddSidekickCommonPlatform()`** — Registers all platform-specific services in DI. There is no separate `StartupExtensions.cs` in the platform layer.
- Use `PlatformOrchestrator.IsWindows`, `PlatformOrchestrator.IsLinux`, or `PlatformOrchestrator.IsKdeWayland` for platform checks.
- Avoid inline `OperatingSystem.IsLinux()` checks in registration code — delegate to the orchestrator.

### Platform Attribute Marking

Mark platform-specific types with `[SupportedOSPlatform]` so the build catches unguarded calls:
- `[SupportedOSPlatform(PlatformNames.Windows)]` for Windows-only code.
- `[SupportedOSPlatform(PlatformNames.Linux)]` for Linux-only code.
- `PlatformNames` constants live in `Sidekick.Common/Platform/PlatformNames.cs` and are shared by both projects.

### X11 vs Wayland Boundary

The clipboard layer abstracts X11 and Wayland behind `ISystemClipboard`:
- `TextCopyClipboard` is cross-platform (Windows + X11 via xsel).
- `WaylandClipboard` is Linux-only and isolated in `Linux/`.
- `ClipboardProvider` orchestrates clipboard operations without knowing about X11 or Wayland details.

## Coding Style

### Syntax Preferences

- **Primary constructors** over traditional constructor injection. Prefer:
  ```csharp
  public class Service(ILogger<Service> logger, IDependency dep) { }
  ```
  over explicit constructor declarations.
- **Expression-bodied members** for simple properties and methods:
  ```csharp
  public bool IsWindows => OperatingSystem.IsWindows();
  ```
- **File-scoped namespaces** — no namespace braces.
- **Implicit usings enabled** — no explicit `using System;` unless needed for clarity.
- **Nullable reference types enabled** — always use `?` for nullable types.

### Documentation

- **XML documentation on all public members.** Every public class, interface, method, and property must have `/// <summary>` comments.
- **Comments explain the "why", not the "what".** Describe the reasoning behind a decision, not what the code literally does. For example:
  ```csharp
  /// <summary>
  /// How long to wait for wl-paste or wl-copy before giving up, so a stuck command can't block a price check.
  /// Both answer within a few milliseconds.
  /// </summary>
  ```
- **Avoid redundant comments.** If the code is self-explanatory, the comment should add context, not restate the code.

### Async Naming Conventions

- **Async methods end with `Async` suffix.** All async methods follow the .NET naming convention: `GetBoolAsync`, `GetStringAsync`, `SetAsync`, etc.
- **Sync fallbacks for hook code.** When code runs in keyboard/mouse hooks and cannot await, provide a synchronous alternative (e.g., `GetBool` alongside `GetBoolAsync`). The sync version reads from in-memory cache, falling back to the database only if the value was never loaded.

### Method and Class Design

- **One class per file.** Each class, interface, or record gets its own file. This makes navigation and refactoring straightforward.
- **Exception: parsing properties and property filters.** These are so intertwined that splitting them would create too many files to manage. They may share a file.
- **Static classes for stateless utilities.** E.g., `LinuxCommand`, `WaylandSession`, `PlatformOrchestrator`.
- **Extension methods for DI registration.** All `StartupExtensions` and `ServiceCollectionExtensions` use extension methods on `IServiceCollection`.
- **Interface-based abstractions.** Every service has a corresponding interface in `Sidekick.Common` (e.g., `IProcessProvider`, `IClipboardProvider`).

## Dependency Injection Patterns

### Service Registration

- Use `TryAddTransient`, `TryAddScoped`, or `TryAddSingleton` to avoid duplicate registrations.
- For services that need async initialization, use `AddSidekickInitializableService<TService, TImplementation>()` — this registers the service and adds it to the initialization pipeline.
- For input handlers (keybinds), use `AddSidekickInputHandler<T>()` — this registers the handler only for applications that support keybinds (Avalonia).
- For module registration, use `AddSidekickModule(assembly)` to track loaded modules.

### Initializable Services

Services that implement `IInitializableService` are initialized during startup in a defined order. Key examples:
- **`IProcessProvider`** — Must initialize before `IInputProvider` and `IHotkeyBlocker` because they depend on focus state.
- **`IInputProvider`** — Sets up global keyboard/mouse hooks via SharpHook.
- **`IHotkeyBlocker`** — On Linux, grabs hotkeys through X11 after the game gains focus; on Windows, does nothing.
- **`KeybindHandler`** — Abstract base class for keybind handlers; implements `IInputHandler` (which extends `IInitializableService`) and reads keybinds from settings.

### Keybind Handler Pattern

Keybind handlers extend `KeybindHandler` from `Sidekick.Common.Settings/Input/`:
- They read keybinds from settings via `ISettingsService`.
- They implement `IsValid(keybind)` to check if the handler should run, and `Execute(keybind)` to perform the action.
- They auto-update when settings change via `OnSettingsChanged` event.
- Register them using `AddSidekickInputHandler<T>()` so they're only loaded on Avalonia.

### Application Type Awareness

The application type (`SidekickApplicationType`) determines which features are available:
- `Avalonia` — Full desktop app with keybinds and authentication.
- `Web` — Blazor WebAssembly without keybinds or authentication.
- `Test` — Test environment with minimal services.

Use `SidekickApplicationType.SupportsKeybinds()` and `SupportsAuthentication()` to guard feature-specific code.

## Testing Conventions

### Framework

- **xUnit** for unit tests, **Moq** for mocking dependencies.
- Test projects mirror source project names: `Sidekick.Game.Parser.Tests`.

### Test Structure

- **Theory with InlineData** for parameterized tests:
  ```csharp
  [Theory]
  [InlineData(GameType.Poe1, ItemClass.Tincture)]
  [InlineData(GameType.Poe2, ItemClass.Spear)]
  public async Task RuleMatchesItemOfSelectedClass(GameType game, ItemClass itemClass) { }
  ```
- **Private helper classes** inside test files for test-specific implementations (e.g., `private class TestFilter : TradeFilter;`).
- **Mock dependencies** using Moq's `.Object` property for interface implementations.
- Test files are organized in folders mirroring the source structure (e.g., `Filters/AutoSelect/`).

### Test Fixtures

- Use fixture classes (e.g., `ParserFixture`) to share setup across test classes.
- Keep test data in dedicated collection files (e.g., `Poe2Collections.cs`, `Poe2EnglishFixtures.cs`).

## Localization

- All user-facing strings use `.resx` resource files with typed accessors generated by `Sidekick.Localization.Generators`.
- Each module has a `Localization` folder with its resource class (e.g., `TradeResources.cs`).
- The generator enforces translation coverage for configured cultures (French, Korean).
- `UiResources` is shared across projects; other resource classes are internal to their project.
- A localization analyzer checks enum keys, computed keys, and missing translations.

## Settings Service

- `ISettingsService` in `Sidekick.Common.Settings` provides typed accessors: `GetBoolAsync`, `GetStringAsync`, `GetIntAsync`, `GetDoubleAsync`, `GetDateTimeAsync`, `GetEnumAsync`, `GetObjectAsync`, and `SetAsync`.
- Settings keys are centralized in `SettingKeys.cs` — don't hardcode key strings.
- `OnSettingsChanged` event notifies listeners when settings are modified.
- `KeybindHandler` and `ScrollWheelHandler` are in `Sidekick.Common.Settings/Input/` and provide the base classes for input-related handlers.

## Package Management

- **Centralized package versions** via `Directory.Packages.props`. Never pin versions in individual `.csproj` files.
- **Treat warnings as errors** is enabled globally via `Directory.Build.props`. New warnings must be fixed, not suppressed.
- Security warnings (`NU1901`–`NU1904`) are excluded from error treatment to avoid breaking builds on newly published advisories.

## Build and Verification

- The solution targets **.NET 10.0**.
- Run `dotnet build Sidekick.slnx` to verify the full solution builds.
- Run `dotnet test` on individual test projects to verify tests pass.
- The `Sidekick.Avalonia` project has a known Blazor static web assets build error unrelated to code changes — verify by building `Sidekick.Common.Platform` and `Sidekick.Web` instead.

## Avalonia Desktop Shell

- `Sidekick.Avalonia` hosts the Blazor UI via Avalonia WebView. `Program.cs` calls `PlatformOrchestrator.Initialize()` at the start of `Main` to perform all platform-specific setup (e.g., setting `WEBKIT_DISABLE_DMABUF_RENDERER` on Linux and initializing the Wayland session).
- `PlatformOrchestrator.Initialize()` is the single entry point for platform startup. Call it before building the app. It's a no-op on non-Linux platforms.
- Linux dependency checking lives in `Linux/LinuxDependencyChecker.cs` in `Sidekick.Common.Platform` — it checks for `xsel` and WebKitGTK, and generates install commands for Arch, Debian/Ubuntu, and Fedora. The `LinuxDependenciesWindow` in Avalonia displays the missing dependencies with copy buttons for the install commands.
- Native methods for platform-specific operations are in `Utilities/WindowsNativeMethods.cs`. Linux native P/Invokes are split in `Linux/X11/`: `Xlib.cs` for libX11 and `Libc.cs` for libc functions (poll, pipe, read, write, close, setenv).

## Git and Commits

- Do not initiate commits unless explicitly requested.
