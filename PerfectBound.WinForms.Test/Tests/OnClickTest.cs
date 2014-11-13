using System.Windows.Forms;
using NUnit.Framework;
using PerfectBound.WinForms.Binding;
using PerfectBound.WinForms.Test.TestClasses;

namespace PerfectBound.WinForms.Test.Tests
{
    [TestFixture]
    public class OnClickTest
    {
        [Test]
        public void ClickingTheControlExecutesTheCommand()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;
            var button = new Button();

            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(button).OnClick(vm => vm.Command);

                // Act
                button.PerformClick();
                button.PerformClick();

                // Assert
                Assert.That(executedCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void EnableIsUpdatedBasedOnCanExecute()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var button = new Button();

            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(button).OnClick(vm => vm.Command);

                viewModel.Command.CanExecuteCondition = vm => vm.IntValue >= 0;

                viewModel.IntValue = 5;
                Assert.That(button.Enabled, Is.True);
                viewModel.IntValue = -1;
                Assert.That(button.Enabled, Is.False);
                viewModel.IntValue = 5;
                Assert.That(button.Enabled, Is.True);
            }
        }
    }
}
