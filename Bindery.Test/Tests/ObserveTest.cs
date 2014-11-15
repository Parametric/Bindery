using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class ObserveTest
    {
        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ObserveAndSendToCommand(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            string mouseMoveButton = null;
            viewModel.Command.ExecuteAction = parm => { mouseMoveButton = parm; };
            viewModel.Command.CanExecuteCondition = vm => commandEnabled;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Observe(c => c.MouseMoveButton).Executes(vm => vm.Command);

                // Act
                if (!binderActiveDuringEvent) binder.Dispose();
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

                var expected = expectUpdated ? "Right" : null;

                // Assert
                Assert.That(mouseMoveButton, Is.EqualTo(expected));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ObserveAndSetSourceValue(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Observe(c => c.MouseMoveButton).UpdateSource(vm => vm.StringValue);

                // Act
                if (!binderActiveDuringEvent) binder.Dispose();
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

                var expected = expectUpdated ? "Right" : null;

                // Assert
                Assert.That(viewModel.StringValue, Is.EqualTo(expected));
            }
        }

    }
}
