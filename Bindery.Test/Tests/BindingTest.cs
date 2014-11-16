using System;
using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class BindingTest
    {
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TwoWayBinding(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Create.Binder(viewModel))
            {
                binder.Control(textBox).Property(c => c.Text).Bind(vm => vm.StringValue);
                if (!binderActiveDuringEvent)
                    binder.Dispose();

                textBox.Text = "value #1";
                var expected = expectUpdated ? textBox.Text : null;
                Assert.That(viewModel.StringValue, Is.EqualTo(expected));

                viewModel.StringValue = "value #2";
                expected = expectUpdated ? viewModel.StringValue : "value #1";
                Assert.That(textBox.Text, Is.EqualTo(expected));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsSource(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Create.Binder(viewModel))
            {
                binder.Control(textBox).Property(c => c.Text).Set(vm => vm.StringValue);
                if (!binderActiveDuringEvent)
                    binder.Dispose();

                textBox.Text = "value #1";
                var expected = expectUpdated ? textBox.Text : string.Empty;
                Assert.That(viewModel.StringValue, Is.EqualTo(expected));
                viewModel.StringValue = "value #2";
                Assert.That(textBox.Text, Is.Not.EqualTo(viewModel.StringValue));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void OneWayBindingTowardsControl(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Create.Binder(viewModel))
            {
                binder.Control(textBox).Property(c => c.Text).Get(vm => vm.StringValue);
                if (!binderActiveDuringEvent)
                    binder.Dispose();

                textBox.Text = "value #1";
                Assert.That(viewModel.StringValue, Is.Not.EqualTo(textBox.Text));
                viewModel.StringValue = "value #2";
                string expected = expectUpdated ? viewModel.StringValue : "value #1";
                Assert.That(textBox.Text, Is.EqualTo(expected));
            }
        }
        
        [Test]
        public void UpdateSourceWithConversion()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Create.Binder(viewModel))
            {
                binder.Control(textBox).Property(c => c.Text).Set(vm => vm.IntValue, Convert.ToInt32);
                textBox.Text = "3";
                Assert.That(viewModel.IntValue, Is.EqualTo(Convert.ToInt32(textBox.Text)));
            }
        }

        [Test]
        public void UpdateControlWithConversion()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Create.Binder(viewModel))
            {
                binder.Control(textBox).Property(c => c.Text).Get(vm => vm.IntValue, Convert.ToString);
                viewModel.IntValue = 3;
                Assert.That(textBox.Text, Is.EqualTo(Convert.ToString(viewModel.IntValue)));
            }
        }

        [Test]
        public void TwoWayBindingWithConversion()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Create.Binder(viewModel))
            {
                binder.Control(textBox).Property(c => c.Text).Bind(vm => vm.IntValue, Convert.ToString, int.Parse);
                
                viewModel.IntValue = 3;
                Assert.That(textBox.Text, Is.EqualTo(Convert.ToString(viewModel.IntValue)));

                textBox.Text = "30";
                Assert.That(viewModel.IntValue, Is.EqualTo(Convert.ToInt32(textBox.Text)));
            }
        }
    }
}
