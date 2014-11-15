using System;
using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    public class EventTest
    {
        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void SimpleEvent(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            var executedCount = 0;
            viewModel.Command.ExecuteAction = vm => executedCount++;
            viewModel.Command.CanExecuteCondition = vm => commandEnabled;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event("Click").Executes(vm => vm.Command);

                // Act
                if (!binderActiveDuringEvent)
                    binder.Dispose();
                button.PerformClick();

                // Assert
                var expected = expectUpdated ? 1 : 0;
                Assert.That(executedCount, Is.EqualTo(expected));
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


        [TestCase(true,true,true)]
        [TestCase(false,false,false)]
        [TestCase(true,false, false)]
        [TestCase(false,true, false)]
        public void ConvertEventArgsAndSendToCommand(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            var x = 0;
            viewModel.Command.ExecuteAction = parm => { x = parm; };
            viewModel.Command.CanExecuteCondition = vm => commandEnabled;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event<MouseEventArgs>("MouseMove").ConvertArgsTo<int>(args=>args.X).Executes(vm => vm.Command);

                // Act
                if (!binderActiveDuringEvent) binder.Dispose();
                const int newX = 7;
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, newX, 0, 0));

                var expectedX = expectUpdated ? newX : 0;

                // Assert
                Assert.That(x, Is.EqualTo(expectedX));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ConvertEventArgsAndUpdateSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Event<MouseEventArgs>("MouseMove").ConvertArgsTo<string>(args => Convert.ToString(args.Button)).UpdateSource(vm => vm.StringValue);
                if (!binderActiveDuringEvent)
                    binder.Dispose();
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

                var expectedValue = expectUpdated ? "Right" : null;
                Assert.That(viewModel.StringValue, Is.EqualTo(expectedValue));
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
                Assert.That(ex.Message, Is.EqualTo("'BadName' is not a member of 'Bindery.Test.TestClasses.TestButton'."));
            }
        }

        [Test]
        public void EventNameIsNonEventMember()
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                // Act
                var ex = Assert.Throws<ArgumentException>(
                    () => binder.Control(button).Event("Text").Executes(vm => vm.Command));
                Assert.That(ex.Message, Is.EqualTo("'Bindery.Test.TestClasses.TestButton.Text' is not an event."));
            }
        }

        [Test]
        public void WrongEventArgSpecified()
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                // Act
                var ex = Assert.Throws<ArgumentException>(() => binder.Control(button).Event<MouseEventArgs>("Click").Executes(vm => vm.Command));
                Assert.That(ex.Message, Is.EqualTo("ParameterExpression of type 'System.Windows.Forms.MouseEventArgs' cannot be used for delegate parameter of type 'System.EventArgs'"));
            }
        }
    }
}
