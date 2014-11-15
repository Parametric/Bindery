using System;
using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class TargetTest
    {
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void TargetUpdatedWhenSourceChanges(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var textBox = new TextBox())
            using (var binder = Bind.Source(viewModel))
            {
                const string originalValue = "value #1";
                const string updatedValue = "value #2";
                viewModel.StringValue = originalValue;
                binder.ToTarget(textBox).Property(c => c.Text).UpdateTargetFrom(vm => vm.StringValue);
                if (!binderActiveDuringEvent)
                    binder.Dispose();

                var expected = originalValue;
                Assert.That(textBox.Text, Is.EqualTo(expected), "Should immediately update target property to source value");
                viewModel.StringValue = updatedValue;
                expected = expectUpdated ? updatedValue : originalValue;
                Assert.That(textBox.Text, Is.EqualTo(expected));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void SourcePropertyValueNeedsToBeConvertedToTargetPropertyType(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var textBox = new TextBox())
            using (var binder = Bind.Source(viewModel))
            {
                const int originalValue = 1;
                const int updatedValue = 2;
                viewModel.IntValue = originalValue;
                Func<int, string> conversion = Convert.ToString;
                binder.ToTarget(textBox).Property(c => c.Text).UpdateTargetFrom(vm => vm.IntValue, conversion);
                if (!binderActiveDuringEvent)
                    binder.Dispose();

                var expected = originalValue;
                Assert.That(textBox.Text, Is.EqualTo(conversion(expected)), "Should immediately update target property to source value");
                viewModel.IntValue = updatedValue;
                expected = expectUpdated ? updatedValue : originalValue;
                Assert.That(textBox.Text, Is.EqualTo(conversion(expected)));
            }
        }
    }
}
