using System;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    [Ignore("Requires controls to be added to a form")]
    public class BindingWithFormCreationTest
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
            _binder.Control(_textBox).Property(c => c.Text).Bind(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _textBox.Text = "value #1";
            var expected = expectUpdated ? _textBox.Text : null;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expected));

            _viewModel.StringValue = "value #2";
            expected = expectUpdated ? _viewModel.StringValue : "value #1";
            Assert.That(_textBox.Text, Is.EqualTo(expected));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_textBox).Property(c => c.Text).Set(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _textBox.Text = "value #1";
            var expected = expectUpdated ? _textBox.Text : string.Empty;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expected));
            _viewModel.StringValue = "value #2";
            Assert.That(_textBox.Text, Is.Not.EqualTo(_viewModel.StringValue));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsControl(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_textBox).Property(c => c.Text).Get(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _textBox.Text = "value #1";
            Assert.That(_viewModel.StringValue, Is.Not.EqualTo(_textBox.Text));
            _viewModel.StringValue = "value #2";
            var expected = expectUpdated ? _viewModel.StringValue : "value #1";
            Assert.That(_textBox.Text, Is.EqualTo(expected));
        }
        
        [Test]
        public void UpdateSourceWithConversion()
        {
            _binder.Control(_textBox).Property(c => c.Text).Set(vm => vm.IntValue, Convert.ToInt32);
            _textBox.Text = "3";
            Assert.That(_viewModel.IntValue, Is.EqualTo(Convert.ToInt32(_textBox.Text)));
        }

        [Test]
        public void UpdateControlWithConversion()
        {
            _binder.Control(_textBox).Property(c => c.Text).Get(vm => vm.IntValue, Convert.ToString);
            _viewModel.IntValue = 3;
            Assert.That(_textBox.Text, Is.EqualTo(Convert.ToString(_viewModel.IntValue)));
        }

        [Test]
        public void TwoWayBindingWithConversion()
        {
            _binder.Control(_textBox).Property(c => c.Text).Bind(vm => vm.IntValue, Convert.ToString, int.Parse);
                
            _viewModel.IntValue = 3;
            Assert.That(_textBox.Text, Is.EqualTo(Convert.ToString(_viewModel.IntValue)));

            _textBox.Text = "30";
            Assert.That(_viewModel.IntValue, Is.EqualTo(Convert.ToInt32(_textBox.Text)));
        }
        
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBindingWithMultiPartSource(bool binderActiveDuringEvent, bool expectUpdated)
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

    }
}