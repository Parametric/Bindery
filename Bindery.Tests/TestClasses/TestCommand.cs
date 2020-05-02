using System;
using System.Windows.Input;

namespace Bindery.Tests.TestClasses
{
    public class TestCommand : ICommand
    {
        public TestCommand(TestViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.PropertyChanged += (sender, e) => CanExecuteChanged?.Invoke(sender, e);
            ExecuteAction = _ => { };
            CanExecuteCondition = vm => true;
        }

        public TestViewModel ViewModel { get; }

        public Func<TestViewModel, bool> CanExecuteCondition { get; set; }
        public Action<dynamic> ExecuteAction { get; set; }

        public object ExecutionParameter { get; set; }

        public bool CanExecute(object parameter)
        {
            ExecutionParameter = parameter;
            return CanExecuteCondition(ViewModel);
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}