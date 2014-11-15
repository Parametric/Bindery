using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bindery.Extensions
{
    public static class CommandExtensions
    {
        public static void ExecuteIfValid<TConverted>(this ICommand command, TConverted parameter)
        {
            if (command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}
