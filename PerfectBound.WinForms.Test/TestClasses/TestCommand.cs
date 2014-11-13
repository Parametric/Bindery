using System;
using System.Windows.Input;

namespace PerfectBound.WinForms.Test.TestClasses
{
    public class TestCommand : ICommand
    {
        private readonly TestViewModel _viewModel;

        public TestCommand(TestViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += (sender, e) => OnCanExecuteChanged();
            ExecuteAction = vm => { };
            CanExecuteCondition = vm => true;
        }

        public Func<TestViewModel, bool> CanExecuteCondition { get; set; }
        public Action<TestViewModel> ExecuteAction { get; set; }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCondition(_viewModel);
        }

        public void Execute(object parameter)
        {
            ExecuteAction(_viewModel);
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}