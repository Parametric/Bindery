using System.Windows.Forms;
using NUnit.Framework;
using PerfectBound.WinForms.Binding;

namespace PerfectBound.WinForms.Test
{
    [TestFixture]
    public class CommandButtonTest
    {
        [Test]
        public void ClickingTheControlExecutesTheCommand()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var button = new Button();

            using (var bindingSource = Bindery.Source(viewModel))
            {
                bindingSource.Control(button).Command(vm => vm.Command);

                // Act
                button.PerformClick();
                button.PerformClick();

                // Assert
                Assert.That(viewModel.CommandExecutedCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void EnableIsUpdatedBasedOnCanExecute()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var button = new Button();

            using (var bindingSource = Bindery.Source(viewModel))
            {
                bindingSource.Control(button).Command(vm => vm.Command);

                viewModel.Command.CanExecuteCondition = vm => vm.Value >= 0;

                viewModel.Value = 5;
                Assert.That(button.Enabled, Is.True);
                viewModel.Value = -1;
                Assert.That(button.Enabled, Is.False);
                viewModel.Value = 5;
                Assert.That(button.Enabled, Is.True);
            }
        }
    }
}
