using System;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests.Basic
{
    [TestFixture]
    public class BindingTest
    {
        private TestBasicViewModel _viewModel;
        private TextBox _textBox;
        private ControlTester _controlTester;
        private ISourceBinder<TestBasicViewModel> _binder;

        [SetUp]
        public void BeforeEach()
        {
            // Arrange
            _viewModel = new TestBasicViewModel();
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
        public void OneWayBindingTowardsSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            _binder.Control(_textBox).Property(c => c.Text).Set(vm => vm.StringValue);
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            _textBox.Text = "value #1";
            var expected = expectUpdated ? _textBox.Text : string.Empty;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expected));
            _viewModel.StringValue = "value #2";
            Assert.That(_textBox.Text, Is.Not.EqualTo(_viewModel.StringValue));
        }

        [Test]
        public void UpdateSourceWithConversion()
        {
            _binder.Control(_textBox).Property(c => c.Text).Set(vm => vm.IntValue, Convert.ToInt32);
            _textBox.Text = "3";
            Assert.That(_viewModel.IntValue, Is.EqualTo(Convert.ToInt32(_textBox.Text)));
        }
    }
}
