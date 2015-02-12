using System;

namespace Bindery.Tests.TestClasses
{
    public class TestCommand : CommandBase
    {
        public TestCommand(TestViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.PropertyChanged += (sender, e) => OnCanExecuteChanged();
            ExecuteAction = parm => { };
            CanExecuteCondition = vm => true;
        }

        public TestViewModel ViewModel { get; private set; }

        public Func<TestViewModel, bool> CanExecuteCondition { get; set; }
        public Action<dynamic> ExecuteAction { get; set; }

        public object ExecutionParameter { get; set; }

        public override bool CanExecute(object parameter)
        {
            ExecutionParameter = parameter;
            return CanExecuteCondition(ViewModel);
        }

        public override void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }
    }
}