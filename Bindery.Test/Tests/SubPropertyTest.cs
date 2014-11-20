using System;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class SubPropertyTest
    {
        private TestViewModel _viewModel;
        private TextBox _textBox;
        private ControlTester _controlTester;
        private ISourceBinder<TestViewModel> _binder;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _binder = Create.Binder(_viewModel);
            _textBox = new TextBox();
            _controlTester = new ControlTester(_textBox);
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _textBox.Dispose();
            _controlTester.Dispose();
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBinding(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_textBox).Property(c => c.Text).Bind(vm => vm.ComplexValue.DecValue, Convert.ToString, Convert.ToDecimal);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _textBox.Text = "10.5";
            var expected = expectUpdated ? _textBox.Text : null;
            Assert.That(_viewModel.ComplexValue.DecValue, Is.EqualTo(Convert.ToDecimal(expected)));

            _viewModel.ComplexValue.DecValue = -33.3m;
            expected = expectUpdated ? "-33.3" : "10.5";
            Assert.That(_textBox.Text, Is.EqualTo(expected));
        }

        [Test]
        public void ActionIsCalledWhenPropertyIsChanged()
        {
            // Arrange
            var callCount = 0;
            var setValue = 0m;
            _binder.Property(vm => vm.ComplexValue.DecValue).OnChanged(x =>
            {
                setValue = x;
                callCount++;
            });

            // Act
            _viewModel.ComplexValue.DecValue = 3;

            // Assert
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(setValue, Is.EqualTo(3));
        }

    }
}
