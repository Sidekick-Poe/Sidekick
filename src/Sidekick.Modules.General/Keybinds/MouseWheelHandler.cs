using Sidekick.Common.Platform;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Modules.General.Keybinds;

public class MouseWheelHandler
(
    ISettingsService settingsService,
    IInputProvider inputProvider,
    IProcessProvider processProvider
) : ScrollWheelHandler(settingsService, inputProvider)
{
    private readonly ISettingsService settingsService = settingsService;
    private readonly IInputProvider inputProvider = inputProvider;

    private bool MouseWheelNavigateStashReverse { get; set; }

    protected override async Task<bool> GetEnabled()
    {
        MouseWheelNavigateStashReverse = await settingsService.GetBool(SettingKeys.MouseWheelNavigateStashReverse);
        return await settingsService.GetBool(SettingKeys.MouseWheelNavigateStash);
    }

    protected override bool IsValid() => processProvider.IsPathOfExileInFocus;

    protected override Task OnScrollUp()
    {
        inputProvider.PressKey(MouseWheelNavigateStashReverse ? "Left" : "Right");
        return Task.CompletedTask;
    }

    protected override Task OnScrollDown()
    {
        inputProvider.PressKey(MouseWheelNavigateStashReverse ? "Right" : "Left");
        return Task.CompletedTask;
    }
}
