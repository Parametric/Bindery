using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Bindery.Extensions
{
    internal static class CommandExtensions
    {
        public static void ExecuteIfValid(this ICommand command, object parameter)
        {
            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }

        public static IObservable<EventArgs> CreateCanExecuteChangedObservable(this ICommand command)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                argsAction => (sender, e) => argsAction(e),
                handler => command.CanExecuteChanged += handler,
                handler => command.CanExecuteChanged -= handler);
        }
    }
}