using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DL444.ImgurUwp.App.ViewModels
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public class Command : CommandBase
    {
        Action command;
        Func<bool> canExecute;
        public Command(Action command, Func<bool> canExecute)
        {
            this.command = command;
            this.canExecute = canExecute;
        }
        public Command(Action command) : this(command, () => true) { }

        public override bool CanExecute(object parameter) => canExecute();
        public override void Execute(object parameter) => command();
    }
}
