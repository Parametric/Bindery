using System;
using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class BindingTest
    {
        [Test]
        public void TwoWayBinding()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Bind.Source(viewModel))
            {
                binder.ToControl(textBox).Property(c => c.Text).BindTo(vm => vm.StringValue);
                textBox.Text = "value #1";
                Assert.That(viewModel.StringValue, Is.EqualTo(textBox.Text));
                viewModel.StringValue = "value #2";
                Assert.That(textBox.Text, Is.EqualTo(viewModel.StringValue));
            }
        }

        [Test]
        public void OneWayBindingTowardsSource()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Bind.Source(viewModel))
            {
                binder.ToControl(textBox).Property(c => c.Text).UpdateSource(vm => vm.StringValue);
                textBox.Text = "value #1";
                Assert.That(viewModel.StringValue, Is.EqualTo(textBox.Text));
                viewModel.StringValue = "value #2";
                Assert.That(textBox.Text, Is.Not.EqualTo(viewModel.StringValue));
            }
        }

        [Test]
        public void OneWayBindingTowardsControl()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Bind.Source(viewModel))
            {
                binder.ToControl(textBox).Property(c => c.Text).UpdateControlFrom(vm => vm.StringValue);
                textBox.Text = "value #1";
                Assert.That(viewModel.StringValue, Is.Not.EqualTo(textBox.Text));
                viewModel.StringValue = "value #2";
                Assert.That(textBox.Text, Is.EqualTo(viewModel.StringValue));
            }
        }
        
        [Test]
        public void UpdateSourceWithConversion()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var binder = Bind.Source(viewModel))
            {
                binder.ToControl(textBox).Property(c => c.Text).UpdateSource(vm => vm.IntValue, Convert.ToInt32);
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
            using (var binder = Bind.Source(viewModel))
            {
                binder.ToControl(textBox).Property(c => c.Text).UpdateControlFrom(vm => vm.IntValue, Convert.ToString);
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
            using (var binder = Bind.Source(viewModel))
            {
                binder.ToControl(textBox).Property(c => c.Text).BindTo(vm => vm.IntValue, Convert.ToString, int.Parse);
                
                viewModel.IntValue = 3;
                Assert.That(textBox.Text, Is.EqualTo(Convert.ToString(viewModel.IntValue)));

                textBox.Text = "30";
                Assert.That(viewModel.IntValue, Is.EqualTo(Convert.ToInt32(textBox.Text)));
            }
        }
    }
}
