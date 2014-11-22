using System;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class TargetTest
    {
        private TestViewModel _viewModel;
        private TextBox _textBox;
        private ISourceBinder<TestViewModel> _binder;
        private TestTarget _target;

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
        public void SetTargetSubPropertyWithAccessThroughParameterlessMethod()
        {
            // Arrange
            _binder.Target(_target).Property(x => x.GetInfo().A).Get(vm => vm.IntValue);

            // Act
            _viewModel.IntValue = 3;

            // Assert
            Assert.That(_target.Info.A, Is.EqualTo(3));
        }


        [Test]
        public void GetReferencesObjectThatDoesNotImplementINotifyPropertyChanged()
        {
            // Arrange
            var testMethod = new Action(()=>_binder.Target(_textBox).Property(t => t.Text).Get(vm => "3"));
         
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(()=>testMethod());
            Assert.That(ex.Message, Is.EqualTo("At least one object defined in the expression must implement INotifyPropertyChanged."));
        }

        [Test]
        public void SetWithOperation()
        {
            // Arrange
            _viewModel.IntValue = 3;
            _viewModel.ComplexValue.DecValue = 7m;
            _binder.Target(_textBox).Property(t => t.Text).Get(vm => Convert.ToString(vm.IntValue*vm.ComplexValue.DecValue));
            Assert.That(_textBox.Text, Is.EqualTo("21"));

            _viewModel.IntValue = 7;
            Assert.That(_textBox.Text, Is.EqualTo("49"));

            _viewModel.ComplexValue.DecValue = 0;
            Assert.That(_textBox.Text, Is.EqualTo("0"));
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

        private class SetCountTarget
        {
            private int _x;

            public int X
            {
                get { return _x; }
                set 
                { 
                    _x = value;
                    SetCount++;
                }
            }

            public int SetCount { get; set; }
        }
    }
}
