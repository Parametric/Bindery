using System;
using System.Windows.Forms;
using NUnit.Framework;
using PerfectBound.WinForms.Binding;
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

        public class ControlTester : IDisposable
        {
            private Form _form = new Form();

            public ControlTester(Control control)
            {
           }

            public void Dispose()
            {
                _form.Close();
            }
        }
    }
}
