using System;
using System.Globalization;
using System.Windows.Forms;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class TargetTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _textBox = new TextBox();
            _target = new TestTarget();
            _binder = Create.Binder(_viewModel);
        }

        [TearDown]
        public void AfterEach()
        {
            _textBox.Dispose();
            _binder.Dispose();
        }

        private TestViewModel _viewModel;
        private TextBox _textBox;
        private ISourceBinder<TestViewModel> _binder;
        private TestTarget _target;

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TargetUpdatedWhenSourceChanges(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            const string originalValue = "value #1";
            const string updatedValue = "value #2";
            _viewModel.StringValue = originalValue;
            _binder.Target(_textBox).Property(c => c.Text).Get(vm => vm.StringValue);
            var expected = originalValue;
            Assert.That(_textBox.Text, Is.EqualTo(expected), "Should immediately update target property to source value");
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act
            _viewModel.StringValue = updatedValue;

            // Assert
            expected = expectUpdated ? updatedValue : originalValue;
            Assert.That(_textBox.Text, Is.EqualTo(expected));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void SourcePropertyValueNeedsToBeConvertedToTargetPropertyType(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            const int originalValue = 1;
            const int updatedValue = 2;
            _viewModel.IntValue = originalValue;
            Func<int, string> conversion = Convert.ToString;
            _binder.Target(_textBox).Property(c => c.Text).Get(vm => Convert.ToString(vm.IntValue));
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            var expected = originalValue;
            Assert.That(_textBox.Text, Is.EqualTo(conversion(expected)), "Should immediately update target property to source value");
            _viewModel.IntValue = updatedValue;
            expected = expectUpdated ? updatedValue : originalValue;
            Assert.That(_textBox.Text, Is.EqualTo(conversion(expected)));
        }

        private class SetCountTarget
        {
            private int _x;

            public int X
            {
                get => _x;
                set
                {
                    _x = value;
                    SetCount++;
                }
            }

            public int SetCount { get; set; }
        }

        [Test]
        public void GetReferencesObjectThatDoesNotImplementINotifyPropertyChanged()
        {
            // Arrange
            var testMethod = new Action(() => _binder.Target(_textBox).Property(t => t.Text).Get(vm => "3"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => testMethod());
            Assert.That(ex.Message, Is.EqualTo("At least one object defined in the expression must implement INotifyPropertyChanged."));
        }

        [Test]
        public void SetOnlyUpdatesOncePerReferencedObject()
        {
            var target = new SetCountTarget();
            _viewModel.IntValue = 3;
            _viewModel.StringValue = "3";
            _binder.Target(target).Property(t => t.X).Get(vm => vm.IntValue*vm.IntValue*Int32.Parse(vm.StringValue));
            Assert.That(target.SetCount, Is.EqualTo(1));
            _viewModel.IntValue = 4;
            Assert.That(target.SetCount, Is.EqualTo(2));
        }

        [Test]
        public void SetTargetSubProperty()
        {
            // Arrange
            _binder.Target(_target).Property(x => x.Info.A).Get(vm => vm.IntValue);

            // Act
            _viewModel.IntValue = 3;

            // Assert
            Assert.That(_target.Info.A, Is.EqualTo(3));
        }

        [Test]
        public void SetTargetSubPropertyWithAccessThroughMethodWithNoParameter()
        {
            // Arrange
            _binder.Target(_target).Property(x => x.GetInfo().A).Get(vm => vm.IntValue);

            // Act
            _viewModel.IntValue = 3;

            // Assert
            Assert.That(_target.Info.A, Is.EqualTo(3));
        }


        [Test]
        public void SetWithOperation()
        {
            // Arrange
            _viewModel.IntValue = 3;
            _viewModel.ComplexValue.DecValue = 7m;
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly - can't handle use of static argument
            _binder.Target(_textBox).Property(t => t.Text).Get(vm => Convert.ToString(vm.IntValue*vm.ComplexValue.DecValue));
            Assert.That(_textBox.Text, Is.EqualTo("21"));

            _viewModel.IntValue = 7;
            Assert.That(_textBox.Text, Is.EqualTo("49"));

            _viewModel.ComplexValue.DecValue = 0;
            Assert.That(_textBox.Text, Is.EqualTo("0"));
        }

        [Test]
        public void TargetIsAnInterfaceWithExplicitImplementation()
        {
            ITestIInterface target = new TestInterfaceImplementation();
            _viewModel.StringValue = "value #1";
            _binder.Target(target).Property(t => t.Text).Get(vm => vm.StringValue);
            Assert.That(target.Text, Is.EqualTo(_viewModel.StringValue));

            _viewModel.StringValue = "value #2";
            Assert.That(target.Text, Is.EqualTo(_viewModel.StringValue));
        }

        private interface ITestIInterface
        {
            string Text { get; set; }
        }

        private class TestInterfaceImplementation : ITestIInterface
        {
            string ITestIInterface.Text { get; set; }
        }
    }
}