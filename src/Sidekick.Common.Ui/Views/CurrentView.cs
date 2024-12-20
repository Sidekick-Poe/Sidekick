using Microsoft.AspNetCore.Components;

namespace Sidekick.Common.Ui.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public class CurrentView(
    IViewLocator viewLocator,
    NavigationManager navigationManager) : ICurrentView
{
    /// <inheritdoc/>
    public event Action<ICurrentView>? ViewChanged;

    /// <inheritdoc/>
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Title { get; private set; } = "Sidekick";

    /// <inheritdoc/>
    public int? Height { get; private set; }

    /// <inheritdoc/>
    public string Url => navigationManager.Uri;

    /// <inheritdoc/>
    public string? Key => new Uri(Url).AbsolutePath.Split('/', '\\').FirstOrDefault(x => !string.IsNullOrEmpty(x));

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
    public void SetHeight(int height)
    {
        Height = height;
        ViewChanged?.Invoke(this);
    }

    /// <inheritdoc/>
    public async Task Initialize(SidekickView view)
    {
        IsInitialized = true;
        Current = view;
        await viewLocator.Initialize(view);
    }

    /// <inheritdoc/>
    public Task Close()
    {
        return Current == null ? Task.CompletedTask : viewLocator.Close(Current);
    }
}
