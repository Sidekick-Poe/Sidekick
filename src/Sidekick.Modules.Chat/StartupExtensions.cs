using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Chat.Keybinds;

namespace Sidekick.Modules.Chat;

/// <summary>
/// Startup configuration functions for the chat module
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds the chat module services to the service collection
    /// </summary>
    /// <param name="services">The services collection to add services to</param>
    /// <returns>The service collection with services added</returns>
    public static IServiceCollection AddSidekickChat(this IServiceCollection services)
    {
        services.AddSidekickModule(typeof(StartupExtensions).Assembly);

        services.AddSidekickKeybind<ChatKeybindHandler>();
        return services;
    }
}
