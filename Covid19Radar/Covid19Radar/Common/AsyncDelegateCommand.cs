using System;
using System.Threading.Tasks;
using Prism.Commands;

namespace Covid19Radar.Common
{
    public class AsyncDelegateCommand : DelegateCommandBase
    {
        private readonly Func<Task> _executeMethod;
        private readonly Func<bool> _canExecuteMethod;

        private bool _isRunning;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        public AsyncDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod = null) : base()
        {
            _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            _canExecuteMethod = canExecuteMethod;
        }

        protected override bool CanExecute(object parameter)
        {
            return !IsRunning && (_canExecuteMethod?.Invoke() ?? true);
        }

        protected override async void Execute(object parameter)
        {
            try
            {
                IsRunning = true;
                await _executeMethod();
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
