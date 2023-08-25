using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazored.Modal;
using Microsoft.JSInterop;
using Sidekick.Wpf.Components;

namespace Sidekick.Wpf;

public partial class Index
{
    private Canvas2DContext _context;
    protected BECanvasComponent _canvasReference;

    private string message = "Hey, it's Blazor in WPF!";

    public string Message
    {
        get => message;
        set
        {
            message = value;
            StateHasChanged();
        }
    }

    protected async Task ClickMe()
    {
        var width = await JSRuntime.InvokeAsync<int>("GetWindowWidth", null);
        Message = $"Message updated at {DateTime.Now.ToLongTimeString()} Window Width: {width}";
    }

    protected override void OnInitialized()
    {
        AppState.IndexComponent = this;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("RegisterWPFApp",
               DotNetObjectReference.Create(this));
        }
        _context = await _canvasReference.CreateCanvas2DAsync();
        await _context.SetFillStyleAsync("green");

        await _context.FillRectAsync(10, 100, 100, 100);

        await _context.SetFontAsync("48px serif");
        await _context.StrokeTextAsync("Hello Blazor!!!", 10, 100);
    }

    [JSInvokable]
    public async Task WindowResized(int Width, int Height)
    {
        Message = $"Screen Size: {Width} x {Height}";
        await InvokeAsync(StateHasChanged);
    }

    public void ShowModal()
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(DisplayMessage.Message), "Hey, WPF!");
        Modal.Show<DisplayMessage>("Passing Data", parameters);
    }

    protected void TalkToWPF()
    {
        AppState.MainWindow?.ShowMessageBox("Hey, this is Blazor Calling!");
    }
}
