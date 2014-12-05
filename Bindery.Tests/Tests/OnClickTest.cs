using System.Windows.Forms;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class OnClickTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _command = new TestCommand(_viewModel);
            _binder = Create.Binder(_viewModel);
            _button = new Button();
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _button.Dispose();
        }

        private TestViewModel _viewModel;
        private Button _button;
        private ISourceBinder<TestViewModel> _binder;
        private TestCommand _command;

        [Test]
        public void ClickingTheControlExecutesTheCommand()
        {
            // Arrange
            var executedCount = 0;
            _command.ExecuteAction = vm => executedCount++;
            _binder.Control(_button).OnClick(_command);

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
            _command.CanExecuteCondition = vm => vm.IntValue > 0;

            // Act & Assert
            _binder.Control(_button).OnClick(_command);
            Assert.That(_button.Enabled, Is.False, "Enabled should be set based off initial condition");
            _viewModel.IntValue = 5;
            Assert.That(_button.Enabled, Is.True);
            _viewModel.IntValue = -1;
            Assert.That(_button.Enabled, Is.False);
            _viewModel.IntValue = 5;
            Assert.That(_button.Enabled, Is.True);
        }
    }
}