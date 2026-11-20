using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Sidekick.Common.Debugging;
using Blazicons;
using Sidekick.Common.Ui.Localization;
namespace Sidekick.Common.Ui.Navigation;

public class NavigationItemProvider(
    IStringLocalizer<UiResources> resources,
    IOptions<SidekickConfiguration> configuration,
    DebugService debugService)
{
    public abstract record NavigationEntry;

    public record NavigationItem(SvgIcon Icon, string Label, string Route) : NavigationEntry;

    public record NavigationSeparator : NavigationEntry;

    public List<NavigationEntry> GetItems()
    {
        var result = new List<NavigationEntry>();

        result.Add(new NavigationItem(Lucide.Sword, resources.Items, "/item"));

        if (configuration.Value.ApplicationType.SupportsAuthentication())
        {
            result.Add(new NavigationItem(Lucide.PiggyBank, resources.Wealth, "/wealth"));
        }

        if (debugService.IsDeveloperMode)
        {
            result.Add(new NavigationSeparator());
            result.Add(new NavigationItem(Lucide.BugPlay, resources.DevTools, "/development"));
        }

        result.Add(new NavigationSeparator());
        result.Add(new NavigationItem(Lucide.Settings, resources.Settings, "/settings"));
        result.Add(new NavigationItem(Lucide.CircleQuestionMark, resources.Help, "/help"));

        return result;
    }
}
