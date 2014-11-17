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

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _textBox = new TextBox();
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
            _binder.Target(_textBox).Property(c => c.Text).Get(vm => vm.IntValue, conversion);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            var expected = originalValue;
            Assert.That(_textBox.Text, Is.EqualTo(conversion(expected)), "Should immediately update target property to source value");
            _viewModel.IntValue = updatedValue;
            expected = expectUpdated ? updatedValue : originalValue;
            Assert.That(_textBox.Text, Is.EqualTo(conversion(expected)));
        }
    }
}
