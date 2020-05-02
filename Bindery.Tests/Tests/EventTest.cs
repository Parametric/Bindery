using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    public class EventTest
    {
        private ISourceBinder<TestViewModel> _binder;
        private TestButton _button;
        private TestCommand _command;
        private TestViewModel _viewModel;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _command = new TestCommand(_viewModel);
            _binder = Binder.Source(_viewModel);
            _button = new TestButton();
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _button.Dispose();
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void SimpleEvent(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var executedCount = 0;
            _command.ExecuteAction = vm => executedCount++;
            _command.CanExecuteCondition = vm => commandEnabled;

            _binder.Control(_button).OnEvent(nameof(_button.Click)).Execute(_command);

            // Act
            if (!binderActiveDuringEvent)
                _binder.Dispose();
            _button.PerformClick();

            // Assert
            var expected = expectUpdated ? 1 : 0;
            Assert.That(executedCount, Is.EqualTo(expected));
        }

        [Test]
        public void EventWithGenericEventHandler()
        {
            // Arrange
            var executedCount = 0;
            _command.ExecuteAction = vm => executedCount++;

            _binder.Control(_button).OnEvent<TestEventArgs>().Execute(_command);

            // Act
            _button.PerformTest(new TestEventArgs());

            // Assert
            Assert.That(executedCount, Is.EqualTo(1));
        }

        [Test]
        public void ExplicitEventSpecification()
        {
            // Arrange
            var executedCount = 0;
            _command.ExecuteAction = vm => executedCount++;
            _binder.Control(_button).OnEvent<MouseEventArgs>(nameof(_button.MouseMove)).Execute(_command);

            // Act
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));

            // Assert
            Assert.That(executedCount, Is.EqualTo(1));
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ExecuteCommandWithConstantParameter(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var x = 0;
            const int newX = 7;
            _command.ExecuteAction = param => { x = param; };
            _command.CanExecuteCondition = vm => commandEnabled;
            _binder.Control(_button).OnEvent<MouseEventArgs>(nameof(_button.MouseMove)).Execute(_command, newX);

            // Act
            if (!binderActiveDuringEvent) _binder.Dispose();
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));

            var expectedX = expectUpdated ? newX : 0;

            // Assert
            Assert.That(x, Is.EqualTo(expectedX));
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ExecuteCommandWithCalculatedParameter(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var x = 0;
            const int newX = 7;
            _command.ExecuteAction = param => { x = param; };
            _command.CanExecuteCondition = vm => commandEnabled;
            _binder.Control(_button).OnEvent<MouseEventArgs>(nameof(_button.MouseMove)).Execute(_command, () => newX);

            // Act
            if (!binderActiveDuringEvent) _binder.Dispose();
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));

            var expectedX = expectUpdated ? newX : 0;

            // Assert
            Assert.That(x, Is.EqualTo(expectedX));
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ConvertEventArgsAndSendToCommand(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var x = 0;
            _command.ExecuteAction = param => { x = param; };
            _command.CanExecuteCondition = vm => commandEnabled;
            _binder.Control(_button)
                .OnEvent<MouseEventArgs>(nameof(_button.MouseMove))
                .Transform(o => o.Select(e => e.Args.X))
                .Execute(_command);

            // Act
            if (!binderActiveDuringEvent) _binder.Dispose();
            const int newX = 7;
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, newX, 0, 0));

            var expectedX = expectUpdated ? newX : 0;

            // Assert
            Assert.That(x, Is.EqualTo(expectedX));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ConvertEventArgsAndUpdateSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_button).OnEvent<MouseEventArgs>(nameof(_button.MouseMove))
                .Transform(o => o.Select(x => Convert.ToString(x.Args.Button)))
                .Set(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

            var expectedValue = expectUpdated ? "Right" : null;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void AccessFullEventParameters()
        {
            // Arrange
            Control sentBy = null;
            var mouseButtons = MouseButtons.None;
            _binder.Control(_button).OnEvent(nameof(_button.Click)).Subscribe(e => sentBy = (Control) e.Sender);
            _binder.Control(_button).OnEvent<MouseEventArgs>(nameof(_button.MouseMove))
                .Subscribe(x => mouseButtons = x.Args.Button);

                // Act
            _button.PerformClick();
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

            // Assert
            Assert.That(sentBy, Is.SameAs(_button));
            Assert.That(mouseButtons, Is.EqualTo(MouseButtons.Right));
        }


        [Test]
        public void MemberDoesNotExist()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => _binder.Control(_button).OnEvent("BadName").Execute(_command));
            Assert.That(ex.Message, Is.EqualTo("'BadName' is not a member of 'Bindery.Tests.TestClasses.TestButton'."));
        }

        [Test]
        public void EventNameIsNonEventMember()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => _binder.Control(_button).OnEvent(nameof(_button.Text)).Execute(_command));
            Assert.That(ex.Message, Is.EqualTo("'Bindery.Tests.TestClasses.TestButton.Text' is not an event."));
        }

        [Test]
        public void WrongEventArgSpecified()
        {
            var ex = Assert.Throws<ArgumentException>(() => _binder.Control(_button).OnEvent<MouseEventArgs>(nameof(_button.Click)).Execute(_command));
            Assert.That(ex.Message,
                Is.EqualTo(
                    "ParameterExpression of type 'System.Windows.Forms.MouseEventArgs' cannot be used for delegate parameter of type 'System.EventArgs'"));
        }
    }
}