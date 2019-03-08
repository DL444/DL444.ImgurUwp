using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DL444.ImgurUwp.App.ViewModels
{
    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        readonly Func<Task<TResult>> command;
        readonly Func<bool> canExecute;
        private AsyncProperty<TResult> _execution;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSingleton { get; private set; }
        public AsyncProperty<TResult> Execution
        {
            get => _execution;
            private set
            {
                _execution = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Execution)));
            }
        }

        public AsyncCommand(Func<Task<TResult>> command, Func<bool> canExecute, bool isSingleton)
        {
            this.command = command;
            this.canExecute = canExecute;
            IsSingleton = isSingleton;
        }
        public AsyncCommand(Func<Task<TResult>> command, bool isSingleton) : this(command, () => true, isSingleton) { }
        public AsyncCommand(Func<Task<TResult>> command, Func<bool> canExecute) : this(command, canExecute, false) { }
        public AsyncCommand(Func<Task<TResult>> command) : this(command, () => true) { }

        public override bool CanExecute(object parameter)
        {
            if(IsSingleton)
            {
                if (Execution == null || Execution.IsCompleted) { return canExecute(); }
                else { return false; }
            }
            else
            {
                return canExecute();
            }
        }
        public override async Task ExecuteAsync(object parameter)
        {
            Execution = new AsyncProperty<TResult>(command());
            RaiseCanExecuteChanged();
            await Execution.CompletionWatch;
            RaiseCanExecuteChanged();
        }
    }

    public abstract class AsyncCommandBase : IAsyncCommand
    {
        // Call this manually when it changed.
        public event EventHandler CanExecuteChanged;
        public abstract Task ExecuteAsync(object parameter);
        public abstract bool CanExecute(object parameter);
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }
    }
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
