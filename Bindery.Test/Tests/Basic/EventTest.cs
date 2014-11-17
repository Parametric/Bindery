using System;
using System.Windows.Forms;
using Bindery.Interfaces.Basic;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests.Basic
{
    [TestFixture]
    public class EventTest
    {
        private TestBasicViewModel _viewModel;
        private TestButton _button;
        private IBasicSourceBinder<TestBasicViewModel> _binder;
        private TestBasicCommand _command;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestBasicViewModel();
            _command = new TestBasicCommand(_viewModel);
            _button = new TestButton();
            _binder = Create.BasicBinder(_viewModel);
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

            _binder.Control(_button).OnEvent("Click").Execute(_command);

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
            _binder.Control(_button).OnEvent<TestEventArgs>("Test").Execute(_command);

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
            _binder.Control(_button).OnEvent<MouseEventArgs>("MouseMove").Execute(_command);

            // Act
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));

            // Assert
            Assert.That(executedCount, Is.EqualTo(1));
        }

        [TestCase(true,true,true)]
        [TestCase(false,false,false)]
        [TestCase(true,false, false)]
        [TestCase(false,true, false)]
        public void ConvertEventArgsAndSendToCommand(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var x = 0;
            _command.ExecuteAction = parm => { x = parm; };
            _command.CanExecuteCondition = vm => commandEnabled;

            _binder.Control(_button).OnEvent<MouseEventArgs>("MouseMove").Execute(_command, args => args.X);

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
                _binder.Control(_button).OnEvent<MouseEventArgs>("MouseMove").Set(vm => vm.StringValue, args => Convert.ToString(args.Button));
                if (!binderActiveDuringEvent)
                    _binder.Dispose();
            // Act
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

            // Assert
            var expectedValue = expectUpdated ? "Right" : null;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void MemberDoesNotExist()
        {
            // Act
            var ex = Assert.Throws<ArgumentException>(
                () => _binder.Control(_button).OnEvent("BadName").Execute(_command));
            Assert.That(ex.Message, Is.EqualTo("'BadName' is not a member of 'Bindery.Test.TestClasses.TestButton'."));
        }

        [Test]
        public void EventNameIsNonEventMember()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => _binder.Control(_button).OnEvent("Text").Execute(_command));
            Assert.That(ex.Message, Is.EqualTo("'Bindery.Test.TestClasses.TestButton.Text' is not an event."));
        }

        [Test]
        public void WrongEventArgSpecified()
        {
            // Act
            var ex = Assert.Throws<ArgumentException>(() => _binder.Control(_button).OnEvent<MouseEventArgs>("Click").Execute(_command));
            Assert.That(ex.Message, Is.EqualTo("ParameterExpression of type 'System.Windows.Forms.MouseEventArgs' cannot be used for delegate parameter of type 'System.EventArgs'"));
        }
    }
}
