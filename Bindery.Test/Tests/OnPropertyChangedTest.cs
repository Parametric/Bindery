using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class OnPropertyChangedTest
    {
        private TestViewModel _viewModel;
        private ISourceBinder<TestViewModel> _binder;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _binder = Create.Binder(_viewModel);
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
        }

        [Test]
        public void ActionIsCalledWhenPropertyIsChanged()
        {
            // Arrange
            var callCount = 0;
            var setValue = 0;
            _binder.Property(vm => vm.IntValue).OnChanged(x =>
            {
                setValue = x;
                callCount++;
            });

            // Act
            _viewModel.IntValue = 3;

            // Assert
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(setValue, Is.EqualTo(3));
        }

        [Test]
        public void ActionIsNotCalledWhenAnotherPropertyIsChanged()
        {
            // Arrange
            var callCount = 0;
            _binder.Property(vm => vm.IntValue).OnChanged(x => callCount++);

            // Act
            _viewModel.StringValue = "3";
        }

        [Test]
        public void ActionIsNotCalledWhenPropertyIsNotChanged()
        {
            // Arrange
            _viewModel.IntValue = 3;
            var callCount = 0;
            _binder.Property(vm => vm.IntValue).OnChanged(x => callCount++);

            // Act
            _viewModel.IntValue = _viewModel.IntValue;

            // Assert
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ActionIsNotCalledAfterSourceIsDisposed()
        {
            // Arrange
            var callCount = 0;
            _binder.Property(vm => vm.IntValue).OnChanged(x => callCount++);
            _binder.Dispose();

            // Act
            _viewModel.IntValue = 3;

            // Assert
            Assert.That(callCount, Is.EqualTo(0));
        }
    }
}
