using NUnit.Framework;
using PerfectBound.WinForms.Test.TestClasses;

namespace PerfectBound.WinForms.Test.Tests
{
    [TestFixture]
    public class OnPropertyChangedTest
    {
        [Test]
        public void ActionIsCalledWhenPropertyIsChanged()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var callCount = 0;
            var setValue = 0;
            using (var binder = Bind.Source(viewModel))
            {
                binder.Property(vm => vm.IntValue).OnChanged(x =>
                {
                    setValue = x;
                    callCount++;
                });

                // Act
                viewModel.IntValue = 3;
            }
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(setValue, Is.EqualTo(3));
        }

        [Test]
        public void ActionIsNotCalledWhenAnotherPropertyIsChanged()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var callCount = 0;
            using (var binder = Bind.Source(viewModel))
            {
                binder.Property(vm => vm.IntValue).OnChanged(x => callCount++);

                // Act
                viewModel.StringValue = "3";
            }
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ActionIsNotCalledWhenPropertyIsNotChanged()
        {
            // Arrange
            var viewModel = new TestViewModel {IntValue=3};
            var callCount = 0;
            using (var binder = Bind.Source(viewModel))
            {
                binder.Property(vm => vm.IntValue).OnChanged(x => callCount++);
                // Act
                viewModel.IntValue = viewModel.IntValue;
            }
            // Assert
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ActionIsNotCalledAfterSourceIsDisposed()
        {
            var viewModel = new TestViewModel();
            var callCount = 0;
            using (var binder = Bind.Source(viewModel))
            {
                binder.Property(vm => vm.IntValue).OnChanged(x => callCount++);
            }
            // Act
            viewModel.IntValue = 3;
            // Assert
            Assert.That(callCount, Is.EqualTo(0));
        }
    }
}
