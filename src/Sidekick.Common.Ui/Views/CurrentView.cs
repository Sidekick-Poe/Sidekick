using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public class CurrentView : ICurrentView, IDisposable
{
    private readonly IViewLocator viewLocator;
    private readonly NavigationManager navigationManager;

    public CurrentView(IViewLocator viewLocator, NavigationManager navigationManager)
    {
        this.viewLocator = viewLocator;
        this.navigationManager = navigationManager;
        navigationManager.LocationChanged += NavigationManagerOnLocationChanged;
    }

    /// <inheritdoc/>
    public event Action<ICurrentView>? ViewChanged;

    /// <inheritdoc/>
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Title { get; private set; } = "Sidekick";

    /// <inheritdoc/>
    public int? Width { get; private set; }

    /// <inheritdoc/>
    public int? Height { get; private set; }

    /// <inheritdoc/>
    public int? MinWidth { get; private set; }

    /// <inheritdoc/>
    public int? MinHeight { get; private set; }

    /// <inheritdoc/>
    public string Url => navigationManager.Uri;

    /// <inheritdoc/>
    public string? Key { get; private set; }

    /// <inheritdoc/>
    public SidekickView? Current { get; private set; }

    /// <inheritdoc/>
    public bool IsInitialized { get; private set; }

    /// <inheritdoc/>
    public void SetTitle(string? title)
    {
        if (Title == title)
        {
            return;
        }

        Title = title?.Trim() ?? "Sidekick";
        ViewChanged?.Invoke(this);
    }

    /// <inheritdoc/>
    public void SetSize(int? width = null, int? height = null, int? minWidth = null, int? minHeight = null)
    {
        Width = width;
        Height = height;
        MinWidth = minWidth;
        MinHeight = minHeight;
        ViewChanged?.Invoke(this);
    }

    /// <inheritdoc/>
    public async Task Initialize(SidekickView view)
    {
        Key ??= new Uri(Url).AbsolutePath.Split('/', '\\').FirstOrDefault(x => !string.IsNullOrEmpty(x));
        IsInitialized = true;
        Current = view;
        await viewLocator.Initialize(view);
    }

    /// <inheritdoc/>
    public Task Close()
    {
        return Current == null ? Task.CompletedTask : viewLocator.Close(Current);
    }

    private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        Width = null;
        Height = null;
        MinWidth = null;
        MinHeight = null;
    }

    public void Dispose()
    {
        navigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
    }
}
