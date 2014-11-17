using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class OnClickTest
    {
        private TestViewModel _viewModel;
        private Button _button;
        private ISourceBinder<TestViewModel> _binder;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _binder = Create.Binder(_viewModel);
            _button = new Button();
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _button.Dispose();
        }

        [Test]
        public void ClickingTheControlExecutesTheCommand()
        {
            // Arrange
            var executedCount = 0;
            _viewModel.Command.ExecuteAction = vm => executedCount++;
            _binder.Control(_button).OnClick(vm => vm.Command);

            // Act
            _button.PerformClick();
            _button.PerformClick();

            // Assert
            Assert.That(executedCount, Is.EqualTo(2));
        }

        [Test]
        public void EnableIsUpdatedBasedOnCanExecute()
        {
            // Arrange
            _binder.Control(_button).OnClick(vm => vm.Command);

            _viewModel.Command.CanExecuteCondition = vm => vm.IntValue >= 0;

            // Act & Assert
            _viewModel.IntValue = 5;
            Assert.That(_button.Enabled, Is.True);
            _viewModel.IntValue = -1;
            Assert.That(_button.Enabled, Is.False);
            _viewModel.IntValue = 5;
            Assert.That(_button.Enabled, Is.True);
        }
    }
}
