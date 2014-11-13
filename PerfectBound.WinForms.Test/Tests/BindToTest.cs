using System;
using System.Windows.Forms;
using NUnit.Framework;
using PerfectBound.WinForms.Test.TestClasses;

namespace PerfectBound.WinForms.Test.Tests
{
    [TestFixture]
    public class BindToTest
    {
        [Test]
        public void UpdatingControlUpdatesSource()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(textBox).Property(c => c.Text).BindTo(vm => vm.StringValue);
                textBox.Text = "new value";
                Assert.That(viewModel.StringValue, Is.EqualTo(textBox.Text));
            }
        }

        [Test]
        public void UpdatingSourceUpdatesControl()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var textBox = new TextBox();

            using (new ControlTester(textBox))
            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(textBox).Property(c => c.Text).BindTo(vm => vm.StringValue);
                viewModel.StringValue = "new value";
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
            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(textBox).Property(c => c.Text).ConvertTo(Convert.ToInt32).UpdateSource(vm => vm.IntValue);
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
            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(textBox).Property(c => c.Text).ConvertFrom<int>(Convert.ToString).UpdateBindable(vm => vm.IntValue);
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
            using (var source = Bindery.ObservableSource(viewModel))
            {
                source.Control(textBox).Property(c => c.Text).Convert(to:int.Parse, from:Convert.ToString).BindTo(vm => vm.IntValue);
                
                viewModel.IntValue = 3;
                Assert.That(textBox.Text, Is.EqualTo(Convert.ToString(viewModel.IntValue)));

                textBox.Text = "30";
                Assert.That(viewModel.IntValue, Is.EqualTo(Convert.ToInt32(textBox.Text)));
            }
        }
    }
}
