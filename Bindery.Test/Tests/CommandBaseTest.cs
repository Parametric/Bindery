using System;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class CommandBaseTest
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
                cmd.SetCanExecute(true);
                Assert.That(value, Is.True);
                Assert.That(count, Is.EqualTo(1));

                cmd.SetCanExecute(false);
                Assert.That(value, Is.False);
                Assert.That(count, Is.EqualTo(2));

                cmd.SetCanExecute(false);
                Assert.That(count, Is.EqualTo(2), "Should only trigger on actual changes");
            }
        }

        private class TestCommand : CommandBase {
            
            private bool _canExecute;


            public void SetCanExecute(bool value)
            {
                _canExecute = value;
                OnCanExecuteChanged();
            }

            public override bool CanExecute(object parameter)
            {
                return _canExecute;
            }

            public override void Execute(object parameter)
            {
            }
        }
    }
}
