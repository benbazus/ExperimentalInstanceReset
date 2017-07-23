using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VSExpInstanceReset
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Predicate<T> _predicate;

        public RelayCommand(Action<T> action)
            : this(action, null)
        {
        }

        public RelayCommand(Action<T> action, Predicate<T> predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            if (_predicate == null)
            {
                return true;
            }

            return _predicate.Invoke((T)parameter);
        }

        public void Execute(object parameter)
        {
            _action.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _predicate;

        public RelayCommand(Action action, Func<bool> predicate = null)
        {
            _action = action;
            _predicate = predicate;
        }

        public RelayCommand(Action action)
        {
            _action = action;
            _predicate = () => true;
        }

        public bool CanExecute(object parameter)
        {
            return _predicate == null || _predicate.Invoke();
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}