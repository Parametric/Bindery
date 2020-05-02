using System.Threading;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class OnClickTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _binder = Binder.Source(_viewModel);
            _control = new TestControl();
            _invoker = new TestInvoker();
            Invoker.Override(_invoker);
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _control.Dispose();
        }

        private TestViewModel _viewModel;
        private TestControl _control;
        private ISourceBinder<TestViewModel> _binder;
        private TestInvoker _invoker;

        [Test]
        public void OnClickObservable()
        {
            // Arrange
            var actionCalled = false;
            _binder.Control(_control).OnClick().Subscribe(_ => actionCalled = true);

            // Act
            _control.PerformClick();

            // Assert
            Assert.That(actionCalled, Is.True);
        }
    }
}