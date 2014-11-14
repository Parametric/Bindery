using System.Windows.Forms;
using NUnit.Framework;
using PerfectBound.WinForms.Test.TestClasses;

namespace PerfectBound.WinForms.Test.Tests
{
    public class EventTest
    {
        [Test]
        public void SimpleEvent()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event(c => h => c.Click += h).Triggers(vm => vm.Command);

                // Act
                button.PerformClick();

                // Assert
                Assert.That(executedCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void EventDoesNotFireCommandIfExecutionConditionIsFalse()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;
            viewModel.Command.CanExecuteCondition = vm => false;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event(c => h => c.Click += h).Triggers(vm => vm.Command);

                // Act
                button.PerformClick();

                // Assert
                Assert.That(executedCount, Is.EqualTo(0));
            }
        }

        [Test]
        public void EventWithGenericEventHandler()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event<TestEventArgs>(c => h => c.Test += h).Triggers(vm => vm.Command);

                // Act
                button.PerformTest(new TestEventArgs());

                // Assert
                Assert.That(executedCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void ExplicitEventSpecification()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event<MouseEventArgs, MouseEventHandler>(c => h => c.MouseMove += h).Triggers(vm => vm.Command);

                // Act
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));

                // Assert
                Assert.That(executedCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void DisposingOfTheBinderUnsubscribesFromEvent()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;

            var button = new TestButton();
            var binder = Bind.Source(viewModel);
            binder.Control(button).Event(c => h => c.Click += h).Triggers(vm => vm.Command);

            try
            {
                // Act
                binder.Dispose();
                button.PerformClick();

                // Assert
                Assert.That(executedCount, Is.EqualTo(0));
            }
            finally
            {
                button.Dispose();
            }
        }

    }
}
