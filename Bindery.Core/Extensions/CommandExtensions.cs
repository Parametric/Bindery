using System.Windows.Input;

namespace Bindery.Extensions
{
    public static class CommandExtensions
    {
        public static void ExecuteIfValid(this ICommand command, object parameter)
        {
            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}