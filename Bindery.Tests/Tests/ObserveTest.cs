using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class ObserveTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _button = new TestButton();
            _binder = Binder.Source(_viewModel);
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _button.Dispose();
        }

        private TestViewModel _viewModel;
        private TestButton _button;
        private ISourceBinder<TestViewModel> _binder;

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ObserveControlAndSendToCommand(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            string mouseMoveButton = null;
            _binder.Observe(_button.MouseMoveButton).Subscribe(x => mouseMoveButton = x);

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
            _binder.Observe(_button.MouseMoveButton).Set(vm => vm.StringValue);
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
            var task = new Task<int>(() => 5);
            _viewModel.MyObservable = task.ToObservable();

            var result = 0;
            var complete = false;
            _binder.Observe(_viewModel.MyObservable)
                .Subscribe(ctx => ctx.OnNext(arg => result = arg).OnComplete(() => complete = true));
            if (!binderActiveDuringEvent)
                _binder.Dispose();

            // Act
            task.Start();
            task.Wait();

            var expectedComplete = expectUpdated;
            var expectedResult = expectUpdated ? 5 : 0;
            ConditionalWait.WaitFor(() => complete == expectedComplete && result == expectedResult);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(complete, Is.EqualTo(expectedComplete));
        }

        [Test]
        public void ObserveSourceWithCancellation()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;
            var task = new Task<int>(() =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                ct.ThrowIfCancellationRequested();
                return 5;
            }, tokenSource.Token);
            _viewModel.MyObservable = task.ToObservable();
            const int expected = 5;

            var result = 0;
            _binder.Observe(_viewModel.MyObservable).Subscribe(ctx => ctx.OnNext(arg => result = arg).CancellationToken(tokenSource.Token));

            // Act
            task.Start();
            tokenSource.Cancel();
            Assert.Throws<OperationCanceledException>(() => task.Wait(tokenSource.Token));
            Assert.That(result, Is.Not.EqualTo(expected));
        }

        [Test]
        public void ObserveSourceWithException()
        {
            // Arrange
            var task = new Task<int>(() => throw new InvalidOperationException());
            _viewModel.MyObservable = task.ToObservable();

            Exception thrown = null;
            _binder.Observe(_viewModel.MyObservable).Subscribe(ctx => ctx.OnNext(arg => { }).OnError(ex => thrown = ex));

            // Act
            task.Start();
            ConditionalWait.WaitFor(() => task.IsFaulted && thrown != null);

            // Assert
            Assert.That(thrown, Is.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void SpecifyDefaultScheduler()
        {
            // Arrange
            _binder = Binder.Source(_viewModel);
            var task = new Task<int>(() => 5);
            _viewModel.MyObservable = task.ToObservable();

            var bindingThread = Thread.CurrentThread;
            Thread actionThread = null;
            _binder.Observe(_viewModel.MyObservable).Subscribe(x => actionThread = Thread.CurrentThread);

            // Act
            task.Start();
            task.Wait();

            ConditionalWait.WaitFor(() => actionThread != null);

            // Assert
            Assert.That(actionThread, Is.Not.SameAs(bindingThread));
        }

        [Test]
        public void OverrideDefaultScheduler()
        {
            // Arrange
            var binder = Binder.Source(_viewModel, Scheduler.Default);
            var observableThreadId = 0;
            var observable = Observable.Create<int>(o =>
            {
                observableThreadId = Thread.CurrentThread.ManagedThreadId;
                o.OnNext(5);
                return Disposable.Empty;
            }).SubscribeOn(NewThreadScheduler.Default);

            var subscriptionThreadId = 0;
            binder.Observe(observable)
                .ObserveOn(Scheduler.CurrentThread) // CurrentThread means observable's thread
                .Subscribe(x => subscriptionThreadId = Thread.CurrentThread.ManagedThreadId);

            // Act
            ConditionalWait.WaitFor(() => subscriptionThreadId > 0);

            // Assert
            Assert.That(subscriptionThreadId, Is.EqualTo(observableThreadId), 
                "Expected subscribed action to run on same thread as the observable.");
        }

        [Test]
        public void SimpleAsyncSubscription()
        {
            // Arrange
            var task = new Task<int>(() => 5);
            _viewModel.MyObservable = task.ToObservable();

            var result = 0;
            _binder.Observe(_viewModel.MyObservable).SubscribeAsync(SetResultAsync);

            // Act
            task.Start();
            task.Wait();

            var expectedResult = 5;
            ConditionalWait.WaitFor(() => result == expectedResult);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));

            async Task SetResultAsync(int value)
            {
                await Task.Run(() => { result = value; });
            }
        }

        [Test]
        public void ComplexAsyncSubscription()
        {
            // Arrange
            var task = new Task<int>(() => 5);
            _viewModel.MyObservable = task.ToObservable();

            var result = 0;
            var complete = false;
            _binder.Observe(_viewModel.MyObservable)
                .Subscribe(ctx => ctx.OnNextAsync(SetResultAsync).OnComplete(() => complete = true));

            // Act
            task.Start();
            task.Wait();

            var expectedResult = 5;
            ConditionalWait.WaitFor(() => complete && result == expectedResult);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(complete, Is.True);

            async Task SetResultAsync(int value)
            {
                await Task.Run(() => { result = value; });
            }
        }

        [Test]
        public void OnNextNullArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => _binder.Observe(_viewModel.MyObservable)
                    .Subscribe(ctx => ctx.OnNext(null))
            );
            Assert.That(ex.ParamName, Is.EqualTo("onNext"));
        }

        [Test]
        public void OnNextAsyncNullArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => _binder.Observe(_viewModel.MyObservable)
                    .Subscribe(ctx => ctx.OnNextAsync(null))
            );
            Assert.That(ex.ParamName, Is.EqualTo("onNext"));
        }

        [Test]
        public void OnErrorNullArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => _binder.Observe(_viewModel.MyObservable).Subscribe(
                    ctx => ctx.OnNext(x => { }).OnError(null))
            );
            Assert.That(ex.ParamName, Is.EqualTo("onError"));
        }

        [Test]
        public void OnCompleteNullArgumentException()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => _binder.Observe(_viewModel.MyObservable).Subscribe(
                    ctx => ctx.OnNext(x => { }).OnError(_ => { }).OnComplete(null))
            );
            Assert.That(ex.ParamName, Is.EqualTo("onCompleted"));
        }
    }
}