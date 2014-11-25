using System;
using System.Windows.Input;

namespace Bindery.Test.TestClasses
{
    public class TestCommand : ICommand
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

        public bool CanExecute(object parameter)
        {
            return CanExecuteCondition(ViewModel);
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}