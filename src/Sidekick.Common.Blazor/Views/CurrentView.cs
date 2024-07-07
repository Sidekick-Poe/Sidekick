using Sidekick.Common.Exceptions;

namespace Sidekick.Common.Blazor.Views;

/// <summary>
/// Interface to manage views
/// </summary>
public class CurrentView(IViewLocator viewLocator) : ICurrentView
{
    /// <inheritdoc/>
    public SidekickView? Current { get; private set; }

    /// <inheritdoc/>
    public void SetCurrent(SidekickView view)
    {
        Current = view;
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
