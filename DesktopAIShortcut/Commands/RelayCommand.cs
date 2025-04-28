using System;
using System.Windows.Input;

namespace DesktopAIShortcut.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public static class CommandManager
    {
        internal static event EventHandler RequerySuggested;

        public static void AddRequerySuggestedHandler(EventHandler handler)
        {
            RequerySuggested += handler;
        }

        public static void RemoveRequerySuggestedHandler(EventHandler handler)
        {
            RequerySuggested -= handler;
        }

        public static void InvalidateRequerySuggested()
        {
            RequerySuggested?.Invoke(null, EventArgs.Empty);
        }
    }
}
