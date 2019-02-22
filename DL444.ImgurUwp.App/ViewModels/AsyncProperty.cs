using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL444.ImgurUwp.App.ViewModels
{
    // See https://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    public sealed class AsyncProperty<TResult> : INotifyPropertyChanged
    {
        public Task<TResult> Task { get; private set; }
        public TResult Default { get; private set; }
        public TResult Value => Task.Status == TaskStatus.RanToCompletion ? Task.Result : Default;
        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsCompletedSuccessfully => Task.IsCompletedSuccessfully;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => (Exception == null) ? null : Exception.InnerException;
        public string ErrorMessage => (InnerException == null) ? null : InnerException.Message;

        public AsyncProperty(Task<TResult> task, TResult defaultValue = default(TResult))
        {
            Default = defaultValue;
            Task = task;
            if(!Task.IsCompleted)
            {
                var _ = WatchTaskAsync(Task);
            }
        }

        async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch { }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));

            if (task.IsCanceled)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InnerException)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompletedSuccessfully)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
