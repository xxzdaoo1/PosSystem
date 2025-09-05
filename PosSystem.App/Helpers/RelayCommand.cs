using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PosSystem.App.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeWithParameter;
        private readonly Action _executeWithoutParameter;
        private readonly Func<object, bool> _canExecuteWithParameter;
        private readonly Func<bool> _canExecuteWithoutParameter;

        // Constructor for methods WITH parameters (Action<object>)
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _executeWithParameter = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteWithParameter = canExecute;
        }

        // Constructor for methods WITHOUT parameters (Action)
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _executeWithoutParameter = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecuteWithoutParameter = canExecute;
        }

        // Constructor for method groups WITH parameters
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        // Constructor for method groups WITHOUT parameters
        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteWithParameter != null)
                return _canExecuteWithParameter(parameter);
            if (_canExecuteWithoutParameter != null)
                return _canExecuteWithoutParameter();
            return true;
        }

        public void Execute(object parameter)
        {
            if (_executeWithParameter != null)
                _executeWithParameter(parameter);
            else
                _executeWithoutParameter?.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}