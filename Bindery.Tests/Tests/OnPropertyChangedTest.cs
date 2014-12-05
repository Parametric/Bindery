using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class OnPropertyChangedTest
    {
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

        private TestViewModel _viewModel;
        private ISourceBinder<TestViewModel> _binder;

        [Test]
        public void ActionIsCalledWhenPropertyIsChanged()
        {
            // Arrange
            var callCount = 0;
            var setValue = 0;
            _binder.OnPropertyChanged(vm => vm.IntValue).Subscribe(x =>
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
        public void ActionIsNotCalledAfterSourceIsDisposed()
        {
            // Arrange
            var callCount = 0;
            _binder.OnPropertyChanged(vm => vm.IntValue).Subscribe(x => callCount++);
            _binder.Dispose();

            // Act
            _viewModel.IntValue = 3;

            // Assert
            Assert.That(callCount, Is.EqualTo(0));
        }

        [Test]
        public void ActionIsNotCalledWhenAnotherPropertyIsChanged()
        {
            // Arrange
            var callCount = 0;
            _binder.OnPropertyChanged(vm => vm.IntValue).Subscribe(x => callCount++);

            // Act
            _viewModel.StringValue = "3";
        }

        [Test]
        public void ActionIsNotCalledWhenPropertyIsNotChanged()
        {
            // Arrange
            _viewModel.IntValue = 3;
            var callCount = 0;
            _binder.OnPropertyChanged(vm => vm.IntValue).Subscribe(x => callCount++);

            // Act
            _viewModel.IntValue = _viewModel.IntValue;

            // Assert
            Assert.That(callCount, Is.EqualTo(0));
        }
    }
}