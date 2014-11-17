using System;
using System.Windows.Input;

namespace Bindery.Test.TestClasses
{
    public class TestBasicCommand : ICommand
    {
        private readonly TestBasicViewModel _viewModel;

        public TestBasicCommand(TestBasicViewModel viewModel)
        {
            _viewModel = viewModel;
            ExecuteAction = parm => { };
            CanExecuteCondition = vm => true;
        }

        public Func<TestBasicViewModel, bool> CanExecuteCondition { get; set; }
        public Action<dynamic> ExecuteAction { get; set; }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCondition(_viewModel);
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}