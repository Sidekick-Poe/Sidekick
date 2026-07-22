using System.Windows.Input;
namespace Sidekick.Avalonia.Utilities;

public class SimpleCommand(Action execute) : ICommand
{
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => execute.Invoke();

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }
}
