using System.Windows.Input;

namespace Sidekick.Wpf.Services
{
    public class SimpleCommand : ICommand
    {
        private readonly Action execute;

        public SimpleCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute.Invoke();

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
