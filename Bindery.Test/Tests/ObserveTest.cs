using System;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class ObserveTest
    {
        private TestViewModel _viewModel;
        private TestButton _button;
        private ISourceBinder<TestViewModel> _binder;
        private TestCommand _command;

        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _command = new TestCommand(_viewModel);
            _button = new TestButton();
            _binder = Create.Binder(_viewModel);
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _button.Dispose();
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ObserveControlAndSendToCommand(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            string mouseMoveButton = null;
            _command.ExecuteAction = parm => { mouseMoveButton = parm; };
            _command.CanExecuteCondition = vm => commandEnabled;
            _binder.Control(_button).Observe(c => c.MouseMoveButton).Execute(_command);

            // Act
            if (!binderActiveDuringEvent) _binder.Dispose();
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

            var expected = expectUpdated ? "Right" : null;

            // Assert
            Assert.That(mouseMoveButton, Is.EqualTo(expected));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ObserveControlAndSetSourceValue(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            _binder.Control(_button).Observe(c => c.MouseMoveButton).Set(vm => vm.StringValue);
            if (!binderActiveDuringEvent) _binder.Dispose();

            // Act
            _button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

            // Assert
            var expected = expectUpdated ? "Right" : null;
            Assert.That(_viewModel.StringValue, Is.EqualTo(expected));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ObserveSourceAndTakeAction(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var task = new Task<int>(()=>5);
            _viewModel.MyObservable = task.ToObservable();

            var result = 0;
            var complete = false;
            _binder.Observe(vm => vm.MyObservable)
                .OnNext(arg => result = arg)
                .OnComplete(() => complete = true)
                .Subscribe();
            if (!binderActiveDuringEvent) 
                _binder.Dispose();

            // Act
            task.Start();
            task.Wait();

            var expectedComplete = expectUpdated;
            var expectedResult = expectUpdated ? 5 : 0;
            ConditionalWait(()=>complete==expectedComplete && result==expectedResult);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(complete, Is.EqualTo(expectedComplete));
        }

        [Test]
        public void ObserveSourceWithException()
        {
            // Arrange
            var task = new Task<int>(() => { throw new NotImplementedException(); });
            _viewModel.MyObservable = task.ToObservable();

            Exception thrown = null;
            _binder.Observe(vm => vm.MyObservable)
                .OnNext(arg => { })
                .OnError(ex=> thrown = ex)
                .Subscribe();

            // Act
            task.Start();
            ConditionalWait(() => task.IsFaulted && thrown != null);

            // Assert
            Assert.That(thrown, Is.InstanceOf<NotImplementedException>());
        }

        [Test]
        public void ObserveSourceWithCancellation()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;
            var task = new Task<int>(() => { Thread.Sleep(TimeSpan.FromMilliseconds(10)); ct.ThrowIfCancellationRequested(); return 5; }, tokenSource.Token);
            _viewModel.MyObservable = task.ToObservable();
            const int expected = 5;

            var result = 0;
            _binder.Observe(vm => vm.MyObservable)
                .OnNext(arg => result = arg)
                .CancellationToken(tokenSource.Token)
                .Subscribe();

            // Act
            task.Start();
            tokenSource.Cancel();
            Assert.Throws<OperationCanceledException>(() => task.Wait(tokenSource.Token));
            Assert.That(result, Is.Not.EqualTo(expected));
        }

        private static void ConditionalWait(Func<bool> condition)
        {
            Task.Run(() =>
            {
                while (!condition())
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }
            }).Wait(TimeSpan.FromSeconds(1));
            Assert.That(condition(), Is.True, "Failed to meet condition before timeout.");
        }

    }
}
