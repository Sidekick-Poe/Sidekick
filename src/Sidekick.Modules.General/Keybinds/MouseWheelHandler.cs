using Sidekick.Common.Platform;
using Sidekick.Common.Settings;
using Sidekick.Common.Settings.Input;

namespace Sidekick.Modules.General.Keybinds;

public class MouseWheelHandler
(
    ISettingsService settingsService,
    IInputProvider inputProvider,
    IProcessProvider processProvider
) : ScrollWheelHandler(inputProvider)
{
    private readonly IInputProvider inputProvider = inputProvider;

    public override bool IsEnabled() => settingsService.GetBool(SettingKeys.MouseWheelNavigateStash);

    protected override bool IsValid() => processProvider.IsPathOfExileInFocus;

    protected override Task OnScrollUp()
    {
        inputProvider.PressKey(IsReversed() ? "Left" : "Right");
        return Task.CompletedTask;
    }

    protected override Task OnScrollDown()
    {
        inputProvider.PressKey(IsReversed() ? "Right" : "Left");
        return Task.CompletedTask;
    }

    private bool IsReversed() => settingsService.GetBool(SettingKeys.MouseWheelNavigateStashReverse);
}
