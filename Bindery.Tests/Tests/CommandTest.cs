using System;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class CommandTest
    {

        [Test]
        public void ObserveCanExecuteChanges()
        {
            // Arrange
            var cmd = new TestCommand();
            bool? value = null;
            var count = 0;

            var sub = cmd.ObserveCanExecuteChanges().Subscribe(x =>
            {
                value = x;
                count++;
            });

            // Act & Assert
            using (sub)
            {
                cmd.Enabled = true;
                Assert.That(value, Is.True);
                Assert.That(count, Is.EqualTo(1));

                cmd.Enabled = false;
                Assert.That(value, Is.False);
                Assert.That(count, Is.EqualTo(2));

                cmd.Enabled = false;
                Assert.That(count, Is.EqualTo(2), "Should only trigger on actual changes");
            }
        }

        private class TestCommand : EnablableCommandBase {
            
            public override void Execute(object parameter)
            {
            }
        }
    }
}
