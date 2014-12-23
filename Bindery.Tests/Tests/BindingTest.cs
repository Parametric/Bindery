using System;
using System.Windows.Forms;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class BindingTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _binder = Create.Binder(_viewModel);
            _userControl = new PropertyGrid();
            _userControl.CreateControl();
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _userControl.Dispose();
        }

        private TestViewModel _viewModel;
        private PropertyGrid _userControl;
        private ISourceBinder<TestViewModel> _binder;

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBinding(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_userControl).Property(c => c.Text).Bind(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _userControl.Text = "value #1";
            var expected = expectUpdated ? _userControl.Text : null;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expected));

            _viewModel.StringValue = "value #2";
            expected = expectUpdated ? _viewModel.StringValue : "value #1";
            Assert.That(_userControl.Text, Is.EqualTo(expected));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_userControl).Property(c => c.Text).Set(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            var currentVmValue = _viewModel.StringValue;
            _userControl.Text = "value #1";
            var expected = expectUpdated ? _userControl.Text : currentVmValue;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expected));
            _viewModel.StringValue = "value #2";
            Assert.That(_userControl.Text, Is.Not.EqualTo(_viewModel.StringValue));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsControl(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_userControl).Property(c => c.Text).Get(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _userControl.Text = "value #1";
            Assert.That(_viewModel.StringValue, Is.Not.EqualTo(_userControl.Text));
            _viewModel.StringValue = "value #2";
            var expected = expectUpdated ? _viewModel.StringValue : "value #1";
            Assert.That(_userControl.Text, Is.EqualTo(expected));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBindingWithMultiPartSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_userControl).Property(c => c.Text).Bind(vm => vm.ComplexValue.DecValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act & Assert
            _userControl.Text = "10.5";
            var expected = expectUpdated ? _userControl.Text : null;
            Assert.That(_viewModel.ComplexValue.DecValue, Is.EqualTo(Convert.ToDecimal(expected)));

            _viewModel.ComplexValue.DecValue = -33.3m;
            expected = expectUpdated ? "-33.3" : "10.5";
            Assert.That(_userControl.Text, Is.EqualTo(expected));
        }

        [Test]
        public void TwoWayBindingWithConversion()
        {
            _binder.Control(_userControl).Property(c => c.Text).Bind(vm => vm.IntValue);

            _viewModel.IntValue = 3;
            Assert.That(_userControl.Text, Is.EqualTo(Convert.ToString(_viewModel.IntValue)));

            _userControl.Text = "30";
            Assert.That(_viewModel.IntValue, Is.EqualTo(Convert.ToInt32(_userControl.Text)));
        }

        [Test]
        public void UpdateControlWithConversion()
        {
            _binder.Control(_userControl).Property(c => c.Text).Get(vm => vm.IntValue);
            _viewModel.IntValue = 3;
            Assert.That(_userControl.Text, Is.EqualTo(Convert.ToString(_viewModel.IntValue)));
        }

        [Test]
        public void UpdateSourceWithConversion()
        {
            _binder.Control(_userControl).Property(c => c.Text).Set(vm => vm.IntValue);
            _userControl.Text = "3";
            Assert.That(_viewModel.IntValue, Is.EqualTo(Convert.ToInt32(_userControl.Text)));
        }

        [Test]
        public void NotAMemberAccess()
        {
            var ex = Assert.Throws<ArgumentException>(
                () => _binder.Control(_userControl).Property(c => c.Text).Set(vm => vm.MyMethod()));
            Assert.That(ex.Message, Is.StringStarting("Expression 'vm.MyMethod()' is not a member access"));
        }

        [Test]
        public void BindControlObjectPropertyToDecimalViewModelProperty()
        {
            // Arrange
            using (var control = new PropertyGrid())
            {
                control.CreateControl();
                _binder.Control(control).Property(c => c.SelectedObject).Bind(vm => vm.IntValue);

                _viewModel.IntValue = 5;
                Assert.That(control.SelectedObject, Is.EqualTo(_viewModel.IntValue));
            }
        }
    }
}