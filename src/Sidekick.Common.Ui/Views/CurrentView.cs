using Microsoft.AspNetCore.Components;
using Sidekick.Common.Exceptions;

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
    public string Url => navigationManager.Uri;

    /// <inheritdoc/>
    public string? Key => Url.Split('/', '\\').FirstOrDefault(x => !string.IsNullOrEmpty(x));

    /// <inheritdoc/>
    public SidekickView? Current { get; private set; }

    /// <inheritdoc/>
    public void SetCurrent(SidekickView view)
    {
        Current = view;
    }

    /// <inheritdoc/>
    public void SetTitle(string? title)
    {
        if (Title == title)
        {
            return;
        }

        Title = $"Sidekick {title}".Trim();
        ViewChanged?.Invoke(this);
    }

    /// <summary>
    /// Closes the current view.
    /// </summary>
    /// <returns>A task.</returns>
    public Task Close()
    {
        if (Current == null)
        {
            throw new SidekickException("The view could not be closed.");
        }

        return viewLocator.Close(Current);
    }
}
