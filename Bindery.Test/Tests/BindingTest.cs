using System;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class BindingTest
    {
        private TestViewModel _viewModel;
        private TextBox _textBox;
        private ISourceBinder<TestViewModel> _binder;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _binder = Create.Binder(_viewModel);
            _textBox = new TextBox();
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _textBox.Dispose();
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBinding(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Act
            _binder.Control(_textBox).Property(c => c.Text).Bind(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Assert
            if (!expectUpdated)
            {
                Assert.That(_textBox.DataBindings.Count, Is.EqualTo(0));
            }
            else
            {
                var binding = _textBox.DataBindings[0];
                Assert.That(binding.PropertyName, Is.EqualTo("Text"));
                Assert.That(binding.DataSource, Is.EqualTo(_viewModel));
                Assert.That(binding.BindingMemberInfo.BindingMember, Is.EqualTo("StringValue"));
                Assert.That(binding.BindingMemberInfo.BindingField, Is.EqualTo("StringValue"));
                Assert.That(binding.ControlUpdateMode, Is.EqualTo(ControlUpdateMode.OnPropertyChanged));
                Assert.That(binding.DataSourceUpdateMode, Is.EqualTo(DataSourceUpdateMode.OnPropertyChanged));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Act
            _binder.Control(_textBox).Property(c => c.Text).Set(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Assert
            if (!expectUpdated)
            {
                Assert.That(_textBox.DataBindings.Count, Is.EqualTo(0));
            }
            else
            {
                var binding = _textBox.DataBindings[0];
                Assert.That(binding.PropertyName, Is.EqualTo("Text"));
                Assert.That(binding.DataSource, Is.EqualTo(_viewModel));
                Assert.That(binding.BindingMemberInfo.BindingMember, Is.EqualTo("StringValue"));
                Assert.That(binding.BindingMemberInfo.BindingField, Is.EqualTo("StringValue"));
                Assert.That(binding.ControlUpdateMode, Is.EqualTo(ControlUpdateMode.Never));
                Assert.That(binding.DataSourceUpdateMode, Is.EqualTo(DataSourceUpdateMode.OnPropertyChanged));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsControl(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Act
            _binder.Control(_textBox).Property(c => c.Text).Get(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Assert
            if (!expectUpdated)
            {
                Assert.That(_textBox.DataBindings.Count, Is.EqualTo(0));
            }
            else
            {
                var binding = _textBox.DataBindings[0];
                Assert.That(binding.PropertyName, Is.EqualTo("Text"));
                Assert.That(binding.DataSource, Is.EqualTo(_viewModel));
                Assert.That(binding.BindingMemberInfo.BindingMember, Is.EqualTo("StringValue"));
                Assert.That(binding.BindingMemberInfo.BindingField, Is.EqualTo("StringValue"));
                Assert.That(binding.ControlUpdateMode, Is.EqualTo(ControlUpdateMode.OnPropertyChanged));
                Assert.That(binding.DataSourceUpdateMode, Is.EqualTo(DataSourceUpdateMode.Never));
            }
        }
        
        
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBindingWithMultiPartSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Act
            _binder.Control(_textBox).Property(c => c.Text).Bind(vm => vm.ComplexValue.DecValue, Convert.ToString, Convert.ToDecimal);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Assert
            if (!expectUpdated)
            {
                Assert.That(_textBox.DataBindings.Count, Is.EqualTo(0));
            }
            else
            {
                var binding = _textBox.DataBindings[0];
                Assert.That(binding.PropertyName, Is.EqualTo("Text"));
                Assert.That(binding.DataSource, Is.EqualTo(_viewModel));
                Assert.That(binding.BindingMemberInfo.BindingMember, Is.EqualTo("ComplexValue.DecValue"));
                Assert.That(binding.BindingMemberInfo.BindingPath, Is.EqualTo("ComplexValue"));
                Assert.That(binding.BindingMemberInfo.BindingField, Is.EqualTo("DecValue"));
                Assert.That(binding.ControlUpdateMode, Is.EqualTo(ControlUpdateMode.OnPropertyChanged));
                Assert.That(binding.DataSourceUpdateMode, Is.EqualTo(DataSourceUpdateMode.OnPropertyChanged));
            }
        }

    }
}
