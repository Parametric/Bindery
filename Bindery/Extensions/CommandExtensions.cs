using System.Windows.Input;

namespace Bindery.Extensions
{
    public static class CommandExtensions
    {
        public static void ExecuteIfValid<T>(this ICommand command, T parameter)
        {
            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}
