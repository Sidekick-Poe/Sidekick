using System.Windows.Input;

namespace Sidekick.Wpf.Services;

public class SimpleCommand(Action execute) : ICommand
{
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => execute.Invoke();

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}
