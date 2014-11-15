using System;
using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
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
                binder.Control(button).Event("Click").Executes(vm => vm.Command);

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
                binder.Control(button).Event("Click").Executes(vm => vm.Command);

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
                binder.Control(button).Event<TestEventArgs>("Test").Executes(vm => vm.Command);

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
                binder.Control(button).Event<MouseEventArgs>("MouseMove").Executes(vm => vm.Command);

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
            binder.Control(button).Event("Click").Executes(vm => vm.Command);

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

        [Test]
        public void MemberDoesNotExist()
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                // Act
                var ex = Assert.Throws<ArgumentException>(
                    () => binder.Control(button).Event("BadName").Executes(vm => vm.Command));
                Console.WriteLine(ex.Message);
            }
        }

        [Test]
        public void EventNameIsNotAnEvent()
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                // Act
                var ex = Assert.Throws<ArgumentException>(
                    () => binder.Control(button).Event("Text").Executes(vm => vm.Command));
                Console.WriteLine(ex.Message);
            }
        }

    }
}
