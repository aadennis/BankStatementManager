using System;
using System.Windows.Input;

namespace WpfExecutor.ViewModels {
	public class DelegateCommand : ICommand {
		private readonly Action _execute;

		public DelegateCommand(Action executeAction) {
			_execute = executeAction;
		}

		public bool CanExecute(object parameter) {
			return true;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter) {
			_execute.Invoke();
		}
	}
}