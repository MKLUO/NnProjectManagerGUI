using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Diagnostics;

namespace NnManagerGUI.ViewModel {
    public class RelayCommand : ICommand {
        readonly Func<bool> _canExecute;
        readonly Action _execute;

        public RelayCommand(Action execute)
            : this(execute, () => true) { }

        public RelayCommand(Action execute, Func<bool> canExecute) {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) => _canExecute();

        public void Execute(object parameter) => _execute();
    }
}
