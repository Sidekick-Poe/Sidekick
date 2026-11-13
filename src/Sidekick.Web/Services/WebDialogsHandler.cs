using Microsoft.JSInterop;
using Sidekick.Common.Dialogs;

namespace Sidekick.Web.Services;

public sealed class WebDialogsHandler : IDisposable
{
    private readonly DialogProvider dialogProvider;
    private readonly IJSRuntime jsRuntime;

    public WebDialogsHandler(
        DialogProvider dialogProvider,
        IJSRuntime jsRuntime)
    {
        this.dialogProvider = dialogProvider;
        this.jsRuntime = jsRuntime;

        dialogProvider.Opened += DialogOpened;
    }

    private void DialogOpened(DialogProvider.OpenedArgs args)
    {
        if (args.Type == DialogProvider.Type.Ok)
        {
            _ = Task.Run(async () =>
            {
                await jsRuntime.InvokeVoidAsync("alert", args.Message);
                args.TaskCompletion.TrySetResult(DialogProvider.Result.Closed);
            });
        }
        else
        {
            _ = Task.Run(async () =>
            {
                var result = await jsRuntime.InvokeAsync<bool>("confirm", args.Message);
                args.TaskCompletion.TrySetResult(result ? DialogProvider.Result.Confirmed : DialogProvider.Result.Closed);
            });
        }
    }

    public void Dispose()
    {
        dialogProvider.Opened -= DialogOpened;
    }
}
