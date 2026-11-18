using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Sidekick.Common.Debugging;
using Sidekick.Common.Ui.Icons;
using Sidekick.Common.Ui.Localization;
namespace Sidekick.Common.Ui.Navigation;

public class NavigationItemProvider(
    IStringLocalizer<UiResources> resources,
    IOptions<SidekickConfiguration> configuration,
    DebugService debugService)
{
    public record NavigationItem(string Icon, string Label, string Route, bool IsSeparator = false);

    public List<NavigationItem> GetItems()
    {
        var result = new List<NavigationItem>();

        result.Add(new(UiIcons.Sword, resources.Items, "/item"));

        if (configuration.Value.ApplicationType.SupportsAuthentication())
        {
            result.Add(new NavigationItem(UiIcons.Wealth, resources.Wealth, "/wealth"));
        }

        if (debugService.IsDeveloperMode)
        {
            result.Add(new NavigationItem("", "", "", true));
            result.Add(new NavigationItem(UiIcons.Toolbox, resources.DevTools, "/development"));
        }

        result.Add(new NavigationItem("", "", "", true));
        result.Add(new NavigationItem(UiIcons.Cog, resources.Settings, "/settings"));
        result.Add(new NavigationItem(UiIcons.Help, resources.Help, "/help"));

        return result;
    }
}
